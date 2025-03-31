using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using DataAccess.Context;

namespace DataAccess
{
    public class ReadingNoteAccess : IReadingNoteAccess
    {
        private readonly IConnection _connectionString;

        public ReadingNoteAccess(IGenericConnection<LibraryConnection> mssqlConnection)
        {
            _connectionString = mssqlConnection;
        }

        public async Task<ReadingNote> GetNoteAsync(int bookId, string userId)
        {
            using var connection = _connectionString.GetConnection();
            var sql = "SELECT * FROM ReadingNote WHERE BookId = @bookId AND UserId = @userId";
            return await connection.QueryFirstOrDefaultAsync<ReadingNote>(sql, new { bookId, userId });
        }

        public async Task UpsertNoteAsync(ReadingNote note)
        {
            using var connection = _connectionString.GetConnection();

            var existingNote = await GetNoteAsync(note.BookId, note.UserId);
            if (existingNote == null)
            {
                var insertSql = @"
                    INSERT INTO ReadingNote (BookId, UserId, Note, LastUpdated) 
                    VALUES (@BookId, @UserId, @Note, GETDATE())";
                await connection.ExecuteAsync(insertSql, note);
            } else
            {
                var updateSql = @"
                    UPDATE ReadingNote 
                    SET Note = @Note, LastUpdated = GETDATE() 
                    WHERE BookId = @BookId AND UserId = @UserId";
                await connection.ExecuteAsync(updateSql, note);
            }
        }

        public async Task<IEnumerable<ReadingNote>> GetAllNotesForBookAsync(int bookId, string userId)
        {
            using var connection = _connectionString.GetConnection();
            var sql = "SELECT * FROM ReadingNote WHERE BookId = @bookId AND UserId = @userId ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<ReadingNote>(sql, new { bookId, userId });
        }

    }
}
