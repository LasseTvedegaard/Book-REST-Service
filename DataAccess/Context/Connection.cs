using Microsoft.Data.SqlClient;  // Skift fra Npgsql
using System.Data;

namespace DataAccess.Context
{
    public class Connection : IConnection
    {
        private readonly string _connectionString;

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);  // Ændret fra NpgsqlConnection
            connection.Open();
            return connection;
        }
    }
}