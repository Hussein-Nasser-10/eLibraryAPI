using eLibraryAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using eLibraryAPI.Services;
using eLibraryAPI.Models.Models;
using eLibraryAPI.Models.Dtos;

namespace eLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Users,Admins")]
    public class BookController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IBooksService _booksService;

        public BookController(ApplicationDbContext context,IBooksService booksService)
        {
            _context = context;
            _booksService = booksService;
        }

        [HttpGet("ReadAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _booksService.GetBooks();
            return Ok(books);
        }

        // Search books
        [HttpPost("searchBooks")]
        public async Task<IActionResult> SearchBooks(BookSearchModel model)
        {
            var books = await _booksService.searchBook(model);
            if (books == null || books.Count == 0)
            {
                return NotFound("No books with the provided criteria");
            }
            return Ok(books);
        }

    }
}
