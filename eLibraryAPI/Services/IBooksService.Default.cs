using AutoMapper;
using eLibraryAPI.Data;
using eLibraryAPI.Data.Models;
using eLibraryAPI.Models.Dtos;
using eLibraryAPI.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eLibraryAPI.Services
{
    public class BooksService : IBooksService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BooksService(ApplicationDbContext context,IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<BookDto>> GetBooks()
        {
            var books = await _context.Books
                .Select(x => new BookDto
                {
                    Id = x.BookId,
                    Title = x.Title,
                    Author = x.Author,
                    Genre = x.Genre,
                    Amount = x.Amount,
                    ImageUrl = x.ImageUrl
                }).ToListAsync();

            return books;
        }

        public async Task<int> addBook(BookModel bookModel)
        {
            var bookId = -1;
            var book = _mapper.Map<Book>(bookModel);
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            bookId = book.BookId;
            return bookId;
        }

        public async Task<string> uploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Build the full image URL
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}"; // e.g., https://localhost:5001
            var imageUrl = $"{baseUrl}/images/{uniqueFileName}";

            return imageUrl;
        }


        public async Task<int> updateBook(BookModel bookModel)
        {
            var book = await _context.Books.Where(x => x.BookId == bookModel.Id).FirstOrDefaultAsync();
            book.Amount = bookModel.Amount;
            book.Author = bookModel.Author;
            book.Title = bookModel.Title;
            book.Genre = bookModel.Genre;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book.BookId;
        }

        public async Task<bool> deleteBook(int bookId)
        {
            var book = await _context.Books.Where(x => x.BookId == bookId).FirstOrDefaultAsync();
            if (book == null) return false;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BookDto>> searchBook(BookSearchModel model)
        {
            var books = await _context.Books
                .Where(x => (string.IsNullOrEmpty(model.Author) || model.Author == x.Author)
                    && (string.IsNullOrEmpty(model.Genre) || model.Genre == x.Genre)
                    && (string.IsNullOrEmpty(model.Title) || model.Title == x.Title)
                )
                .Select(x => _mapper.Map<BookDto>(x))
                .ToListAsync();
            return books;
        }
    }
}
