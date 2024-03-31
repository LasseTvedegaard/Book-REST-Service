﻿using Dapper;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;

// This class uses the Dapper Object-Relational Mapping (ORM) library to handle database operations for the Book entity. Dapper
// simplifies the process of mapping database records to C# objects by allowing us to write native SQL queries and 
// automatically mapping the result set to our defined models. Create, Read, Update, and fetching all records are implemented
// using Dapper's methods like ExecuteScalarAsync and QueryAsync.

namespace DataAccess {
    public class BookAccess : IBookAccess {

        private readonly string? _connectionString;
        private readonly ILogger<IBookAccess>? _logger;

        public BookAccess(IConfiguration configuration, ILogger<IBookAccess>? logger = null) {
            _connectionString = configuration.GetConnectionString("DbAccessConnection");
            _logger = logger;
        }

        // For Test
        public BookAccess(string connectionStringForTest) {
            _connectionString = connectionStringForTest;
        }
        public async Task<int> Create(Book entity) {
            var insertedId = -1;
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
                using (var transaction = conn.BeginTransaction()) {
                    try {
                        // Insert to Book table
                        var sqlBook = @"INSERT INTO Book
                                (genreId,
                                locationId,
                                isbnNo,
                                title,
                                author,
                                noOfPages,
                                bookType,
                                imageURL,
                                status)
                            OUTPUT INSERTED.BookId
                            VALUES
                                (@genreId,
                                 @locationId,
                                 @isbnNo,
                                 @title,
                                 @author,
                                 @noOfPages,
                                 @bookType,
                                 @imageURL,
                                 @status)";


                        insertedId = await conn.ExecuteScalarAsync<int>(sqlBook, new {
                            entity.Genre.GenreId,
                            entity.Location.LocationId,
                            entity.IsbnNo,
                            entity.Title,
                            entity.Author,
                            entity.NoOfPages,
                            entity.BookType,
                            entity.ImageURL,
                            entity.Status
                        }, transaction);

                        transaction.Commit();
                    } catch (Exception ex) {
                        _logger?.LogError(ex.Message);
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return insertedId;
        }


        public async Task<Book>? Get(int id) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                await conn.OpenAsync();

                var bookQuery = @"SELECT
                b.bookId, 
                b.isbnNo,
                b.title,
                b.author,
                b.noOfPages,
                b.bookType,
                b.imageURL,
                b.status,              
                b.genreId,
                g.genreName,
                b.locationId,               
                l.locationName
            FROM Book b
            INNER JOIN Genre g ON b.genreId = g.genreId
            LEFT JOIN Location l ON b.locationId = l.locationId
            WHERE b.BookId = @bookId";

                var result = await conn.QueryAsync<Book, Genre, Location, Book>(
                    bookQuery,
                    (book, genre, location) => {
                        book.Genre = genre;
                        book.Location = location;
                        return book;
                    },
                    new { bookId = id },
                    splitOn: "Genreid, LocationName"  // Splitting columns for mapping
                );

                return result.FirstOrDefault();  // Return the first (or default) book from the result
            }
        }


        // This method queries and returns all books from the 'books' database table.
        public async Task<List<Book>> GetAll() {

            // Initialize a new sqlConnection object to connect to the database
            using (SqlConnection conn = new SqlConnection(_connectionString)) {

                // Open an asynchronous connection to the database.
                await conn.OpenAsync();

                // Start a transaction in order to comply the ACID principles
                using (var transaction = conn.BeginTransaction()) {

                    // Initialize a list to store the retrieved books.
                    var books = new List<Book>();

                    // SQL query to join 'Book' with 'Genre' and 'Location' tables.
                    string bookQuery = @"SELECT
                b.bookId, 
                b.isbnNo,
                b.title,
                b.author,
                b.noOfPages,
                b.bookType,
                b.imageURL,
                b.status,              
                b.genreId,
                g.genreName,
                b.locationId,               
                l.locationName
                FROM Book b
                INNER JOIN Genre g ON b.genreId = g.genreId
                LEFT JOIN Location l ON b.locationId = l.locationId";

                    // Execute the SQL query and map the results to the 'Book' list.

                    books = (await conn.QueryAsync<Book, Genre, Location, Book>(bookQuery,
                        (book, genre, location) => {
                            book.Genre = genre;
                            book.Location = location;
                            return book;
                        },

                        transaction: transaction,

                        // The 'splitOn' parameter specifies where to split the columns in the result set when mapping to C# objects.
                        // In this case, columns before 'GenreId' map to 'Book', columns between 'GenreId' and 'LocationId' map to 'Genre',
                        // and columns from 'LocationId' onwards map to 'Location'.
                        splitOn: "GenreId,LocationId"))
                    .ToList();

                    // Commit the transaction.
                    transaction.Commit();

                    // Return list of books
                    return books;
                }
            }
        }

        public async Task<bool> Update(int id, Book entity) {
            int rowsAffected = -1;
            entity.BookId = id;

            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
                var updateSql = @"
                    UPDATE Book 
                    SET 
                        isbnNo = @isbnNo, 
                        title = @title, 
						author = @author,
						noOfPages = @noOfPages,
						bookType = @bookType,
                        imageURL = @imageURL,
                        status = @status, 
						genreId = @genreId,
                        locationId = @locationId 
                       
                    WHERE 
                        bookId = @bookId";
                try {
                    rowsAffected = await conn.ExecuteAsync(updateSql, new {
                        isbnNo = entity.IsbnNo,
                        title = entity.Title,
                        author = entity.Author,
                        noOfPages = entity.NoOfPages,
                        bookType = entity.BookType,
                        imageURL = entity.ImageURL,
                        status = entity.Status,
                        genreId = entity.Genre?.GenreId ?? 0,
                        locationId = entity.Location?.LocationId ?? 0,
                        bookId = entity.BookId,
                    });


                } catch (Exception ex) {
                    _logger?.LogError(ex.Message);
                }
            }
            return rowsAffected > 0;
        }
    }
}
