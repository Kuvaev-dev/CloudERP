using API.Controllers.Account;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Account
{
    [TestFixture]
    public class AccountControlApiControllerTests
    {
        private Mock<IAccountControlRepository> _accountControlRepositoryMock;
        private AccountControlApiController _controller;
        private List<AccountControl> _testAccountControls;
        private AccountControl _testAccountControl;

        [SetUp]
        public void SetUp()
        {
            _accountControlRepositoryMock = new Mock<IAccountControlRepository>();
            _controller = new AccountControlApiController(_accountControlRepositoryMock.Object);
            _testAccountControls = new List<AccountControl>
            {
                new AccountControl
                {
                    AccountControlID = 1,
                    AccountControlName = "Control1",
                    AccountHeadID = 1,
                    AccountHeadName = "Head1",
                    BranchID = 1,
                    CompanyID = 1,
                    UserID = 1,
                    FullName = "User One",
                    IsGlobal = true
                },
                new AccountControl
                {
                    AccountControlID = 2,
                    AccountControlName = "Control2",
                    AccountHeadID = 2,
                    AccountHeadName = "Head2",
                    BranchID = 1,
                    CompanyID = 1,
                    UserID = 2,
                    FullName = "User Two",
                    IsGlobal = false
                }
            };
            _testAccountControl = new AccountControl
            {
                AccountControlID = 3,
                AccountControlName = "Control3",
                AccountHeadID = 3,
                AccountHeadName = "Head3",
                BranchID = 2,
                CompanyID = 2,
                UserID = 3,
                FullName = "User Three",
                IsGlobal = true
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AccountControlApiController(null));
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_ShouldReturnOkWithAccountControls_WhenRepositorySucceeds()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _accountControlRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                         .ReturnsAsync(_testAccountControls);

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountControls);
            _accountControlRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _accountControlRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAll(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountControlRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ShouldReturnOkWithAccountControl_WhenIdExists()
        {
            // Arrange
            var id = 1;
            _accountControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ReturnsAsync(_testAccountControls[0]);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountControls[0]);
            _accountControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var id = 999;
            _accountControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ReturnsAsync((AccountControl)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _accountControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = 1;
            var exceptionMessage = "Database error";
            _accountControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            _accountControlRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountControl>()))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(_testAccountControl);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testAccountControl.AccountControlID);
            createdResult.Value.Should().BeEquivalentTo(_testAccountControl);
            _accountControlRepositoryMock.Verify(r => r.AddAsync(It.Is<AccountControl>(a => a == _testAccountControl)), Times.Once());
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
            _accountControlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountControl>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _accountControlRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountControl>()))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Create(_testAccountControl);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountControlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountControl>()), Times.Once());
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            // Arrange
            var id = _testAccountControl.AccountControlID;
            _accountControlRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountControl>()))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, _testAccountControl);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountControl);
            _accountControlRepositoryMock.Verify(r => r.UpdateAsync(It.Is<AccountControl>(a => a == _testAccountControl)), Times.Once());
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
            _accountControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountControl>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _controller.Update(id, _testAccountControl);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _accountControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountControl>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = _testAccountControl.AccountControlID;
            var exceptionMessage = "Database error";
            _accountControlRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountControl>()))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Update(id, _testAccountControl);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountControl>()), Times.Once());
        }

        #endregion
    }
}