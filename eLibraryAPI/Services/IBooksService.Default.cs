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
        public BooksService(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<int> updateBook(BookModel bookModel)
        {
            var book = await _context.Books.Where(x => x.BookId == bookModel.Id).FirstOrDefaultAsync();
            book = _mapper.Map<Book>(bookModel);
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
    }
}
