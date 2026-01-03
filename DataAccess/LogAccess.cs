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
        // GET BY ID
        // -----------------------------
        public async Task<Log> GetLogById(int logId, string listType)
        {
            const string sql = @"
                SELECT *
                FROM Log
                WHERE logId = @logId
                  AND listType = @listType";

            using var db = _connection.GetConnection();

            return await db.QuerySingleOrDefaultAsync<Log>(
                sql,
                new { logId, listType }
            );
        }

        // -----------------------------
        // GET LOGS BY USER + LIST TYPE
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLogsByUser(string userId, string listType)
        {
            const string sql = @"
                SELECT *
                FROM Log
                WHERE userId = @userId
                  AND listType = @listType
                ORDER BY createdAt DESC";

            using var db = _connection.GetConnection();

            return await db.QueryAsync<Log>(
                sql,
                new { userId, listType }
            );
        }

        // -----------------------------
        // GET LATEST LOG PER BOOK
        // -----------------------------
        public async Task<IEnumerable<Log>> GetLatestLogsByUserAndListType(string userId, string listType)
        {
            const string sql = @"
                SELECT *
                FROM Log l
                WHERE l.userId = @userId
                  AND l.listType = @listType
                  AND l.logId IN (
                      SELECT MAX(logId)
                      FROM Log
                      WHERE userId = @userId
                        AND listType = @listType
                      GROUP BY bookId
                  )
                ORDER BY l.createdAt DESC";

            using var db = _connection.GetConnection();

            return await db.QueryAsync<Log>(
                sql,
                new { userId, listType }
            );
        }

        // -----------------------------
        // GET FULL HISTORY (USER)
        // -----------------------------
        public async Task<List<Log>> GetAllLogs(string userId)
        {
            const string sql = @"
                SELECT *
                FROM Log
                WHERE userId = @userId
                ORDER BY createdAt DESC";

            using var db = _connection.GetConnection();

            var logs = await db.QueryAsync<Log>(
                sql,
                new { userId }
            );

            return logs.ToList();
        }
    }
}
