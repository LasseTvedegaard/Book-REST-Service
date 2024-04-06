using Bogus;
using Book_REST_Service.Controllers;
using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Model;
using Moq;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Book_Test.System.Controllers {
    public class EmployeeIntegrationTest : IDisposable {
        private readonly IDbConnection _dbConnectionTest;
        private readonly ICRUDAccess<Employee> _employeeAccess;
        private readonly IConfiguration _configuration;

        public EmployeeIntegrationTest() {
            try {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.Test.Json", optional: false, reloadOnChange: true)
                    .Build();

                // Rest of the setup code...
            } catch (FileNotFoundException fnEx) {
                // Log or handle the exception as necessary, for example:
                Console.WriteLine($"Could not find the configuration file: {fnEx.Message}");
                throw; // Re-throwing the exception if you want to stop the execution
            }
        

        var connectionString = _configuration.GetConnectionString("TestDatabaseConnection");
            _dbConnectionTest = new SqlConnection(connectionString);
            _dbConnectionTest.Open();

            _employeeAccess = new EmployeeAccess(connectionString);
        }

        private async Task SetupAsync() {
            var faker = new Faker<Employee>()
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(100))
                .RuleFor(o => o.Address, f => f.Address.FullAddress())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumberFormat(0)) 
                .RuleFor(o => o.Email, f => f.Internet.Email());

            for (int i = 0; i < 2; i++) {
                var newEmployee = faker.Generate();
                await _employeeAccess.Create(newEmployee);
            }
        }

        private async Task TeardownAsync() {
            var employees = await _employeeAccess.GetAll();
            foreach (var employee in employees) {
                if (employee.Email.EndsWith("@test.com") && employee.EmployeeId.HasValue) {
                    await _employeeAccess.Delete(employee.EmployeeId.Value);
                }
            }
        }

        [Fact]
        public async Task CreateEmployee_ShouldSuccessfullyAddEmployee() {
            // Arrange
            await SetupAsync();

            var faker = new Faker<Employee>()
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(100))
                .RuleFor(o => o.Address, f => f.Address.FullAddress())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(o => o.Email, f => f.Internet.Email());

            var newEmployee = faker.Generate();

            // Act
            var createdEmployeeId = await _employeeAccess.Create(newEmployee);

            // Assert
            createdEmployeeId.Should().BeGreaterThan(0);

            var createdEmployee = await _employeeAccess.Get(createdEmployeeId);
            createdEmployee.Should().NotBeNull();

            createdEmployee.FirstName.Should().Be(newEmployee.FirstName);
            createdEmployee.Email.Should().Be(newEmployee.Email);

            // Teardown
            await TeardownAsync();
        }
        public void Dispose() {
            _dbConnectionTest?.Dispose();

        }
    }
}