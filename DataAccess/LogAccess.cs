using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Model;

namespace DataAccess
{
    public class LogAccess : ILogAccess
    {
        private readonly IConnection _connection;
        private readonly ILogger<LogAccess> _logger;
        private readonly IBookAccess _bookAccess;

        public LogAccess(
            IGenericConnection<LibraryConnection> connection,
            IBookAccess bookAccess,
            ILogger<LogAccess> logger)
        {
            _connection = connection;
            _bookAccess = bookAccess;
            _logger = logger;
        }

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<int> Create(Log entity)
        {
            entity.CreatedAt = DateTime.Now;

            const string sql = @"
                INSERT INTO Log (bookId, userId, currentPage, noOfPages, listType, createdAt)
                VALUES (@BookId, @UserId, @CurrentPage, @NoOfPages, @ListType, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var db = _connection.GetConnection();

            return await db.ExecuteScalarAsync<int>(sql, entity);
        }

        // -----------------------------
        // GET BY ID (MED BOOK)
        // -----------------------------
        public async Task<Log?> GetLogById(int logId, string listType)
        {
            const string sql = @"
                SELECT 
                    l.logId, l.bookId, l.userId, l.currentPage, l.noOfPages, l.listType, l.createdAt,

                    b.bookId, 
                    b.title, 
                    b.author, 
                    b.noOfPages, 
                    b.bookType, 
                    b.isbnNo, 
                    b.status, 
                    b.imageURL, 
                    b.userId

                FROM Log l
                INNER JOIN Book b ON b.bookId = l.bookId
                WHERE l.logId = @logId
                  AND l.listType = @listType;";

            using var db = _connection.GetConnection();

            var result = await db.QueryAsync<Log, Book, Log>(
                sql,
                (log, book) =>
                {
                    log.Book = book;
                    return log;
                },
                new { logId, listType },
                splitOn: "bookId"
            );

            return result.FirstOrDefault();
        }

        // -----------------------------
        // GET LOGS BY USER + LIST TYPE (MED BOOK)
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLogsByUser(string userId, string listType)
        {
            const string sql = @"
                SELECT 
                    l.logId, l.bookId, l.userId, l.currentPage, l.noOfPages, l.listType, l.createdAt,

                    b.bookId, 
                    b.title, 
                    b.author, 
                    b.noOfPages, 
                    b.bookType, 
                    b.isbnNo, 
                    b.status, 
                    b.imageURL, 
                    b.userId

                FROM Log l
                INNER JOIN Book b ON b.bookId = l.bookId
                WHERE l.userId = @userId
                  AND l.listType = @listType
                ORDER BY l.createdAt DESC;";

            using var db = _connection.GetConnection();

            var result = await db.QueryAsync<Log, Book, Log>(
                sql,
                (log, book) =>
                {
                    log.Book = book;
                    return log;
                },
                new { userId, listType },
                splitOn: "bookId"
            );

            return result;
        }

        // -----------------------------
        // GET LATEST LOG PER BOOK (MED BOOK)  ⭐ DEN KRITISKE TIL DASHBOARD
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(string userId, string listType)
        {
            const string sql = @"
                SELECT 
                    l.logId, l.bookId, l.userId, l.currentPage, l.noOfPages, l.listType, l.createdAt,

                    b.bookId, 
                    b.title, 
                    b.author, 
                    b.noOfPages, 
                    b.bookType, 
                    b.isbnNo, 
                    b.status, 
                    b.imageURL, 
                    b.userId

                FROM Log l
                INNER JOIN Book b ON b.bookId = l.bookId
                WHERE l.userId = @userId
                  AND l.listType = @listType
                  AND l.logId IN (
                      SELECT MAX(logId)
                      FROM Log
                      WHERE userId = @userId
                        AND listType = @listType
                      GROUP BY bookId
                  )
                ORDER BY l.createdAt DESC;";

            using var db = _connection.GetConnection();

            var result = await db.QueryAsync<Log, Book, Log>(
                sql,
                (log, book) =>
                {
                    log.Book = book;
                    return log;
                },
                new { userId, listType },
                splitOn: "bookId"
            );

            return result;
        }

        // -----------------------------
        // GET FULL HISTORY (MED BOOK)
        // -----------------------------
        public async Task<List<Log>> GetAllLogs(string userId)
        {
            const string sql = @"
                SELECT 
                    l.logId, l.bookId, l.userId, l.currentPage, l.noOfPages, l.listType, l.createdAt,

                    b.bookId, 
                    b.title, 
                    b.author, 
                    b.noOfPages, 
                    b.bookType, 
                    b.isbnNo, 
                    b.status, 
                    b.imageURL, 
                    b.userId

                FROM Log l
                INNER JOIN Book b ON b.bookId = l.bookId
                WHERE l.userId = @userId
                ORDER BY l.createdAt DESC;";

            using var db = _connection.GetConnection();

            var result = await db.QueryAsync<Log, Book, Log>(
                sql,
                (log, book) =>
                {
                    log.Book = book;
                    return log;
                },
                new { userId },
                splitOn: "bookId"
            );

            return result.ToList();
        }

        public Task<bool> Update(Log log, string listType)
        {
            using var db = _connection.GetConnection();

            var sql = @"
                UPDATE Log
                SET currentPage = @CurrentPage,
                    noOfPages = @NoOfPages
                WHERE logId = @LogId 
                  AND userId = @UserId
                  AND listType = @ListType;";

            var rowsAffected = db.Execute(sql, new
            {
                log.CurrentPage,
                log.NoOfPages,
                log.LogId,
                log.UserId,
                ListType = listType
            });

            return Task.FromResult(rowsAffected > 0);
        }
    }
}
