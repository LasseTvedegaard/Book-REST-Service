using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Model;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class GenreAccess : ICRUDAccess<Genre>
    {
        private readonly IConnection _connection;
        private readonly ILogger<GenreAccess>? _logger;

        public GenreAccess(IGenericConnection<LibraryConnection> pgConnection, ILogger<GenreAccess>? logger = null)
        {
            _connection = pgConnection;
            _logger = logger;
        }

        // Til testformål (fx unit tests med mock connection)
        public GenreAccess(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Genre entity)
        {
            const string sql = @"
                INSERT INTO Genre (GenreName)
                VALUES (@GenreName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using var db = _connection.GetConnection();
                return await db.ExecuteScalarAsync<int>(sql, entity);
            } catch (SqlException sqlEx)
            {
                _logger?.LogError(sqlEx, "SQL Server error in Create");
                throw;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in Create");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string sql = @"DELETE FROM Genre WHERE GenreId = @id";

            using var db = _connection.GetConnection();
            int rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }

        public Task<Genre> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Genre>> GetAll()
        {
            const string sql = @"SELECT GenreId, GenreName FROM Genre";

            using var db = _connection.GetConnection();
            try
            {
                var genres = await db.QueryAsync<Genre>(sql);
                return genres.ToList();
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error in GetAll");
                return new List<Genre>();
            }
        }

        public async Task<bool> Update(int id, Genre entity)
        {
            const string sql = @"
                UPDATE Genre
                SET GenreName = @GenreName
                WHERE GenreId = @GenreId";

            using var db = _connection.GetConnection();
            int rowsAffected = await db.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public Task<bool> DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}