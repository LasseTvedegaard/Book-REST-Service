using Microsoft.Extensions.Configuration;
using System;

namespace DataAccess.Helpers
{
    public static class ConnectionStringHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("DbAccessConnection");

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("No database connection string found.");

            Console.WriteLine("[ConnectionStringHelper] Using appsettings.json connection string");
            return cs;
        }
    }
}
