using System.ComponentModel.DataAnnotations;

namespace eLibraryAPI.Data.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        [MaxLength(50)]
        public string Genre { get; set; }

        [Required]
        public int Amount { get; set; } // Number of copies available

        public string Status => Amount > 0 ? "Available" : "Unavailable";

        public string? ImageUrl { get; set; } // New field for book image

        public ICollection<Borrow> Borrows { get; set; }
    }
}
