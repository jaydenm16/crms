namespace CRMS_API.DTOs
{
    public class CreateBookingDTO
    {
        public int CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}