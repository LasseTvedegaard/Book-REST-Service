using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using Npgsql;

namespace DataAccess
{
    public class UserAccess : IUserAccess
    {
        private readonly IConnection _connectionString;
        private readonly ILogger<UserAccess>? _logger;

        public UserAccess(IGenericConnection<LibraryConnection> pgConnection, IConfiguration configuration, ILogger<UserAccess>? logger = null)
        {
            _connectionString = pgConnection;
            _logger = logger;
        }

        public async Task<bool> Create(User entity)
        {
            using var db = _connectionString.GetConnection();
            entity.UserId ??= Guid.NewGuid().ToString();

            const string sql = @"
                INSERT INTO ""user"" (
                    userid,
                    firstname,
                    lastname,
                    email
                )
                VALUES (
                    @UserId,
                    @FirstName,
                    @LastName,
                    @Email
                )";

            try
            {
                if (string.IsNullOrWhiteSpace(entity.Email))
                {
                    _logger?.LogWarning("Attempted to create user without email");
                    return false;
                }

                // Tjek om email allerede findes
                var existing = await GetByEmail(entity.Email);
                if (existing != null)
                {
                    _logger?.LogWarning("User with email {Email} already exists", entity.Email);
                    return false;
                }

                Console.WriteLine($"📤 Inserting user: {entity.FirstName} {entity.LastName}, {entity.Email}");
                int rowsAffected = await db.ExecuteAsync(sql, entity);

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation("✅ User created. UserId: {UserId}, Email: {Email}", entity.UserId, entity.Email);
                    return true;
                } else
                {
                    _logger?.LogWarning("⚠️ No rows affected for email: {Email}", entity.Email);
                    return false;
                }
            } catch (PostgresException pgEx)
            {
                _logger?.LogError(pgEx, "🐘 PostgreSQL error in {Method}: {Email}", nameof(Create), entity.Email);
                return false;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "🔥 Unhandled error in {Method} for user {@User}", nameof(Create), entity);
                return false;
            }
        }

        public async Task<User?> Get(string id)
        {
            const string sql = @"
                SELECT
                    userid,
                    firstname,
                    lastname,
                    email
                FROM
                    ""user""
                WHERE
                    userid = @id";

            using var db = _connectionString.GetConnection();

            try
            {
                var user = await db.QueryFirstOrDefaultAsync<User>(sql, new { id });

                if (user != null)
                {
                    _logger?.LogInformation("🔎 Found user with ID: {UserId}", user.UserId);
                }

                return user;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error in {Method} while fetching user {Id}", nameof(Get), id);
                return null;
            }
        }

        public async Task<bool> Update(User entity)
        {
            const string sql = @"
                UPDATE ""user""
                SET
                    firstname = @FirstName,
                    lastname = @LastName,
                    email = @Email
                WHERE
                    userid = @UserId";

            try
            {
                using var db = _connectionString.GetConnection();
                int rowsAffected = await db.ExecuteAsync(sql, entity);

                if (rowsAffected > 0)
                {
                    _logger?.LogInformation("✅ User {UserId} updated", entity.UserId);
                    return true;
                } else
                {
                    _logger?.LogWarning("⚠️ No update occurred for user: {UserId}", entity.UserId);
                    return false;
                }
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "🔥 Error in {Method} for user: {UserId}", nameof(Update), entity.UserId);
                throw;
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            const string sql = @"
                SELECT
                    userid,
                    firstname,
                    lastname,
                    email
                FROM
                    ""user""
                WHERE
                    email = @email";

            using var db = _connectionString.GetConnection();

            try
            {
                return await db.QueryFirstOrDefaultAsync<User>(sql, new { email });
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error in {Method} while fetching user with email: {Email}", nameof(GetByEmail), email);
                return null;
            }
        }
    }
}
