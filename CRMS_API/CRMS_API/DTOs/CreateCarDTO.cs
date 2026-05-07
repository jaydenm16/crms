namespace CRMS_API.DTOs
{
    public class CreateCarDTO
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public string LicencePlate { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
    }
}