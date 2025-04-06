using eLibraryAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace eLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Add these methods inside your BookController

        // Search books by title
        //git healthcheck
        [HttpGet("searchByTitle")]
        public async Task<IActionResult> SearchBooksByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title cannot be empty.");
            }

           var books = await _context.Books
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .ToListAsync();

            if (books.Count == 0)
            {
                return NotFound("No books found with that title.");
            }

            return Ok(books);
        }

        // Filter books by genre
        [HttpGet("filterByGenre")]
        public async Task<IActionResult> FilterBooksByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return BadRequest("Genre cannot be empty.");
            }

            var books = await _context.Books
                .Where(b => b.Genre.ToLower().Contains(genre.ToLower()))
                .ToListAsync();

            if (books.Count == 0)
            {
                return NotFound("No books found with that genre.");
            }

            return Ok(books);
        }

        // Filter books by author
        [HttpGet("filterByAuthor")]
        public async Task<IActionResult> FilterBooksByAuthor(string author)
        {
            if (string.IsNullOrEmpty(author))
            {
                return BadRequest("Author cannot be empty.");
            }

            var books = await _context.Books
                .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                .ToListAsync();

            if (books.Count == 0)
            {
                return NotFound("No books found with that author.");
            }

            return Ok(books);
        }

    }
}
