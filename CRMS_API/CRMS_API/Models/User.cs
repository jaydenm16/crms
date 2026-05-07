using System.ComponentModel.DataAnnotations;

namespace CRMS_API.Models
{
    public class User
    {
            public int Id { get; set; }

            [MaxLength(100)]
            public string Username { get; set; } = string.Empty;

            [MaxLength(256)]
            public string PasswordHash { get; set; } = string.Empty; // SHA-256, never plain text

            [MaxLength(50)]
            public string Role { get; set; } = string.Empty; // Customer | Staff | Admin

            [MaxLength(150)]
            public string FullName { get; set; } = string.Empty;

            [MaxLength(150)]
            public string Email { get; set; } = string.Empty;

            [MaxLength(20)]
            public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
