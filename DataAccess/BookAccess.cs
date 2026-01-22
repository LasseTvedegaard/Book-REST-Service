using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

namespace DataAccess
{
    public class BookAccess : IBookAccess
    {
        private IConnection _connectionString;
        private readonly ILogger<IBookAccess>? _logger;

        public BookAccess(
            IGenericConnection<LibraryConnection> _pgConnection,
            IConfiguration configuration,
            ILogger<IBookAccess>? logger = null)
        {
            _connectionString = _pgConnection;
            _logger = logger;
        }

        // For Test
        public BookAccess(string connectionStringForTest)
        {
            // Brug evt. mock IConnection til tests
        }

        // -------------------------
        // CREATE
        // -------------------------
        public async Task<int> Create(Book entity)
        {
            var insertedId = -1;
            using var db = _connectionString.GetConnection();

            using var transaction = db.BeginTransaction();
            try
            {
                var sqlBook = @"
                    INSERT INTO Book
                    (
                        GenreId,
                        LocationId,
                        IsbnNo,
                        Title,
                        Author,
                        NoOfPages,
                        BookType,
                        ImageUrl,
                        Status,
                        UserId
                    )
                    VALUES
                    (
                        @GenreId,
                        @LocationId,
                        @IsbnNo,
                        @Title,
                        @Author,
                        @NoOfPages,
                        @BookType,
                        @ImageUrl,
                        @Status,
                        @UserId
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                insertedId = await db.ExecuteScalarAsync<int>(
                    sqlBook,
                    new
                    {
                        GenreId = entity.Genre?.GenreId,
                        LocationId = entity.Location?.LocationId,
                        entity.IsbnNo,
                        entity.Title,
                        entity.Author,
                        entity.NoOfPages,
                        entity.BookType,
                        ImageUrl = entity.ImageURL,
                        entity.Status,
                        UserId = entity.UserId          // 🔒 Ejer sættes her
                    },
                    transaction
                );

                transaction.Commit();
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating book");
                transaction.Rollback();
                throw;
            }

            return insertedId;
        }

        // -------------------------
        // GET SINGLE (USER-SCOPED)
        // -------------------------
        public async Task<Book?> Get(int id, string userId)
        {
            using var db = _connectionString.GetConnection();

            var bookQuery = @"
                SELECT
                    b.BookId, 
                    b.IsbnNo,
                    b.Title,
                    b.Author,
                    b.NoOfPages,
                    b.BookType,
                    b.ImageUrl,
                    b.Status,              
                    b.GenreId,
                    g.GenreName,
                    b.LocationId,               
                    l.LocationName
                FROM Book b
                INNER JOIN Genre g ON b.GenreId = g.GenreId
                LEFT JOIN Location l ON b.LocationId = l.LocationId
                WHERE b.BookId = @bookId
                  AND b.UserId = @userId";

            var result = await db.QueryAsync<Book, Genre, Location, Book>(
                bookQuery,
                (book, genre, location) =>
                {
                    book.Genre = genre;
                    book.Location = location;
                    return book;
                },
                new { bookId = id, userId },   // 🔹 VIGTIG: begge parametre
                splitOn: "GenreName,LocationName"
            );

            return result.FirstOrDefault();
        }

        // -------------------------
        // GET ALL FOR USER
        // -------------------------
        public async Task<List<Book>> GetAll(string userId)
        {
            using var db = _connectionString.GetConnection();
            using var transaction = db.BeginTransaction();

            var bookQuery = @"
                SELECT
                    b.BookId, 
                    b.IsbnNo,
                    b.Title,
                    b.Author,
                    b.NoOfPages,
                    b.BookType,
                    b.ImageUrl,
                    b.Status,              
                    b.GenreId,
                    g.GenreName,
                    b.LocationId,               
                    l.LocationName
                FROM Book b
                INNER JOIN Genre g ON b.GenreId = g.GenreId
                LEFT JOIN Location l ON b.LocationId = l.LocationId 
                WHERE b.UserId = @userId";

            var books = (await db.QueryAsync<Book, Genre, Location, Book>(
                bookQuery,
                (book, genre, location) =>
                {
                    book.Genre = genre;
                    book.Location = location;
                    return book;
                },
                new { userId },              // 🔹 VIGTIG
                transaction: transaction,
                splitOn: "GenreName,LocationName"
            )).ToList();

            transaction.Commit();
            return books;
        }

        // -------------------------
        // UPDATE (USER-SCOPED)
        // -------------------------
        public async Task<bool> Update(int id, Book entity, string userId)
        {
            entity.BookId = id;

            using var db = _connectionString.GetConnection();

            var updateSql = @"
        UPDATE Book 
        SET 
            IsbnNo = @IsbnNo, 
            Title = @Title, 
            Author = @Author,
            NoOfPages = @NoOfPages,
            BookType = @BookType,
            ImageUrl = @ImageUrl,
            Status = @Status, 
            GenreId = @GenreId,
            LocationId = @LocationId
        WHERE 
            BookId = @BookId
        AND UserId = @UserId";   // 🔒 Kun ejer må opdatere

            try
            {
                var rowsAffected = await db.ExecuteAsync(
                    updateSql,
                    new
                    {
                        entity.IsbnNo,
                        entity.Title,
                        entity.Author,
                        entity.NoOfPages,
                        entity.BookType,
                        ImageUrl = entity.ImageURL,
                        entity.Status,
                        GenreId = (entity.Genre?.GenreId > 0) ? (int?)entity.Genre.GenreId : null,
                        LocationId = (entity.Location?.LocationId > 0) ? (int?)entity.Location.LocationId : null,
                        BookId = entity.BookId,
                        UserId = userId              // 🔹 BRUG PARAMETER, ikke entity.UserId
                    }
                );

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation("Book with ID {BookId} updated successfully", entity.BookId);
                } else
                {
                    _logger?.LogWarning("No rows affected when updating book with ID {BookId}", entity.BookId);
                }

                return rowsAffected > 0;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating book with ID {BookId}", entity.BookId);
                return false;
            }
        }


        // -------------------------
        // GET BY STATUS (USER-SCOPED)
        // -------------------------
        public async Task<List<Book>> GetByStatus(string status, string userId)
        {
            using var db = _connectionString.GetConnection();
            using var transaction = db.BeginTransaction();

            var sql = @"
                SELECT
                    b.BookId, 
                    b.IsbnNo,
                    b.Title,
                    b.Author,
                    b.NoOfPages,
                    b.BookType,
                    b.ImageUrl,
                    b.Status,              
                    b.GenreId,
                    g.GenreName,
                    b.LocationId,               
                    l.LocationName
                FROM Book b
                INNER JOIN Genre g ON b.GenreId = g.GenreId
                LEFT JOIN Location l ON b.LocationId = l.LocationId
                WHERE LOWER(b.Status) = LOWER(@status)
                  AND b.UserId = @userId";

            var books = (await db.QueryAsync<Book, Genre, Location, Book>(
                sql,
                (book, genre, location) =>
                {
                    book.Genre = genre;
                    book.Location = location;
                    return book;
                },
                new { status, userId },     // 🔹 VIGTIG
                transaction: transaction,
                splitOn: "GenreName,LocationName"
            )).ToList();

            transaction.Commit();
            return books;
        }

        // -------------------------
        // UPDATE STATUS (USER-SCOPED)
        // -------------------------
        public async Task<bool> UpdateStatus(int bookId, string status, string userId)
        {
            using var db = _connectionString.GetConnection();

            var sql = @"
                UPDATE Book 
                SET Status = @status 
                WHERE BookId = @bookId
                  AND UserId = @userId";   // 🔒 Kun ejer må opdatere

            var rowsAffected = await db.ExecuteAsync(
                sql,
                new { status, bookId, userId }   // 🔹 VIGTIG
            );

            return rowsAffected > 0;
        }
    }
}
