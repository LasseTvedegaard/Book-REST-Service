using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess {
    public class LogAccess : ILogAccess {

        private IConnection _connectionString;
        private readonly ILogger<LogAccess> _logger;
        private IBookAccess _bookAccess;
        private IUserAccess _userAccess;

        public LogAccess(IGenericConnection<LibraryConnection> _mssqlConnection, IConfiguration configuration, IBookAccess bookAccess, IUserAccess userAccess, ILogger<LogAccess> logger = null) {
            _connectionString = _mssqlConnection;
            _logger = logger;
            _bookAccess = bookAccess;
            _userAccess = userAccess;
        }

        public LogAccess(string connectionString) {
            //_connectionString = connectionString;
        }

        public async Task<int> Create(Log entity, string listType) {
            int statusCodeOrId = -1;

            var sql = @"INSERT INTO [dbo].[log] 
                (bookId, 
                userId, 
                currentPage, 
                noOfPages, 
                ListType)
                VALUES 
                (@bookId, 
                 @userId, 
                 @currentPage, 
                 @noOfPages, 
                 @listType);
                SELECT CAST(SCOPE_IDENTITY() AS INT);"; // Retrieve the inserted ID

            try {
                using var db = _connectionString.GetConnection();
                statusCodeOrId = await db.ExecuteScalarAsync<int>(sql, new {
                    entity.BookId,
                    entity.UserId,
                    entity.CurrentPage,
                    entity.NoOfPages,
                    listType
                });

                if (statusCodeOrId == 0) {
                    _logger?.LogError("No ID returned after insert");
                    statusCodeOrId = 500;
                }
            } catch (Exception ex) {
                _logger?.LogError(ex.Message);
                statusCodeOrId = 500;
            }
            return statusCodeOrId;
        }


        public Task<List<Log>> GetAllLogs(string userId) {
            throw new NotImplementedException();
        }

        public async Task<Log> GetLogById(int logId, string listType) {
            using var db = _connectionString.GetConnection();
            var sql = @"
            SELECT
                Log.logId,
                Log.currentPage,
                Log.noOfPages,
                Log.listType,
                Book.bookId,
                [User].userId AS userId
            FROM Log
            JOIN [User] ON Log.userId = [User].userId
            JOIN Book ON Log.bookId = Book.bookId
            WHERE Log.logId = @logId
                AND Log.listType = @listType";

            var result = await db.QueryAsync<Log, Book, User, Log>(
                sql,
                (log, book, user) => {
                    log.Book = book;
                    log.User = user;
                    return log;
                },
                new { logId, listType },
                splitOn: "bookId, userId"
            );

            var foundLog = result.FirstOrDefault();

            if (foundLog != null) {
                foundLog.Book = await _bookAccess.Get(foundLog.Book.BookId);
            }

            return foundLog;
        }


        Task<List<Log>> ILogAccess.GetLogsByUserId(string userId) {
            throw new NotImplementedException();
        }

        public async Task<int> Update(int logId, Log updatedLog) {
            int rowsAffected = 0;

            string sql = @"
        UPDATE [Log]
        SET
            bookId = @BookId, 
            userId = @UserId, 
            currentPage = @CurrentPage, 
            noOfPages = @NoOfPages, 
            ListType = @ListType
        WHERE
            logId = @LogId";

            using var db = _connectionString.GetConnection();

            try {
                rowsAffected = await db.ExecuteAsync(sql, updatedLog);

                if (rowsAffected > 0) {
                    _logger?.LogInformation($"Log {logId} updated successfully");
                    return 0; // Set returnCode to 0 for success
                } else {
                    _logger?.LogWarning($"Log {logId} was not updated");
                    return -1; // Set returnCode to -1 for not found
                }
            } catch (Exception ex) {
                _logger?.LogError($"Error updating log {logId}: {ex.Message}");
                return -500; // Set returnCode to -500 for other errors
            }
        }

        public async Task<IEnumerable<Log>> GetLogsByUser(Guid userId, string listType)
        {
            var sql = @"
        SELECT
            Log.logId,
            Log.currentPage,
            Log.noOfPages,
            Log.listType,
            Book.bookId,
            [User].userId AS userId
        FROM Log
        JOIN [User] ON Log.userId = [User].userId
        JOIN Book ON Log.bookId = Book.bookId
        WHERE Log.userId = @UserId AND Log.listType = @ListType";

            using var db = _connectionString.GetConnection();

            var result = await db.QueryAsync<Log, Book, User, Log>(
                sql,
                (log, book, user) => {
                    log.Book = book;
                    log.User = user;
                    return log;
                },
                new { UserId = userId, ListType = listType },
                splitOn: "bookId, userId"
            );

            // Hent fulde bogdetaljer for hver log (valgfrit – afhængigt af behov)
            foreach (var log in result)
            {
                log.Book = await _bookAccess.Get(log.Book.BookId);
            }

            return result;
        }

        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(Guid userId, string listType)
        {
            var sql = @"
    SELECT l.logId, l.currentPage, l.noOfPages, l.listType,
           b.bookId, u.userId
    FROM Log l
    INNER JOIN (
        SELECT bookId, MAX(logId) AS MaxLogId
        FROM Log
        WHERE userId = @UserId AND listType = @ListType
        GROUP BY bookId
    ) latest ON l.logId = latest.MaxLogId
    JOIN Book b ON l.bookId = b.bookId
    JOIN [User] u ON l.userId = u.userId";

            using var db = _connectionString.GetConnection();

            var result = await db.QueryAsync<Log, Book, User, Log>(
                sql,
                (log, book, user) =>
                {
                    log.Book = book;
                    log.User = user;
                    return log;
                },
                new { UserId = userId, ListType = listType },
                splitOn: "bookId, userId"
            );

            // Valgfrit – hent fulde book-detaljer hvis nødvendigt
            foreach (var log in result)
            {
                log.Book = await _bookAccess.Get(log.Book.BookId);
            }

            return result;
        }


    }

}
