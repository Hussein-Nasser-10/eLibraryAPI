namespace eLibraryAPI
{
    using AutoMapper;
    using eLibraryAPI.Data.Models;
    using eLibraryAPI.Models.Dtos;
    using eLibraryAPI.Models.Models;

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Book, BookModel>();
            CreateMap<BookModel, Book>();
        }
    }
}
