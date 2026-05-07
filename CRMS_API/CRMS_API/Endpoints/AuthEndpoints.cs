using CRMS_API.Data;
using CRMS_API.DTOs;
using CRMS_API.Helpers;
using CRMS_API.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace CRMS_API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            // POST /auth/register — public, anyone can register as a Customer
            app.MapPost("/auth/register", async (CreateUserDTO request, AppDbContext db) =>
            {
                // Check if username is already taken
                if (db.Users.Any(u => u.Username == request.Username))
                    return Results.Conflict("Username already exists");

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = PasswordHasher.HashPassword(request.Password),
                    Role = "Customer", // Always register as Customer
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Created($"/users/{user.Id}", new ResponseUserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    CreatedAt = user.CreatedAt
                });
            });

            // GET /users — Admin only, returns all registered users
            app.MapGet("/users", (AppDbContext db) =>
            {
                var users = db.Users.Select(u => new ResponseUserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                }).ToList();

                return Results.Ok(users);
            }).RequireAuthorization(p => p.RequireRole("Admin"));
        }
    }
}