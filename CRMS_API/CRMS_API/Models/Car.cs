using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMS_API.Models
{
    public class Car
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Make { get; set; } = string.Empty;        // e.g. Toyota

        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;       // e.g. Camry
        public int Year { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;    // e.g. Sedan, SUV, Van

        [Column(TypeName = "decimal(10,2)")]
        public decimal DailyRate { get; set; }

        [MaxLength(20)]
        public string LicencePlate { get; set; } = string.Empty; // Must be unique

        [MaxLength(50)]
        public string Colour { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Available";       // Available | Rented | Maintenance
    }
}