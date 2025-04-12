namespace eLibraryAPI.Models.Models
{
    public class BookModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Amount { get; set; }
        public string ImageUrl { get; set; }
    }
}
