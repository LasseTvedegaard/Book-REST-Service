using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;
using static Dapper.SqlMapper;

namespace DataAccess {
    public class UserAccess : IUserAccess {

        private IConnection _connectionString;
        private readonly ILogger<UserAccess>? _logger;

        public UserAccess(IGenericConnection<LibraryConnection> _mssqlConnection, IConfiguration configuration, ILogger<UserAccess>? logger = null) {
            _connectionString = _mssqlConnection;
            _logger = logger;
        }

        public UserAccess(string connectionString) {
            //_connectionString = connectionString;
        }

        public async Task<bool> Create(User entity) {
            bool userInserted = false;
            using var db = _connectionString.GetConnection();
            var sql = @"
                            INSERT INTO [User] (
                                userId,
                                firstName,
                                lastName,
                                birthdate,
                                phone,
                                email,
                                address,
                                zipcode,
                                city
                            )
                            VALUES (
                                @userId,
                                @firstName,
                                @lastName,
                                @birthdate,
                                @phone,
                                @email,
                                @address,
                                @zipcode,
                                @city
                            )";

            try {
                var rowsAffected = await db.ExecuteAsync(sql, entity);
                if (rowsAffected > 0) {
                    userInserted = true;
                    _logger?.LogInformation($"A user is created with userId {entity.UserId}");

                }
            } catch (Exception ex) {
                userInserted = false;
                _logger?.LogError(ex.Message + "// Create user failed");
            }
            return userInserted;
        }

        public async Task<User> Get(string id) {
            User? foundUser = null;

            string sql = @"SELECT
                            userid,
                            firstname,
                            lastname,
                            birthdate,
                            phone,
                            email,
                            address,
                            zipcode,
                            city
                        FROM
                            [User]
                        WHERE
                            userId = @userId";

            using var db = _connectionString.GetConnection();

            try {
                var userEnumerable = await db.QueryAsync<User>(sql, new { userId = id });
                foundUser = userEnumerable.FirstOrDefault();
                if (foundUser != null) {
                    _logger?.LogInformation($"A user was found with userId {foundUser.UserId}");
                }
            } catch (Exception ex) {
                _logger?.LogError(ex.Message + "// Get a user failed");
            }
            return foundUser;
        }

        public async Task<bool> Update(User entity) {
            int rowsAffected = -1;

            string sql = @"UPDATE [User]
                           SET
                            firstName = @firstName,
                            lastName = @lastName,
                            birthdate = @birthdate,
                            phone = @phone,
                            email = @email,
                            address = @address,
                            zipcode = @zipcode,
                            city = @city
                        WHERE   
                            userId = @userId";

            try {
                using var db = _connectionString.GetConnection();

                rowsAffected = await db.ExecuteAsync(sql, entity);


                if (rowsAffected > 0) {
                    _logger?.LogInformation($"User {entity.UserId} was updated");

                } else {
                    _logger?.LogWarning($"User {entity.UserId} was not updated");
                }
            } catch (Exception ex) {
                _logger?.LogError($"There was an error updating {entity.UserId} the message was {ex.Message}");
                throw;
            }

            return rowsAffected > 0;
        }
    }
}
