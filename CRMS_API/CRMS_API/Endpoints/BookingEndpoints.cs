using CRMS_API.Data;
using CRMS_API.DTOs;
using CRMS_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRMS_API.Endpoints
{
    public static class BookingEndpoints
    {
        public static void MapBookingEndpoints(this WebApplication app)
        {
            // POST /bookings — Customer creates a new booking
            app.MapPost("/bookings", async (CreateBookingDTO request, AppDbContext db, ClaimsPrincipal user) =>
            {
                var customerId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var car = db.Cars.Find(request.CarId);
                if (car == null) return Results.NotFound("Car not found");

                // Availability check — return 409 if car is already booked for overlapping dates
                var conflict = db.Bookings.Any(b =>
                    b.CarId == request.CarId &&
                    (b.Status == "Approved" || b.Status == "Active") &&
                    b.PickupDate < request.ReturnDate &&
                    b.ReturnDate > request.PickupDate);

                if (conflict) return Results.Conflict("Car is not available for the selected dates");

                // Calculate total amount server-side
                var days = (request.ReturnDate - request.PickupDate).Days;
                if (days <= 0) return Results.BadRequest("Return date must be after pickup date");

                var booking = new Booking
                {
                    CustomerId = customerId,
                    CarId = request.CarId,
                    PickupDate = request.PickupDate,
                    ReturnDate = request.ReturnDate,
                    TotalAmount = car.DailyRate * days,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                db.Bookings.Add(booking);
                await db.SaveChangesAsync();

                return Results.Created($"/bookings/{booking.Id}", new ResponseBookingDTO
                {
                    Id = booking.Id,
                    CustomerId = booking.CustomerId,
                    CustomerName = user.FindFirstValue(ClaimTypes.Name)!,
                    CarId = booking.CarId,
                    CarDetails = $"{car.Year} {car.Make} {car.Model}",
                    PickupDate = booking.PickupDate,
                    ReturnDate = booking.ReturnDate,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status,
                    CreatedAt = booking.CreatedAt
                });
            }).RequireAuthorization(p => p.RequireRole("Customer"));

            // GET /bookings/my — Customer views their own bookings only
            app.MapGet("/bookings/my", async (AppDbContext db, ClaimsPrincipal user) =>
            {
                var customerId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var bookings = await db.Bookings
                    .Include(b => b.Car)
                    .Include(b => b.Customer)
                    .Where(b => b.CustomerId == customerId)
                    .Select(b => new ResponseBookingDTO
                    {
                        Id = b.Id,
                        CustomerId = b.CustomerId,
                        CustomerName = b.Customer!.FullName,
                        CarId = b.CarId,
                        CarDetails = $"{b.Car!.Year} {b.Car.Make} {b.Car.Model}",
                        PickupDate = b.PickupDate,
                        ReturnDate = b.ReturnDate,
                        TotalAmount = b.TotalAmount,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt
                    }).ToListAsync();

                return Results.Ok(bookings);
            }).RequireAuthorization(p => p.RequireRole("Customer"));

            // DELETE /bookings/{id} — Customer cancels their own Pending booking
            app.MapDelete("/bookings/{id}", async (int id, AppDbContext db, ClaimsPrincipal user) =>
            {
                var customerId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var booking = db.Bookings.Find(id);
                if (booking == null) return Results.NotFound("Booking not found");

                // Make sure the booking belongs to this customer
                if (booking.CustomerId != customerId) return Results.Forbid();

                // Can only cancel Pending bookings
                if (booking.Status != "Pending")
                    return Results.BadRequest("Only Pending bookings can be cancelled");

                booking.Status = "Cancelled";
                await db.SaveChangesAsync();

                return Results.Ok("Booking cancelled successfully");
            }).RequireAuthorization(p => p.RequireRole("Customer"));

            // GET /bookings — Staff/Admin views all bookings
            app.MapGet("/bookings", async (AppDbContext db) =>
            {
                var bookings = await db.Bookings
                    .Include(b => b.Car)
                    .Include(b => b.Customer)
                    .Select(b => new ResponseBookingDTO
                    {
                        Id = b.Id,
                        CustomerId = b.CustomerId,
                        CustomerName = b.Customer!.FullName,
                        CarId = b.CarId,
                        CarDetails = $"{b.Car!.Year} {b.Car.Make} {b.Car.Model}",
                        PickupDate = b.PickupDate,
                        ReturnDate = b.ReturnDate,
                        TotalAmount = b.TotalAmount,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt
                    }).ToListAsync();

                return Results.Ok(bookings);
            }).RequireAuthorization(p => p.RequireRole("Staff", "Admin"));

            // PUT /bookings/{id}/approve — Staff/Admin approves a Pending booking
            app.MapPut("/bookings/{id}/approve", async (int id, AppDbContext db, ClaimsPrincipal user) =>
            {
                var staffId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var booking = db.Bookings.Find(id);
                if (booking == null) return Results.NotFound("Booking not found");

                if (booking.Status != "Pending")
                    return Results.BadRequest("Only Pending bookings can be approved");

                booking.Status = "Approved";
                booking.ApprovedById = staffId;
                await db.SaveChangesAsync();

                return Results.Ok("Booking approved");
            }).RequireAuthorization(p => p.RequireRole("Staff", "Admin"));

            // PUT /bookings/{id}/reject — Staff/Admin rejects a Pending booking
            app.MapPut("/bookings/{id}/reject", async (int id, AppDbContext db, ClaimsPrincipal user) =>
            {
                var staffId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var booking = db.Bookings.Find(id);
                if (booking == null) return Results.NotFound("Booking not found");

                if (booking.Status != "Pending")
                    return Results.BadRequest("Only Pending bookings can be rejected");

                booking.Status = "Rejected";
                booking.ApprovedById = staffId;
                await db.SaveChangesAsync();

                return Results.Ok("Booking rejected");
            }).RequireAuthorization(p => p.RequireRole("Staff", "Admin"));

            // PUT /bookings/{id}/complete — Staff/Admin marks an Active booking as Completed
            app.MapPut("/bookings/{id}/complete", async (int id, AppDbContext db) =>
            {
                var booking = db.Bookings.Find(id);
                if (booking == null) return Results.NotFound("Booking not found");

                if (booking.Status != "Active")
                    return Results.BadRequest("Only Active bookings can be marked as Completed");

                booking.Status = "Completed";
                await db.SaveChangesAsync();

                return Results.Ok("Booking marked as completed");
            }).RequireAuthorization(p => p.RequireRole("Staff", "Admin"));
        }
    }
}