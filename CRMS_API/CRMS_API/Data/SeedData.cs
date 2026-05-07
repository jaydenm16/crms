using CRMS_API.Helpers;
using CRMS_API.Models;

namespace CRMS_API.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            // Only seed if no users exist yet
            if (db.Users.Any()) return;

            db.Users.AddRange(
                new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("admin123"),
                    Role = "Admin",
                    FullName = "System Admin",
                    Email = "admin@crms.com",
                    Phone = "555-0001",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "staff",
                    PasswordHash = PasswordHasher.HashPassword("staff123"),
                    Role = "Staff",
                    FullName = "Staff Member",
                    Email = "staff@crms.com",
                    Phone = "555-0002",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "customer",
                    PasswordHash = PasswordHasher.HashPassword("customer123"),
                    Role = "Customer",
                    FullName = "Test Customer",
                    Email = "customer@crms.com",
                    Phone = "555-0003",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Only seed cars if none exist
            if (!db.Cars.Any())
            {
                db.Cars.AddRange(
                    new Car { Make = "Toyota", Model = "Camry", Year = 2022, Category = "Sedan", DailyRate = 50.00m, LicencePlate = "TYT001", Colour = "Black", Status = "Available" },
                    new Car { Make = "Honda", Model = "CR-V", Year = 2023, Category = "SUV", DailyRate = 75.00m, LicencePlate = "HND002", Colour = "White", Status = "Available" },
                    new Car { Make = "Ford", Model = "Transit", Year = 2021, Category = "Van", DailyRate = 90.00m, LicencePlate = "FRD003", Colour = "Silver", Status = "Available" }
                );
                db.SaveChanges();
            }

            db.SaveChanges();
        }
    }
}