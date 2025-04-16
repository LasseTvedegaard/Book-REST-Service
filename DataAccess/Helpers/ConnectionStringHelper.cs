using Microsoft.Extensions.Configuration;
using System;

namespace DataAccess.Helpers
{
    public static class ConnectionStringHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            // Prøv at læse fra environment variables
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");

            if (!string.IsNullOrWhiteSpace(dbHost))
            {
                var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
                var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "bookbuddy";
                var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "bookbuddy";
                var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "secret";

                var envConnection = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Pooling=true;";

                Console.WriteLine($"✅ [ConnectionStringHelper] ENV-based connection string resolved: {envConnection}");
                return envConnection;
            }

            // Fallback til appsettings.json > ConnectionStrings > DbAccessConnection
            var fallback = configuration.GetConnectionString("DbAccessConnection");

            if (!string.IsNullOrWhiteSpace(fallback))
            {
                Console.WriteLine($"⚠️ [ConnectionStringHelper] Fallback connection string from appsettings.json: {fallback}");
                return fallback;
            }

            // Sidste udvej: Fejl
            throw new InvalidOperationException("❌ No valid database connection string could be resolved.");
        }
    }
}
