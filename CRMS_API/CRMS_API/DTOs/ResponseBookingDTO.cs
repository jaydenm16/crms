namespace CRMS_API.DTOs
{
    public class ResponseBookingDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int CarId { get; set; }
        public string CarDetails { get; set; } = string.Empty; // e.g. "2022 Toyota Camry"
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}