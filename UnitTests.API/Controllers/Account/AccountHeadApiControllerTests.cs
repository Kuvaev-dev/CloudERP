using API.Controllers.Account;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Account
{
    [TestFixture]
    public class AccountHeadApiControllerTests
    {
        private Mock<IAccountHeadRepository> _accountHeadRepositoryMock;
        private AccountHeadApiController _controller;
        private List<AccountHead> _testAccountHeads;
        private AccountHead _testAccountHead;

        [SetUp]
        public void SetUp()
        {
            _accountHeadRepositoryMock = new Mock<IAccountHeadRepository>();
            _controller = new AccountHeadApiController(_accountHeadRepositoryMock.Object);
            _testAccountHeads = new List<AccountHead>
            {
                new AccountHead
                {
                    AccountHeadID = 1,
                    AccountHeadName = "Head1",
                    Code = 1001,
                    UserID = 1,
                    FullName = "User One"
                },
                new AccountHead
                {
                    AccountHeadID = 2,
                    AccountHeadName = "Head2",
                    Code = 1002,
                    UserID = 2,
                    FullName = "User Two"
                }
            };
            _testAccountHead = new AccountHead
            {
                AccountHeadID = 3,
                AccountHeadName = "Head3",
                Code = 1003,
                UserID = 3,
                FullName = "User Three"
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AccountHeadApiController(null));
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_ShouldReturnOkWithAccountHeads_WhenRepositorySucceeds()
        {
            // Arrange
            _accountHeadRepositoryMock.Setup(r => r.GetAllAsync())
                                     .ReturnsAsync(_testAccountHeads);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountHeads);
            _accountHeadRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _accountHeadRepositoryMock.Setup(r => r.GetAllAsync())
                                     .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountHeadRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ShouldReturnOkWithAccountHead_WhenIdExists()
        {
            // Arrange
            var id = 1;
            _accountHeadRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                     .ReturnsAsync(_testAccountHeads[0]);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountHeads[0]);
            _accountHeadRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;
            _accountHeadRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                     .ReturnsAsync((AccountHead)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _accountHeadRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = 1;
            var exceptionMessage = "Database error";
            _accountHeadRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                     .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountHeadRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            _accountHeadRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountHead>()))
                                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testAccountHead);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testAccountHead.AccountHeadID);
            createdResult.Value.Should().BeEquivalentTo(_testAccountHead);
            _accountHeadRepositoryMock.Verify(r => r.AddAsync(It.Is<AccountHead>(a => a == _testAccountHead)), Times.Once());
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
            _accountHeadRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountHead>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _accountHeadRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountHead>()))
                                     .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testAccountHead);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountHeadRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountHead>()), Times.Once());
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            // Arrange
            var id = _testAccountHead.AccountHeadID;
            _accountHeadRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountHead>()))
                                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testAccountHead);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountHead);
            _accountHeadRepositoryMock.Verify(r => r.UpdateAsync(It.Is<AccountHead>(a => a == _testAccountHead)), Times.Once());
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
            _accountHeadRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountHead>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _controller.Update(id, _testAccountHead);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _accountHeadRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountHead>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = _testAccountHead.AccountHeadID;
            var exceptionMessage = "Database error";
            _accountHeadRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountHead>()))
                                     .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testAccountHead);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountHeadRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountHead>()), Times.Once());
        }

        #endregion
    }
}