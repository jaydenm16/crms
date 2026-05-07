using CRMS_API.Data;
using CRMS_API.DTOs;
using CRMS_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CRMS_API.Endpoints
{
    public static class CarEndpoints
    {
        public static void MapCarEndpoints(this WebApplication app)
        {
            // GET /cars — any authenticated user can view all cars
            app.MapGet("/cars", (AppDbContext db) =>
            {
                var cars = db.Cars.Select(c => new ResponseCarDTO
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Year = c.Year,
                    Category = c.Category,
                    DailyRate = c.DailyRate,
                    LicencePlate = c.LicencePlate,
                    Colour = c.Colour,
                    Status = c.Status
                }).ToList();

                return Results.Ok(cars);
            }).RequireAuthorization();

            // GET /cars/{id} — any authenticated user can view a specific car
            app.MapGet("/cars/{id}", (int id, AppDbContext db) =>
            {
                var car = db.Cars.Find(id);
                if (car == null) return Results.NotFound("Car not found");

                return Results.Ok(new ResponseCarDTO
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Category = car.Category,
                    DailyRate = car.DailyRate,
                    LicencePlate = car.LicencePlate,
                    Colour = car.Colour,
                    Status = car.Status
                });
            }).RequireAuthorization();

            // POST /cars — Admin only, add a new car
            app.MapPost("/cars", async (CreateCarDTO request, AppDbContext db) =>
            {
                // Check if licence plate is already in use
                if (db.Cars.Any(c => c.LicencePlate == request.LicencePlate))
                    return Results.Conflict("Licence plate already exists");

                var car = new Car
                {
                    Make = request.Make,
                    Model = request.Model,
                    Year = request.Year,
                    Category = request.Category,
                    DailyRate = request.DailyRate,
                    LicencePlate = request.LicencePlate,
                    Colour = request.Colour,
                    Status = request.Status
                };

                db.Cars.Add(car);
                await db.SaveChangesAsync();

                return Results.Created($"/cars/{car.Id}", new ResponseCarDTO
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Category = car.Category,
                    DailyRate = car.DailyRate,
                    LicencePlate = car.LicencePlate,
                    Colour = car.Colour,
                    Status = car.Status
                });
            }).RequireAuthorization(p => p.RequireRole("Admin"));

            // PUT /cars/{id} — Admin only, update an existing car
            app.MapPut("/cars/{id}", async (int id, CreateCarDTO request, AppDbContext db) =>
            {
                var car = db.Cars.Find(id);
                if (car == null) return Results.NotFound("Car not found");

                car.Make = request.Make;
                car.Model = request.Model;
                car.Year = request.Year;
                car.Category = request.Category;
                car.DailyRate = request.DailyRate;
                car.LicencePlate = request.LicencePlate;
                car.Colour = request.Colour;
                car.Status = request.Status;

                await db.SaveChangesAsync();

                return Results.Ok(new ResponseCarDTO
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Category = car.Category,
                    DailyRate = car.DailyRate,
                    LicencePlate = car.LicencePlate,
                    Colour = car.Colour,
                    Status = car.Status
                });
            }).RequireAuthorization(p => p.RequireRole("Admin"));

            // DELETE /cars/{id} — Admin only, remove a car
            app.MapDelete("/cars/{id}", async (int id, AppDbContext db) =>
            {
                var car = db.Cars.Find(id);
                if (car == null) return Results.NotFound("Car not found");

                db.Cars.Remove(car);
                await db.SaveChangesAsync();

                return Results.Ok("Car removed successfully");
            }).RequireAuthorization(p => p.RequireRole("Admin"));
        }
    }
}