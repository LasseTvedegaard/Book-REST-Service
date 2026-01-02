namespace DTOs
{
    // Used when creating/updating a book (input from client)
    public class BookInDto
    {
        public string Title { get; set; }
        public string Author { get; set; }

        public int GenreId { get; set; }
        public int LocationId { get; set; }

        public int NoOfPages { get; set; }
        public string BookType { get; set; }
        public string IsbnNo { get; set; }
        public string Status { get; set; }
        public string ImageURL { get; set; }

        public BookInDto() { }

        public BookInDto(
            string title,
            string author,
            int genreId,
            int noOfPages,
            string bookType,
            string isbnNo,
            int locationId,
            string status,
            string imageURL)
        {
            Title = title?.Trim();
            Author = author?.Trim();
            GenreId = genreId;
            NoOfPages = noOfPages;
            BookType = bookType?.Trim();
            IsbnNo = isbnNo?.Trim();
            LocationId = locationId;
            Status = status?.Trim();
            ImageURL = imageURL?.Trim();
        }
    }
}
