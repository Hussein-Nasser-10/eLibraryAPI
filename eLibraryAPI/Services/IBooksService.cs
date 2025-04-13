using eLibraryAPI.Models.Dtos;
using eLibraryAPI.Models.Models;

namespace eLibraryAPI.Services
{
    public interface IBooksService
    {
        public Task<List<BookDto>> GetBooks();
        public Task<int> addBook(BookModel bookModel);
        public Task<int> updateBook(BookModel bookModel);
        public Task<bool> deleteBook(int bookId);
        public Task<List<BookDto>> searchBook(BookSearchModel model);
        public Task<string> uploadImage(IFormFile imageFile);
    }
}
