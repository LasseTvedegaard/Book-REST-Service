namespace Model
{
    public class Log
    {
        public int LogId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public int CurrentPage { get; set; }
        public int NoOfPages { get; set; }
        public string ListType { get; set; }
        public DateTime CreatedAt { get; set; } // ✅ korrekt placeret

        public User User { get; set; }
        public Book Book { get; set; }

        public Log() { }

        public Log(int logId, int bookId, string userId, int currentPage, int totalPages, string listType, DateTime createdAt)
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
