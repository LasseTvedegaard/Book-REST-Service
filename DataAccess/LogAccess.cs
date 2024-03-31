using Dapper;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess {
    public class LogAccess : ILogAccess {

        private readonly string? _connectionString;
        private readonly ILogger<LogAccess> _logger;
        private IBookAccess _bookAccess;
        private IUserAccess _userAccess;

        public LogAccess(IConfiguration configuration, IBookAccess bookAccess, IUserAccess userAccess, ILogger<LogAccess> logger = null) {
            _connectionString = configuration.GetConnectionString("DbAccessConnection");
            _logger = logger;
            _bookAccess = bookAccess;
            _userAccess = userAccess;
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
                using (IDbConnection conn = new SqlConnection(_connectionString)) {
                    conn.Open();
                    statusCodeOrId = await conn.ExecuteScalarAsync<int>(sql, new {
                        entity.BookId,
                        entity.UserId,
                        entity.CurrentPage,
                        entity.NoOfPages,
                        listType
                    });
                }

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
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
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

                var result = await conn.QueryAsync<Log, Book, User, Log>(
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

            using (SqlConnection con = new SqlConnection(_connectionString)) {
                await con.OpenAsync();

                try {
                    rowsAffected = await con.ExecuteAsync(sql, updatedLog);

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

        }

    }
}
