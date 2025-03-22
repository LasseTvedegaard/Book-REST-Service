// Import the Model namespace to access the Genre and Location classes.
using Model;

// Namespace named DTOs to encapsulate the data transfer objects.
namespace DTOs
{
    public class BookOutDto
    {
        public int? BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre? Genre { get; set; }
        public int NoOfPages { get; set; }
        public string BookType { get; set; }
        public string IsbnNo { get; set; }
        public Location? Location { get; set; }
        public string Status { get; set; }
        public string ImageURL { get; set; }

        public BookOutDto() { }

        public BookOutDto(int? bookId, string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL)
        {
            BookId = bookId;
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
