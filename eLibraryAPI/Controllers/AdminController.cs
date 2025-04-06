using eLibraryAPI.Data;
using eLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Add a new book
        [HttpPost("addBook")]
        public async Task<IActionResult> AddBook([FromBody] Book book, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                book.ImageUrl = "/uploads/" + uniqueFileName;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok("Book added successfully.");
        }

        // Remove a book
        [HttpDelete("removeBook/{id}")]
        public async Task<IActionResult> RemoveBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound("Book not found.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book removed successfully.");
        }

        // Add a new user
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user.PasswordHash.Length < 8)
                return BadRequest("Password must be at least 8 characters long.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User added successfully.");
        }

        // Remove a user
        [HttpDelete("removeUser/{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("User removed successfully.");
        }

        // Admin borrowing books
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(int userId, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.Amount <= 0)
                return BadRequest("Book is unavailable.");

            var borrow = new Borrow
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow
            };

            _context.Borrows.Add(borrow);
            book.Amount -= 1;

            await _context.SaveChangesAsync();
            return Ok("Book borrowed successfully.");
        }
    }
}
