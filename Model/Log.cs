namespace Model {
    public class Log {
        public int LogId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int CurrentPage { get; set; }
        public int NoOfPages { get; set; }
        public string ListType { get; set; }
        public User User { get; set; }
        public Book Book { get; set; }

        public Log() { }

        public Log(int logId, int bookId, int userId, int currentPage, int totalPages, string listType) {
            LogId = logId;
            BookId = bookId;
            UserId = userId;
            CurrentPage = currentPage;
            NoOfPages = totalPages;
            ListType = listType;
        }
    }
}
