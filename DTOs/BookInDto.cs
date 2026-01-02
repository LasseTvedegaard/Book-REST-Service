using Model;

// The DTOs namespace is used for Data Transfer Objects
namespace DTOs
{

    // The BookInDto class is used for receiving book information from the client.
    public class BookInDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre? Genre { get; set; }
        public int NoOfPages { get; set; }
        public string BookType { get; set; }
        public string IsbnNo { get; set; }
        public Location? Location { get; set; }
        public string Status { get; set; }
        public string ImageURL { get; set; }

        public BookInDto() { }

        public BookInDto(string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL)
        {
            Title = title?.Trim();
            Author = author?.Trim();
            Genre = genre;
            NoOfPages = noOfPages;
            BookType = bookType?.Trim();
            IsbnNo = isbnNo?.Trim();
            Location = location;
            Status = status?.Trim();
            ImageURL = imageURL?.Trim();
        }
    }
}
