using API.Controllers.User.Settings;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utils.Helpers;

namespace UnitTests.Controllers.API.User.Settings
{
    [TestFixture]
    public class UserSettingApiControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUserTypeRepository> _userTypeRepositoryMock;
        private Mock<IPasswordHelper> _passwordHelperMock;
        private UserSettingApiController _controller;
        private Domain.Models.User _testUser;
        private Employee _testEmployee;
        private List<Domain.Models.User> _testUsers;
        private List<Employee> _testEmployees;

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userTypeRepositoryMock = new Mock<IUserTypeRepository>();
            _passwordHelperMock = new Mock<IPasswordHelper>();
            _controller = new UserSettingApiController(
                _employeeRepositoryMock.Object,
                _userRepositoryMock.Object,
                _userTypeRepositoryMock.Object,
                _passwordHelperMock.Object);

            _testUser = new Domain.Models.User
            {
                UserID = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = "password123",
                Salt = "somesalt",
                UserTypeID = 1,
                IsActive = true,
                UserTypeName = "Admin",
                BranchName = "Main Branch"
            };

            _testEmployee = new Employee
            {
                EmployeeID = 1,
                UserID = 1,
                Email = "john.doe@example.com",
                BranchID = 1,
                CompanyID = 1
            };

            _testUsers = new List<Domain.Models.User> { _testUser };
            _testEmployees = new List<Employee> { _testEmployee };

            // Налаштування ModelState для контролера
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenEmployeeRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserSettingApiController(
                null, _userRepositoryMock.Object, _userTypeRepositoryMock.Object, _passwordHelperMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenUserRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserSettingApiController(
                _employeeRepositoryMock.Object, null, _userTypeRepositoryMock.Object, _passwordHelperMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenUserTypeRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserSettingApiController(
                _employeeRepositoryMock.Object, _userRepositoryMock.Object, null, _passwordHelperMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenPasswordHelperIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserSettingApiController(
                _employeeRepositoryMock.Object, _userRepositoryMock.Object, _userTypeRepositoryMock.Object, null));
        }

        [Test]
        public async Task CreateUser_ShouldReturnOk_WhenUserAndEmployeeAreValidAndDoNotExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Domain.Models.User>());
            _employeeRepositoryMock.Setup(r => r.GetByBranchAsync(branchId, companyId))
                                   .ReturnsAsync(new List<Employee>());
            _userRepositoryMock.Setup(r => r.AddAsync(_testUser))
                               .Returns(Task.CompletedTask);
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(_testUser.UserID))
                                   .ReturnsAsync(_testEmployee);
            _employeeRepositoryMock.Setup(r => r.UpdateAsync(_testEmployee))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(_testUser, companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUser);
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(branchId, companyId), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(_testUser), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(_testUser.UserID), Times.Once());
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(_testEmployee), Times.Once());
        }

        [Test]
        public async Task CreateUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _controller.ModelState.AddModelError("Email", "Email is required.");
            var invalidUser = new Domain.Models.User { UserID = 1 }; // Email is null

            // Act
            var result = await _controller.CreateUser(invalidUser, companyId, branchId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model is invalid.");
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never());
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task CreateUser_ShouldReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(_testUsers);

            // Act
            var result = await _controller.CreateUser(_testUser, companyId, branchId);

            // Assert
            var conflictResult = result.Result as ConflictObjectResult;
            conflictResult.Should().NotBeNull();
            conflictResult.StatusCode.Should().Be(409);
            conflictResult.Value.Should().Be("User or employee already exists");
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(branchId, companyId), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task CreateUser_ShouldReturnConflict_WhenEmployeeAlreadyExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Domain.Models.User>());
            _employeeRepositoryMock.Setup(r => r.GetByBranchAsync(branchId, companyId))
                                   .ReturnsAsync(_testEmployees);

            // Act
            var result = await _controller.CreateUser(_testUser, companyId, branchId);

            // Assert
            var conflictResult = result.Result as ConflictObjectResult;
            conflictResult.Should().NotBeNull();
            conflictResult.StatusCode.Should().Be(409);
            conflictResult.Value.Should().Be("User or employee already exists");
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(branchId, companyId), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task CreateUser_ShouldReturnOk_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Domain.Models.User>());
            _employeeRepositoryMock.Setup(r => r.GetByBranchAsync(branchId, companyId))
                                   .ReturnsAsync(new List<Employee>());
            _userRepositoryMock.Setup(r => r.AddAsync(_testUser))
                               .Returns(Task.CompletedTask);
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(_testUser.UserID))
                                   .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.CreateUser(_testUser, companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUser);
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(branchId, companyId), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(_testUser), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(_testUser.UserID), Times.Once());
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task CreateUser_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CreateUser(_testUser, companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task UpdateUser_ShouldReturnOk_WhenUserIsValid()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.UpdateAsync(_testUser))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(_testUser);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUser);
            _userRepositoryMock.Verify(r => r.UpdateAsync(_testUser), Times.Once());
        }

        [Test]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required.");
            var invalidUser = new Domain.Models.User { UserID = 1 }; // Email is null

            // Act
            var result = await _controller.UpdateUser(invalidUser);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model is invalid.");
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.User>()), Times.Never());
        }

        [Test]
        public async Task UpdateUser_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Update error";
            _userRepositoryMock.Setup(r => r.UpdateAsync(_testUser))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.UpdateUser(_testUser);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Contain(exceptionMessage);
            _userRepositoryMock.Verify(r => r.UpdateAsync(_testUser), Times.Once());
        }

        [Test]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                               .ReturnsAsync(_testUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUser);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once());
        }

        [Test]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 999;
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                               .ReturnsAsync((Domain.Models.User)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model cannot be null.");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once());
        }

        [Test]
        public async Task GetUser_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int userId = 1;
            var exceptionMessage = "Retrieval error";
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once());
        }
    }
}