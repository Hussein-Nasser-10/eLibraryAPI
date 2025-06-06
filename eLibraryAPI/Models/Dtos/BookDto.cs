﻿namespace eLibraryAPI.Models.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Amount { get; set; }
        public string? ImageUrl { get; set; }
    }
}
