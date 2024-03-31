using Model;

// The DTOs namespace is used for Data Transfer Objects
namespace DTOs {

    // The BookInDto class is used for receiving book information from the client.
    public class BookInDto {

        // Title of the book.
        public string Title { get; set; }

        // Author of the book.
        public string Author { get; set; }

        // Genre of the book. Nullable type allows for a null value.
        public Genre? Genre { get; set; }

        // Number of pages in the book.
        public int NoOfPages { get; set; }

        // Type of the book (e.g., Hardcover, Paperback, etc.)
        public string BookType { get; set; }

        // ISBN number of the book.
        public string IsbnNo { get; set; }

        // Location where the book is stored. Nullable type allows for a null value.
        public Location? Location { get; set; }

        // Current status of the book (e.g., Available, Checked Out, etc.)
        public string Status { get; set; }

        // URL for the book's cover image.
        public string ImageURL { get; set; }

        // Default constructor.
        public BookInDto() { }

        // Parameterized constructor to initialize properties.
        public BookInDto(string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL) {
            Title = title;
            Author = author;
            Genre = genre;
            NoOfPages = noOfPages;
            BookType = bookType;
            IsbnNo = isbnNo;
            Location = location;
            Status = status;
            ImageURL = imageURL;
        }
    }
}
