using Book_REST_Service.Controllers;
using Book_Test.Fixtures.Book_Test.Fixtures;
using BusinessLogic.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Model;
using Moq;

namespace Book_Test.System.Controllers {
    public class EmployeeControllerTest {

        [Fact]
        public async Task Should_Create_Employee_Return_201() {

            // Arrange
            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            var tester = new EmployeeController(mockEmployeeControl.Object);
            Employee employeeToCreate = EmployeeFixtures.GetAnEmployee();

            mockEmployeeControl
                .Setup(x => x.Create(employeeToCreate))
                .ReturnsAsync(1);

            // Act
            var result = (CreatedAtActionResult)await tester.CreateEmployee(employeeToCreate);

            // Assert 
            result.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Should_Fail_To_Create_Employee() {

            // Arrange
            Employee employeeToCreate = EmployeeFixtures.GetAnEmployee();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(service => service.Create(employeeToCreate))
                .ReturnsAsync(-1);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = await controller.CreateEmployee(employeeToCreate);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Should_Succeed_To_Retrieve_An_Empty_List() {
            // Arrange
            List<Employee> employeesToRetrieve = new List<Employee>();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.GetAll())
                .ReturnsAsync(employeesToRetrieve);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var Result = await controller.Get();

            // Assert
            Result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<List<Employee>>()
                .Which.Count().Should().Be(0);
        }

        [Fact]
        public async Task Should_Succeed_To_Retrieve_List() {
            // Arrange
            List<Employee> employeesToRetrieve = EmployeeFixtures.GetListOfEmployees();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();
           
            mockEmployeeControl
                .Setup(x => x.GetAll())
                .ReturnsAsync(employeesToRetrieve);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = await controller.Get();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should()
                .BeAssignableTo<List<Employee>>()
                .Which.Should()
                .HaveCount(3);
        }

        [Fact]
        public async Task Should_Fail_To_Retrieve_List() {
            // Arrange
            List<Employee> employeeToRetrieve = new List<Employee>();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.GetAll())
                .ReturnsAsync(employeeToRetrieve);

            var controller = new EmployeeController (mockEmployeeControl.Object);

            // Act
            var result = await controller.Get();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should()
                .BeAssignableTo<List<Employee>>()
                .Which.Should()
                .HaveCount(0);
        }

        [Fact]
        public async Task Should_Update_Employee() {
            // Arrange
            Employee employeeToUpdate = EmployeeFixtures.GetAnEmployee();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.Update(1, employeeToUpdate))
                .ReturnsAsync(true);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = (StatusCodeResult)await controller.UpdateEmployee(1, employeeToUpdate);

            // Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Should_Fail_To_Update_Employee() {
            // Arrange
            Employee employeeToUpdate = EmployeeFixtures.GetAnEmployee();

            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.Update(1, employeeToUpdate))
                .ReturnsAsync(false);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = (StatusCodeResult)await controller.UpdateEmployee(1, employeeToUpdate);

            // Assert
            result.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task Should_Delete_Employee() {
            // Arrange
            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.Delete(140))
                .ReturnsAsync(true);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = (StatusCodeResult)await controller.DeleteEmployee(140);

            // Assert
            result.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task Should_Fail_To_Delete_Employee() {
            // Arrange
            var mockEmployeeControl = new Mock<ICRUD<Employee>>();

            mockEmployeeControl
                .Setup(x => x.Delete(10))
                .ReturnsAsync(false);

            var controller = new EmployeeController(mockEmployeeControl.Object);

            // Act
            var result = (StatusCodeResult)await controller.DeleteEmployee(10);

            // Assert
            result.StatusCode.Should().Be(500);
        }
    }
}
