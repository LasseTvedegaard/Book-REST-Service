// Import the Model namespace to access the Genre and Location classes.
using Model;

// Namespace named DTOs to encapsulate the data transfer objects.
namespace DTOs {

    public class BookOutDto {

        public int? BookId { get; set; } // Nullable integer to store the ID of the book.

        public string Title { get; set; } // String to store the title of the book.

        public string Author { get; set; } // String to store the author of the book.

        public Genre? Genre { get; set; } // Nullable Genre object to represent the genre of the book.

        public int NoOfPages { get; set; } // Integer to store the number of pages in the book.

        public string BookType { get; set; } // String to store the type of the book (e.g., Hardcover, Paperback).

        public string IsbnNo { get; set; } // String to store the ISBN number of the book.

        public Location? Location { get; set; } // Nullable Location object to represent the physical location of the book.

        public string Status { get; set; } // String to represent the status of the book (e.g., Available, Checked-Out).

        public string ImageURL { get; set; } // String to store the URL of the book's image.
        
        // Default constructor for the BookOutDto class.
        public BookOutDto() { }

        // Constructor for initializing BookOutDto objects.
        public BookOutDto(int? bookId, string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL) {
            BookId = bookId;
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
