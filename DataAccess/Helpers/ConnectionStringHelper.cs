using Microsoft.Extensions.Configuration;
using System;

namespace DataAccess.Helpers {
    public static class ConnectionStringHelper {

        public static string GetConnectionString(IConfiguration configuration) {
            // Retrieve the connection string directly using GetConnectionString method
            var connectionString = configuration.GetConnectionString("DbAccessConnection");
            if (!string.IsNullOrWhiteSpace(connectionString)) {
                return connectionString;
            } else {
                throw new InvalidOperationException("The database connection string is missing in the application configuration.");
            }
        }
    }
}
