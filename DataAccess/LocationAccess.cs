using Dapper;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;

namespace DataAccess {
    public class LocationAccess : ICRUDAccess<Location> {

        private readonly string? _connectionString;
        private readonly ILogger<ICRUDAccess<Location>>? _logger;

        public LocationAccess(IConfiguration configuration, ILogger<ICRUDAccess<Location>> logger = null) {
            _connectionString = configuration.GetConnectionString("DbAccessConnection");
            _logger = logger;
        }

        // Test
        public LocationAccess(string connectionForTest) {
            _connectionString = connectionForTest;
        }
        public async Task<int> Create(Location entity) {
            int insertedLocationId = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"INSERT INTO location
                            (locationName)
                            OUTPUT INSERTED.locationId
                            VALUES
                            (@locationName) ";
                try {
                    insertedLocationId = await con.ExecuteScalarAsync<int>(sql, entity);
                } catch (Exception ex) {
                    _logger?.LogError(ex.Message, ex);
                    throw;
                }
            }
            return insertedLocationId;
        }

        public async Task<bool> Delete(int id) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection( _connectionString)) {
                con.Open();
                var sql = @"DELETE FROM location
                            WHERE
                            locationId = @id";
                rowsAffected = await con.ExecuteAsync(sql, new { id });
            }
            return rowsAffected > 0;
        }

        public Task<Location> Get(int id) {
            throw new NotImplementedException();
        }

        public async Task<List<Location>> GetAll() {
            List<Location>? foundLocation;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"SELECT locationId, locationName
                            FROM Location";
                try {
                    foundLocation = (await con.QueryAsync<Location>(sql)).ToList();
                } catch (Exception ex) {
                    foundLocation = null;
                    _logger?.LogError(ex, ex.Message);
                }
            }
            return foundLocation;
        }


        public async Task<bool> Update(int id, Location entity) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"UPDATE location
                            SET
                                locationName = @locationName
                            WHERE
                                locationId = @locationId";
                rowsAffected = await con.ExecuteAsync(sql, new { locationName = entity.LocationName, locationId = id });
            }
            return rowsAffected > 0;
        }

        public Task<bool> DeleteAll() {
            throw new NotImplementedException();
        }
    }
}
