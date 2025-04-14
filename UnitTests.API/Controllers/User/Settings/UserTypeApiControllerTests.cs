using API.Controllers.User.Settings;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.User.Settings
{
    [TestFixture]
    public class UserTypeApiControllerTests
    {
        private Mock<IUserTypeRepository> _userTypeRepositoryMock;
        private UserTypeApiController _controller;
        private UserType _testUserType;
        private List<UserType> _testUserTypes;

        [SetUp]
        public void SetUp()
        {
            _userTypeRepositoryMock = new Mock<IUserTypeRepository>();
            _controller = new UserTypeApiController(_userTypeRepositoryMock.Object);

            _testUserType = new UserType
            {
                UserTypeID = 1,
                UserTypeName = "Admin"
            };

            _testUserTypes = new List<UserType>
            {
                _testUserType,
                new UserType
                {
                    UserTypeID = 2,
                    UserTypeName = "Employee"
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserTypeApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithUserTypes_WhenUserTypesExist()
        {
            // Arrange
            _userTypeRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ReturnsAsync(_testUserTypes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUserTypes);
            _userTypeRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoUserTypesExist()
        {
            // Arrange
            _userTypeRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ReturnsAsync(new List<UserType>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.As<IEnumerable<UserType>>().Should().BeEmpty();
            _userTypeRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _userTypeRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userTypeRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithUserType_WhenUserTypeExists()
        {
            // Arrange
            int id = 1;
            _userTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync(_testUserType);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUserType);
            _userTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenUserTypeDoesNotExist()
        {
            // Arrange
            int id = 999;
            _userTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((UserType)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _userTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Retrieval error";
            _userTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenUserTypeIsValid()
        {
            // Arrange
            _userTypeRepositoryMock.Setup(r => r.AddAsync(_testUserType))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testUserType);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testUserType.UserTypeID);
            createdResult.Value.Should().BeEquivalentTo(_testUserType);
            _userTypeRepositoryMock.Verify(r => r.AddAsync(_testUserType), Times.Once());
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
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _userTypeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserType>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _userTypeRepositoryMock.Setup(r => r.AddAsync(_testUserType))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testUserType);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userTypeRepositoryMock.Verify(r => r.AddAsync(_testUserType), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenUserTypeIsValid()
        {
            // Arrange
            int id = 1;
            _userTypeRepositoryMock.Setup(r => r.UpdateAsync(_testUserType))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testUserType);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testUserType);
            _userTypeRepositoryMock.Verify(r => r.UpdateAsync(_testUserType), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.Update(id, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _userTypeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserType>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            int id = 999;
            _testUserType.UserTypeID = 1;

            // Act
            var result = await _controller.Update(id, _testUserType);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _userTypeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserType>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Update error";
            _userTypeRepositoryMock.Setup(r => r.UpdateAsync(_testUserType))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testUserType);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _userTypeRepositoryMock.Verify(r => r.UpdateAsync(_testUserType), Times.Once());
        }
    }
}