using eLibraryAPI.Data;
using eLibraryAPI.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("BorrowedBooks")]
        public async Task<IActionResult> GetBorrowedBook()
        {
            var books = await _context.Borrows
                .Select(x => new
                {
                    x.UserId,
                    UserName = x.User.Username,
                    x.BookId,
                    BookName = x.Book.Title
                }).ToListAsync();
            return Ok(books);
        }

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
            book.Amount -= 1; // Reduce amount

            await _context.SaveChangesAsync();

            return Ok(new { message = "Book borrowed successfully." });
        }
    }
}
