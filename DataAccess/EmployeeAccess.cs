﻿using Dapper;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System.Data.SqlClient;


namespace DataAccess {
    public class EmployeeAccess : ICRUDAccess<Employee> {

        private readonly string? _connectionString;
        private readonly ILogger<ICRUDAccess<Employee>>? _logger;

        public EmployeeAccess(IConfiguration configuration, ILogger<ICRUDAccess<Employee>> logger = null) {
            _connectionString = configuration.GetConnectionString("DbAccessConnection");
            _logger = logger;
        }

        // Test
        public EmployeeAccess(string connectionStringTest) {
            _connectionString = connectionStringTest;
        }
        public async Task<int> Create(Employee entity) {
            int insertedEmployeeId = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"INSERT INTO Employee
                            (firstname,
                            lastname,
                            address,
                            birthdate,
                            phone,
                            email)
                            OUTPUT INSERTED.employeeId
                            VALUES
                            (@firstname,
                            @lastname,
                            @address,
                            @birthdate,
                            @phone,
                            @email)";
                try {
                    insertedEmployeeId = await con.ExecuteScalarAsync<int>(sql, entity);
                } catch (Exception ex) {
                    insertedEmployeeId = -1;
                    _logger?.LogError(ex.Message);
                    throw;
                }
            }
            return insertedEmployeeId;
        }

        public async Task<bool> Delete(int id) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"DELETE FROM employee 
                            WHERE
                            employeeId = @id";
                rowsAffected = await con.ExecuteAsync(sql, new { id });
            }
            return rowsAffected > 0;
        }

        public async Task<Employee> Get(int id) {
            using (var con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"SELECT employeeId, FirstName, LastName, Address, BirthDate, Phone, Email
                    FROM Employee
                    WHERE employeeId = @Id";
                try {
                    var result = await con.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
                    if (result == null) {
                        _logger?.LogInformation($"Employee with id {id} was not found.");
                    }
                    return result;
                } catch (Exception ex) {
                    _logger?.LogError(ex, "An error occurred when trying to retrieve employee with id {Id}.", id);
                    throw;
                }
            }
        }


        public async Task<List<Employee>> GetAll() {
            List<Employee>? foundEmployee;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"SELECT employeeId, FirstName, LastName, Address, BirthDate, Phone, Email
                         FROM Employee";

                try {
                    foundEmployee = (await con.QueryAsync<Employee>(sql)).ToList();
                } catch (Exception ex) {
                    foundEmployee = null;
                    _logger?.LogError(ex.Message);
                }
            }
            return foundEmployee;
        }

        public async Task<bool> Update(int EmployeeId, Employee employeeToUpdate) {
            int rowsAffected = -1;
            using (SqlConnection con = new SqlConnection(_connectionString)) {
                con.Open();
                var sql = @"UPDATE employee
                            SET
                                firstname = @firstname,
                                lastname = @lastname,
                                address = @address,
                                birthdate = @birthdate,
                                phone = @phone,
                                email = @email
                            WHERE
                                employeeId = @employeeId";
                rowsAffected = await con.ExecuteAsync(sql, employeeToUpdate);
            }
            return rowsAffected > 0;
        }

        // For test tear down
        public async Task<bool> DeleteAll() {
            using (var conn = new SqlConnection(_connectionString)) {
                conn.Open();
                var sql = @"DELETE FROM Employee";
                var rowsAffected = await conn.ExecuteAsync(sql);
                return rowsAffected > 0;
            }
        }
    }
}
