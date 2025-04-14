using API.Controllers.Utilities;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.Utilities
{
    [TestFixture]
    public class TaskApiControllerTests
    {
        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private TaskApiController _controller;
        private TaskModel _testTask;
        private Employee _testEmployee;
        private List<TaskModel> _testTasks;
        private List<Employee> _testEmployees;

        [SetUp]
        public void SetUp()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _controller = new TaskApiController(_taskRepositoryMock.Object, _employeeRepositoryMock.Object);

            _testTask = new TaskModel
            {
                TaskID = 1,
                Title = "Test Task",
                Description = "This is a test task",
                DueDate = new DateTime(2024, 12, 31),
                ReminderDate = new DateTime(2024, 12, 30),
                AssignedByUserID = 1,
                AssignedToUserID = 2,
                IsCompleted = false,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testEmployee = new Employee
            {
                EmployeeID = 1,
                UserID = 2,
                FullName = "John Doe",
                CompanyID = 1,
                BranchID = 1
            };

            _testTasks = new List<TaskModel> { _testTask };
            _testEmployees = new List<Employee> { _testEmployee };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenTaskRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TaskApiController(null, _employeeRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenEmployeeRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TaskApiController(_taskRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithTasks_WhenTasksExist()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            _taskRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId, userId))
                               .ReturnsAsync(_testTasks);

            // Act
            var result = await _controller.GetAll(companyId, branchId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTasks);
            _taskRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId, userId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnNotFound_WhenNoTasksExist()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            _taskRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId, userId))
                               .ReturnsAsync((IEnumerable<TaskModel>)null);

            // Act
            var result = await _controller.GetAll(companyId, branchId, userId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _taskRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId, userId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var exceptionMessage = "Database error";
            _taskRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId, userId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll(companyId, branchId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _taskRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId, userId), Times.Once());
        }

        [Test]
        public async Task GetEmployeesForTaskAssignment_ShouldReturnOkWithEmployees_WhenEmployeesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _employeeRepositoryMock.Setup(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId))
                                   .ReturnsAsync(_testEmployees);

            // Act
            var result = await _controller.GetEmployeesForTaskAssignment(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testEmployees);
            _employeeRepositoryMock.Verify(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetEmployeesForTaskAssignment_ShouldReturnNotFound_WhenNoEmployeesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _employeeRepositoryMock.Setup(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId))
                                   .ReturnsAsync((IEnumerable<Employee>)null);

            // Act
            var result = await _controller.GetEmployeesForTaskAssignment(companyId, branchId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _employeeRepositoryMock.Verify(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetEmployeesForTaskAssignment_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetEmployeesForTaskAssignment(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.GetEmployeesForTaskAssignmentAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithTask_WhenTaskExists()
        {
            // Arrange
            int taskId = 1;
            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId))
                               .ReturnsAsync(_testTask);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTask);
            _taskRepositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int taskId = 999;
            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId))
                               .ReturnsAsync((TaskModel)null);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _taskRepositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int taskId = 1;
            var exceptionMessage = "Database error";
            _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _taskRepositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenTaskIsValid()
        {
            // Arrange
            _taskRepositoryMock.Setup(r => r.AddAsync(_testTask))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testTask);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testTask.TaskID);
            createdResult.Value.Should().BeEquivalentTo(_testTask);
            _taskRepositoryMock.Verify(r => r.AddAsync(_testTask), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskModel>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _taskRepositoryMock.Setup(r => r.AddAsync(_testTask))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testTask);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _taskRepositoryMock.Verify(r => r.AddAsync(_testTask), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenTaskIsValid()
        {
            // Arrange
            int taskId = 1;
            _testTask.TaskID = taskId;
            _taskRepositoryMock.Setup(r => r.UpdateAsync(_testTask))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(taskId, _testTask);

            // Assert
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            _taskRepositoryMock.Verify(r => r.UpdateAsync(_testTask), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Arrange
            int taskId = 1;

            // Act
            var result = await _controller.Update(taskId, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskModel>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            int taskId = 999;
            _testTask.TaskID = 1;

            // Act
            var result = await _controller.Update(taskId, _testTask);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskModel>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int taskId = 1;
            _testTask.TaskID = taskId;
            var exceptionMessage = "Database error";
            _taskRepositoryMock.Setup(r => r.UpdateAsync(_testTask))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(taskId, _testTask);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _taskRepositoryMock.Verify(r => r.UpdateAsync(_testTask), Times.Once());
        }

        [Test]
        public async Task Delete_ShouldReturnOk_WhenTaskExists()
        {
            // Arrange
            int taskId = 1;
            _taskRepositoryMock.Setup(r => r.DeleteAsync(taskId))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            _taskRepositoryMock.Verify(r => r.DeleteAsync(taskId), Times.Once());
        }

        [Test]
        public async Task Delete_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int taskId = 1;
            var exceptionMessage = "Database error";
            _taskRepositoryMock.Setup(r => r.DeleteAsync(taskId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _taskRepositoryMock.Verify(r => r.DeleteAsync(taskId), Times.Once());
        }
    }
}