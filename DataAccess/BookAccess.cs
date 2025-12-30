using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Net;

namespace DataAccess
{
    public class BookAccess : IBookAccess
    {
        private IConnection _connectionString;
        private readonly ILogger<IBookAccess>? _logger;

        public BookAccess(IGenericConnection<LibraryConnection> _pgConnection, IConfiguration configuration, ILogger<IBookAccess>? logger = null)
        {
            _connectionString = _pgConnection;
            _logger = logger;
        }

        // For Test
        public BookAccess(string connectionStringForTest)
        {
            // Brug evt. mock IConnection til tests
        }

        public async Task<int> Create(Book entity)
        {
            var insertedId = -1;
            using var db = _connectionString.GetConnection();

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    // Insert to Book table
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
                            Status)
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
                            @Status);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    insertedId = await db.ExecuteScalarAsync<int>(sqlBook, new
                    {
                        GenreId = entity.Genre.GenreId,
                        LocationId = entity.Location.LocationId,
                        entity.IsbnNo,
                        entity.Title,
                        entity.Author,
                        entity.NoOfPages,
                        entity.BookType,
                        ImageUrl = entity.ImageURL,
                        entity.Status
                    }, transaction);

                    transaction.Commit();
                } catch (Exception ex)
                {
                    _logger?.LogError(ex.Message);
                    transaction.Rollback();
                    throw;
                }
            }

            return insertedId;
        }

        public async Task<Book>? Get(int id)
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
                WHERE b.BookId = @bookId";

            var result = await db.QueryAsync<Book, Genre, Location, Book>(
                bookQuery,
                (book, genre, location) => {
                    book.Genre = genre;
                    book.Location = location;
                    return book;
                },
                new { bookId = id },
                splitOn: "GenreName,LocationName"
            );

            return result.FirstOrDefault();
        }

        // This method queries and returns all books from the 'Book' database table.
        public async Task<List<Book>> GetAll()
        {
            using var db = _connectionString.GetConnection();

            using (var transaction = db.BeginTransaction())
            {
                var books = new List<Book>();

                string bookQuery = @"
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
                    LEFT JOIN Location l ON b.LocationId = l.LocationId";

                books = (await db.QueryAsync<Book, Genre, Location, Book>(
                    bookQuery,
                    (book, genre, location) => {
                        book.Genre = genre;
                        book.Location = location;
                        return book;
                    },
                    transaction: transaction,
                    splitOn: "GenreName,LocationName"
                )).ToList();

                transaction.Commit();
                return books;
            }
        }

        public async Task<bool> Update(int id, Book entity)
        {
            int rowsAffected = -1;
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
                    BookId = @BookId";

            try
            {
                rowsAffected = await db.ExecuteAsync(updateSql, new
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
                    BookId = entity.BookId
                });

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation("Book with ID {BookId} updated successfully", entity.BookId);
                } else
                {
                    _logger?.LogWarning("No rows affected when updating book with ID {BookId}", entity.BookId);
                }
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating book with ID {BookId}", entity.BookId);
                rowsAffected = 0;
            }

            return rowsAffected > 0;
        }

        public async Task<List<Book>> GetByStatus(string status)
        {
            using var db = _connectionString.GetConnection();
            using var transaction = db.BeginTransaction();

            string sql = @"
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
                WHERE LOWER(b.Status) = LOWER(@status)";

            var books = (await db.QueryAsync<Book, Genre, Location, Book>(
                sql,
                (book, genre, location) => {
                    book.Genre = genre;
                    book.Location = location;
                    return book;
                },
                new { status },
                transaction: transaction,
                splitOn: "GenreName,LocationName"
            )).ToList();

            transaction.Commit();
            return books;
        }

        public async Task<bool> UpdateStatus(int bookId, string status)
        {
            using var db = _connectionString.GetConnection();

            string sql = @"UPDATE Book SET Status = @status WHERE BookId = @bookId";

            var rowsAffected = await db.ExecuteAsync(sql, new { status, bookId });

            return rowsAffected > 0;
        }
    }
}