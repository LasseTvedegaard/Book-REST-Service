using Dapper;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

namespace DataAccess
{
    public class EmployeeAccess : ICRUDAccess<Employee>
    {
        private IConnection _connection;
        private readonly ILogger<EmployeeAccess>? _logger;

        public EmployeeAccess(IGenericConnection<LibraryConnection> pgConnection, IConfiguration configuration, ILogger<EmployeeAccess> logger = null)
        {
            _connection = pgConnection;
            _logger = logger;
        }

        // For testing purposes
        public EmployeeAccess(IConnection connection)
        {
            _connection = connection;
        }

        // Create a new employee
        public async Task<int> Create(Employee entity)
        {
            int insertedEmployeeId = -1;
            try
            {
                using var db = _connection.GetConnection();
                var sql = @"
                    INSERT INTO Employee (
                        FirstName,
                        LastName,
                        Address,
                        BirthDate,
                        Phone,
                        Email
                    )
                    VALUES (
                        @FirstName,
                        @LastName,
                        @Address,
                        @BirthDate,
                        @Phone,
                        @Email
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                insertedEmployeeId = await db.ExecuteScalarAsync<int>(sql, entity);
            } catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                throw;
            }

            return insertedEmployeeId;
        }

        // Delete employee by ID
        public async Task<bool> Delete(int id)
        {
            using var db = _connection.GetConnection();
            var sql = @"
                DELETE FROM Employee
                WHERE EmployeeId = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }

        // Get employee by ID
        public async Task<Employee> Get(int id)
        {
            using var db = _connection.GetConnection();
            var sql = @"
                SELECT
                    EmployeeId,
                    FirstName,
                    LastName,
                    Address,
                    BirthDate,
                    Phone,
                    Email
                FROM Employee
                WHERE EmployeeId = @Id";

            try
            {
                var result = await db.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
                if (result == null)
                {
                    _logger?.LogInformation($"Employee with id {id} was not found.");
                }
                return result;
            } catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred when trying to retrieve employee with id {Id}.", id);
                throw;
            }
        }

        // Get all employees
        public async Task<List<Employee>> GetAll()
        {
            List<Employee>? foundEmployees;
            using var db = _connection.GetConnection();
            var sql = @"
                SELECT
                    EmployeeId,
                    FirstName,
                    LastName,
                    Address,
                    BirthDate,
                    Phone,
                    Email
                FROM Employee";

            try
            {
                foundEmployees = (await db.QueryAsync<Employee>(sql)).ToList();
            } catch (Exception ex)
            {
                foundEmployees = null;
                _logger?.LogError(ex, "An error occurred while fetching all employees.");
            }

            return foundEmployees;
        }

        // Update an employee by ID
        public async Task<bool> Update(int EmployeeId, Employee employeeToUpdate)
        {
            using var db = _connection.GetConnection();
            var sql = @"
                UPDATE Employee
                SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Address = @Address,
                    BirthDate = @BirthDate,
                    Phone = @Phone,
                    Email = @Email
                WHERE EmployeeId = @EmployeeId";

            var rowsAffected = await db.ExecuteAsync(sql, employeeToUpdate);
            return rowsAffected > 0;
        }

        // For test tear down
        public async Task<bool> DeleteAll()
        {
            using var db = _connection.GetConnection();
            var sql = @"
                DELETE FROM Employee";

            var rowsAffected = await db.ExecuteAsync(sql);
            return rowsAffected > 0;
        }
    }
}