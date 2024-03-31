using Dapper;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;


namespace DataAccess {
    public class GenreAccess : ICRUDAccess<Genre> {

        private readonly string? _connectionString;
        private readonly ILogger<ICRUDAccess<Genre>>? _logger;

        public GenreAccess(IConfiguration configuration, ILogger<ICRUDAccess<Genre>> logger = null) {
            _connectionString = configuration.GetConnectionString("DbAccessConnection");
            _logger = logger;
        }

        // Test
        public GenreAccess(string connectionStringForTest) {
            _connectionString = connectionStringForTest;
        }

        public async Task<int> Create(Genre entity) {
            int insertedGenreId = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"INSERT INTO Genre
                            (genreName)
                            OUTPUT INSERTED.genreId
                            VALUES
                            (@genreName)";
                try {
                    insertedGenreId = await con.ExecuteScalarAsync<int>(sql, entity);
                } catch (Exception ex) {
                    _logger?.LogError(ex.Message, ex);
                    throw;
                }
            }
            return insertedGenreId;
        }

        public async Task<bool> Delete(int id) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"DELETE FROM genre
                            WHERE
                            genreId = @id";
                rowsAffected = await con.ExecuteAsync(sql, new { id });
            }
            return rowsAffected > 0;
        }

        public Task<Genre> Get(int id) {
            throw new NotImplementedException();
        }

        public async Task<List<Genre>> GetAll() {
            List<Genre>? foundGenre;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"SELECT genreId, genreName
                            FROM Genre";
                try {
                    foundGenre = (await con.QueryAsync<Genre>(sql)).ToList();
                } catch (Exception ex) {
                    foundGenre = null;
                    _logger?.LogError(ex, ex.Message);
                }
            }
            return foundGenre;
        }

        public async Task<bool> Update(int id, Genre entity) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"UPDATE genre
                            SET
                                genreName = @genreName
                            WHERE
                                genreId = @genreId";
                rowsAffected = await con.ExecuteAsync(sql, entity);
            }
            return rowsAffected > 0;
        }
    }
}
