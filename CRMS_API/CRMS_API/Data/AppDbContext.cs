using Microsoft.EntityFrameworkCore;
using CRMS_API.Models;

namespace CRMS_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure Username and LicencePlate are unique in the database
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.LicencePlate)
                .IsUnique();

            // A booking belongs to a customer (User)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // A booking belongs to a car
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Car)
                .WithMany()
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            // A booking can optionally be approved by a staff member
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ApprovedBy)
                .WithMany()
                .HasForeignKey(b => b.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
