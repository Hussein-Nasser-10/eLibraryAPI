using System.ComponentModel.DataAnnotations;

namespace eLibraryAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string PasswordHash { get; set; } // Hashed password

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // "Admin" or "User"

        public ICollection<Borrow> Borrows { get; set; }
    }
}
