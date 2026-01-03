namespace Model
{
    public class Log
    {
        public int LogId { get; set; }
        public int BookId { get; set; }

        // ✅ MATCHER DB (nvarchar)
        public string UserId { get; set; } = string.Empty;

        public int CurrentPage { get; set; }
        public int NoOfPages { get; set; }
        public string ListType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Book? Book { get; set; }

        public Log() { }

        public Log(
            int logId,
            int bookId,
            string userId,     // ✅ string
            int currentPage,
            int totalPages,
            string listType,
            DateTime createdAt)
        {
            LogId = logId;
            BookId = bookId;
            UserId = userId;
            CurrentPage = currentPage;
            NoOfPages = totalPages;
            ListType = listType;
            CreatedAt = createdAt;
        }
    }
}
