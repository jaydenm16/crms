using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMS_API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } // Calculated server-side: DailyRate × number of days

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending | Approved | Active | Completed | Cancelled | Rejected
        public int? ApprovedById { get; set; } // Nullable — only set when staff approves/rejects
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — EF Core uses these to understand the relationships & keys
        public User? Customer { get; set; }
        public Car? Car { get; set; }
        public User? ApprovedBy { get; set; }
    }
}