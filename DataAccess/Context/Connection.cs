using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context {
    public class Connection : IConnection {

        private readonly string _connectionString;

        public Connection(string connectionString) {
            _connectionString = connectionString;
        }
        public IDbConnection GetConnection() {
            //AttributeTypeMapperExtensions.RegisterAttributeTypeMappers(AppDomain.CurrentDomain.GetAssemblies()
            //    .Single(x => !string.IsNullOrEmpty(x.FullName) && x.FullName.StartsWith("Domain")));

            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
