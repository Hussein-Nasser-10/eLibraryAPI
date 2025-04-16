using eLibraryAPI.Data;
using eLibraryAPI.Data.Models;
using eLibraryAPI.Models.Models;
using eLibraryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace eLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admins")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IBooksService _booksService;
        private readonly IUserService _userService;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment,IBooksService booksService,IUserService userService)
        {
            _context = context;
            _environment = environment;
            _booksService = booksService;
            _userService = userService;
        }

        //// Add a new book
        //[HttpPost("addBook1")]
        //public async Task<IActionResult> AddBook([FromBody] Book book, IFormFile imageFile)
        //{
        //    if (imageFile != null && imageFile.Length > 0)
        //    {
        //        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await imageFile.CopyToAsync(stream);
        //        }
        //        book.ImageUrl = "/uploads/" + uniqueFileName;
        //    }

        //    _context.Books.Add(book);
        //    await _context.SaveChangesAsync();
        //    return Ok("Book added successfully.");
        //}

        [HttpPost("addBook")]
        public async Task<IActionResult> addBook([FromForm] BookModel book)
        {
            string imageUrl = await _booksService.uploadImage(book.Image);
            book.ImageUrl = imageUrl;
            var bookId = await _booksService.addBook(book);
            return Ok("Book added successfully");
        }

        // Remove a book
        [HttpDelete("removeBook/{id}")]
        public async Task<IActionResult> RemoveBook(int id)
        {
            var deleteState = await _booksService.deleteBook(id);
            if (!deleteState) return NotFound("Book not found");
            return Ok("Book removed successfully.");
        }


        [HttpPut("updateBook")]
        public async Task<IActionResult> UpdateBook(BookModel model)
        {
            await _booksService.updateBook(model);
            return Ok("Book updated successfully.");
        }

        // Add a new user
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] UserModel user)
        {
            if (user.Password.Length < 8)
                return BadRequest("Password must be at least 8 characters long.");

            var userState = await _userService.createUser(user);
            if (!userState) return BadRequest("Something went wrong");
            return Ok("User added successfully.");
        }

        // Remove a user
        [HttpDelete("removeUser/{id}")]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var deleteState = await _userService.deleteUser(id);
            if (!deleteState)
            {
                return BadRequest("operation is not successful, please check the provided user info to remove!");
            }
            return Ok("user deleted successfully");
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
