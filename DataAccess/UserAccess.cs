using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;

namespace DataAccess
{
    public class UserAccess : IUserAccess
    {

        private IConnection _connectionString;
        private readonly ILogger<UserAccess>? _logger;

        public UserAccess(IGenericConnection<LibraryConnection> _mssqlConnection, IConfiguration configuration, ILogger<UserAccess>? logger = null)
        {
            _connectionString = _mssqlConnection;
            _logger = logger;
        }
        public async Task<bool> Create(User entity)
        {
            using var db = _connectionString.GetConnection();

            // Giv en ny GUID hvis UserId mangler
            entity.UserId ??= Guid.NewGuid().ToString();

            const string sql = @"
        INSERT INTO [User] (
            userId,
            firstName,
            lastName,
            email
        )
        VALUES (
            @userId,
            @firstName,
            @lastName,
            @email
        )";

            try
            {
                int rowsAffected = await db.ExecuteAsync(sql, new
                {
                    userId = entity.UserId,
                    firstName = entity.FirstName,
                    lastName = entity.LastName,
                    email = entity.Email
                });

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation("User successfully created. UserId: {UserId}, Email: {Email}", entity.UserId, entity.Email);
                    return true;
                } else
                {
                    _logger?.LogWarning("User was not created. No rows affected. Email: {Email}", entity.Email);
                    return false;
                }
            } catch (SqlException sqlEx)
            {
                _logger?.LogError(sqlEx, "SQL Error while creating user with Email: {Email}", entity.Email);
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error while creating user with Email: {Email}", entity.Email);
            }

            return false;
        }



        public async Task<User> Get(string id)
        {
            User? foundUser = null;

            string sql = @"SELECT
                            userId,
                            firstName,
                            lastName,
                            email
                        FROM
                            [User]
                        WHERE
                            userId = @userId";

            using var db = _connectionString.GetConnection();

            try
            {
                var userEnumerable = await db.QueryAsync<User>(sql, new { userId = id });
                foundUser = userEnumerable.FirstOrDefault();
                if (foundUser != null)
                {
                    _logger?.LogInformation($"A user was found with userId {foundUser.UserId}");
                }
            } catch (Exception ex)
            {
                _logger?.LogError(ex.Message + "// Get a user failed");
            }
            return foundUser;
        }

        public async Task<bool> Update(User entity)
        {
            int rowsAffected = -1;

            string sql = @"UPDATE [User]
                           SET
                            firstName = @firstName,
                            lastName = @lastName,
                            email = @email
                        WHERE   
                            userId = @userId";

            try
            {
                using var db = _connectionString.GetConnection();
                rowsAffected = await db.ExecuteAsync(sql, entity);

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation($"User {entity.UserId} was updated");
                } else
                {
                    _logger?.LogWarning($"User {entity.UserId} was not updated");
                }
            } catch (Exception ex)
            {
                _logger?.LogError($"There was an error updating {entity.UserId} the message was {ex.Message}");
                throw;
            }

            return rowsAffected > 0;
        }

        public async Task<User?> GetByEmail(string email)
        {
            using var db = _connectionString.GetConnection();

            string sql = @"SELECT
                            userId,
                            firstName,
                            lastName,
                            email
                        FROM
                            [User]
                        WHERE
                            email = @email";

            try
            {
                var result = await db.QueryAsync<User>(sql, new { email });
                return result.FirstOrDefault();
            } catch (Exception ex)
            {
                _logger?.LogError(ex.Message + "// GetByEmail failed");
                return null;
            }
        }
    }
}