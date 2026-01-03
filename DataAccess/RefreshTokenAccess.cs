using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

public class RefreshTokenAccess
{
    private readonly IDbConnection _db;

    public RefreshTokenAccess(IDbConnection db)
    {
        _db = db;
    }

    public Task InsertAsync(Guid id, Guid userId, string tokenHash, DateTime expiresAt)
    {
        const string sql = @"
            INSERT INTO dbo.RefreshToken (Id, UserId, TokenHash, ExpiresAt)
            VALUES (@Id, @UserId, @TokenHash, @ExpiresAt)";

        return _db.ExecuteAsync(sql, new
        {
            Id = id,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt
        });
    }

    public Task<Guid?> GetUserIdByValidTokenAsync(string tokenHash)
    {
        const string sql = @"
            SELECT UserId
            FROM dbo.RefreshToken
            WHERE TokenHash = @TokenHash
              AND RevokedAt IS NULL
              AND ExpiresAt > SYSUTCDATETIME()";

        return _db.ExecuteScalarAsync<Guid?>(sql, new { TokenHash = tokenHash });
    }

    public Task RevokeAsync(string tokenHash)
    {
        const string sql = @"
            UPDATE dbo.RefreshToken
            SET RevokedAt = SYSUTCDATETIME()
            WHERE TokenHash = @TokenHash";

        return _db.ExecuteAsync(sql, new { TokenHash = tokenHash });
    }
}
