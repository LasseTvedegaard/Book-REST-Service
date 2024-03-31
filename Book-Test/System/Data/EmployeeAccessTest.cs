using FluentAssertions;
using Book_Test.Helper;
using System.Data;
using System.Data.SqlClient;
using Model;
using Book_Test.Fixtures;
using DataAccess;
using DataAccess.Interfaces;
using Book_Test.Fixtures.Book_Test.Fixtures;

namespace Book_Test.System.Data {
    public class EmployeeAccessTest : IDisposable {

        private readonly ICRUDAccess<Employee> _employeeAccess;

        // Connection for test DB
        private readonly string _connectionStringTest = "Data Source=.;Database=TestBookLibrary;Data Source=.;integrated security=true";
        
        public EmployeeAccessTest() {
            _employeeAccess = new EmployeeAccess(_connectionStringTest);
        }

        [Fact]
        public void WasConnectedToDatabase() {
            // Arrange
            using (SqlConnection con = new SqlConnection(_connectionStringTest)) {

                // Act
                con.Open();

                // Assert
                con.State.Should().Be(ConnectionState.Open);
            }
        }

        [Fact]
        public async void Should_Insert_New_Employee() {
            // Arrange
            Employee employeeToCreate = EmployeeFixtures.GetAnEmployee();

            // Act
            var result = await _employeeAccess.Create(employeeToCreate);

            // Assert
            Assert.True(result > 0);
        }

        [Fact]
        public async void Should_Get_All_Employees() {
            // Arrange
            int expectedResult = 3;

            // Act
            var employees = await _employeeAccess.GetAll();
            int result = employees.Count();

            // Assert
            Assert.Equal(expectedResult, result);
            result.Should().Be(expectedResult);
        }

        [Fact]  
        public async void Should_Update_An_Employee() {
            // Arrange
            int idToUpdate = 1;
            Employee employeeToUpdate = EmployeeFixtures.GetAnEmployee();
            employeeToUpdate.EmployeeId = idToUpdate;

            // Act
            var result = await _employeeAccess.Update(idToUpdate, employeeToUpdate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async void Should_Delete_An_Employee() {
            // Arrange 
            int idToDelete = 3;

            // Act
            bool result = await _employeeAccess.Delete(idToDelete); 

            // Assert
            Assert.True(result);
        }


        public void Dispose() {
            // To be done after all tests
            int result = DatabaseCleanerEmployee.CleanEmployeeDatabase(_connectionStringTest);
            
            // Assert
            Assert.NotEqual(0, result);
        }
    }
}
