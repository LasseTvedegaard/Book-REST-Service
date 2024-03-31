// Namespace for holding the data models of the application
namespace Model {

    // Class to represent a Book entity
    public class Book {

        // Properties for the Book class
        public int BookId { get; set; }  // ID for each book
        public string Title { get; set; }  // Title of the book
        public string Author { get; set; }  // Author of the book
        public Genre? Genre { get; set; }  // Optional Genre associated with the book
        public int NoOfPages { get; set; }  // Number of pages in the book
        public string BookType { get; set; }  // Type or format of the book (e.g., Hardcover, Paperback, eBook)
        public string IsbnNo { get; set; }  // ISBN number for the book
        public Location? Location { get; set; }  // Optional Location where the book can be found
        public string Status { get; set; }  // Status of the book (e.g., read, unread)
        public string ImageURL { get; set; }  // URL for the book's cover image

        // Default constructor
        public Book() { }

        // Constructor for initializing a Book object with all fields
        public Book(int bookId, string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL = null) {
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

        // Another constructor for initializing a Book object, without BookId
        public Book(string title, string author, Genre? genre, int noOfPages, string bookType, string isbnNo, Location? location, string status, string imageURL = null) {
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
