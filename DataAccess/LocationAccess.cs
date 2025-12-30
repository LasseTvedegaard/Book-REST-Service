using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class LocationAccess : ICRUDAccess<Location>
    {
        private readonly IConnection _connection;
        private readonly ILogger<LocationAccess>? _logger;

        public LocationAccess(IGenericConnection<LibraryConnection> pgConnection, IConfiguration configuration, ILogger<LocationAccess> logger = null)
        {
            _connection = pgConnection;
            _logger = logger;
        }

        // For testing
        public LocationAccess(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> Create(Location entity)
        {
            const string sql = @"
                INSERT INTO Location (LocationName)
                VALUES (@LocationName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using var db = _connection.GetConnection();
                return await db.ExecuteScalarAsync<int>(sql, entity);
            } catch (SqlException sqlEx)
            {
                _logger?.LogError(sqlEx, "SQL Server error in Create(Location)");
                throw;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Unhandled error in Create(Location)");
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string sql = @"DELETE FROM Location WHERE LocationId = @id";

            using var db = _connection.GetConnection();
            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }

        public Task<Location> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Location>> GetAll()
        {
            const string sql = @"SELECT LocationId, LocationName FROM Location";

            try
            {
                using var db = _connection.GetConnection();
                var result = await db.QueryAsync<Location>(sql);
                return result.ToList();
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetAll(Location)");
                return new List<Location>();
            }
        }

        public async Task<bool> Update(int id, Location entity)
        {
            const string sql = @"
                UPDATE Location
                SET LocationName = @LocationName
                WHERE LocationId = @LocationId";

            using var db = _connection.GetConnection();
            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                entity.LocationName,
                LocationId = id
            });

            return rowsAffected > 0;
        }

        public Task<bool> DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}