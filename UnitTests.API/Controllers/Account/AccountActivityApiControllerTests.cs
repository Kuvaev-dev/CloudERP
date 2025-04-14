using API.Controllers.Account;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Account
{
    [TestFixture]
    public class AccountActivityApiControllerTests
    {
        private Mock<IAccountActivityRepository> _accountActivityRepositoryMock;
        private AccountActivityApiController _controller;
        private List<AccountActivity> _testActivities;
        private AccountActivity _testActivity;

        [SetUp]
        public void SetUp()
        {
            _accountActivityRepositoryMock = new Mock<IAccountActivityRepository>();
            _controller = new AccountActivityApiController(_accountActivityRepositoryMock.Object);
            _testActivities = new List<AccountActivity>
            {
                new AccountActivity { AccountActivityID = 1, Name = "Login" },
                new AccountActivity { AccountActivityID = 2, Name = "Logout" }
            };
            _testActivity = new AccountActivity { AccountActivityID = 3, Name = "Update" };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AccountActivityApiController(null));
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_ShouldReturnOkWithActivities_WhenRepositorySucceeds()
        {
            // Arrange
            _accountActivityRepositoryMock.Setup(r => r.GetAllAsync())
                                         .ReturnsAsync(_testActivities);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testActivities);
            _accountActivityRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _accountActivityRepositoryMock.Setup(r => r.GetAllAsync())
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountActivityRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ShouldReturnOkWithActivity_WhenIdExists()
        {
            // Arrange
            var id = 1;
            _accountActivityRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ReturnsAsync(_testActivities[0]);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testActivities[0]);
            _accountActivityRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;
            _accountActivityRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ReturnsAsync((AccountActivity)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _accountActivityRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = 1;
            var exceptionMessage = "Database error";
            _accountActivityRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountActivityRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            _accountActivityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountActivity>()))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testActivity);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testActivity.AccountActivityID);
            createdResult.Value.Should().BeEquivalentTo(_testActivity);
            _accountActivityRepositoryMock.Verify(r => r.AddAsync(It.Is<AccountActivity>(a => a == _testActivity)), Times.Once());
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
            _accountActivityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountActivity>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _accountActivityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountActivity>()))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testActivity);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountActivityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountActivity>()), Times.Once());
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            // Arrange
            var id = _testActivity.AccountActivityID;
            _accountActivityRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountActivity>()))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testActivity);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testActivity);
            _accountActivityRepositoryMock.Verify(r => r.UpdateAsync(It.Is<AccountActivity>(a => a == _testActivity)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Update(1, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _accountActivityRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountActivity>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _controller.Update(id, _testActivity);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _accountActivityRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountActivity>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = _testActivity.AccountActivityID;
            var exceptionMessage = "Database error";
            _accountActivityRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountActivity>()))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testActivity);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountActivityRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountActivity>()), Times.Once());
        }

        #endregion
    }
}