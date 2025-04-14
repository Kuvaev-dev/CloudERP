using API.Controllers.Account;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Account
{
    [TestFixture]
    public class AccountSubControlApiControllerTests
    {
        private Mock<IAccountSubControlRepository> _accountSubControlRepositoryMock;
        private Mock<IAccountControlRepository> _accountControlRepositoryMock;
        private Mock<IAccountHeadRepository> _accountHeadRepositoryMock;
        private AccountSubControlApiController _controller;
        private List<AccountSubControl> _testAccountSubControls;
        private AccountSubControl _testAccountSubControl;

        [SetUp]
        public void SetUp()
        {
            _accountSubControlRepositoryMock = new Mock<IAccountSubControlRepository>();
            _accountControlRepositoryMock = new Mock<IAccountControlRepository>();
            _accountHeadRepositoryMock = new Mock<IAccountHeadRepository>();
            _controller = new AccountSubControlApiController(
                _accountSubControlRepositoryMock.Object,
                _accountControlRepositoryMock.Object,
                _accountHeadRepositoryMock.Object);
            _testAccountSubControls = new List<AccountSubControl>
            {
                new AccountSubControl
                {
                    AccountSubControlID = 1,
                    AccountSubControlName = "SubControl1",
                    AccountControlID = 1,
                    AccountControlName = "Control1",
                    AccountHeadID = 1,
                    AccountHeadName = "Head1",
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 1,
                    FullName = "User One",
                    IsGlobal = true
                },
                new AccountSubControl
                {
                    AccountSubControlID = 2,
                    AccountSubControlName = "SubControl2",
                    AccountControlID = 2,
                    AccountControlName = "Control2",
                    AccountHeadID = 2,
                    AccountHeadName = "Head2",
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 2,
                    FullName = "User Two",
                    IsGlobal = false
                }
            };
            _testAccountSubControl = new AccountSubControl
            {
                AccountSubControlID = 3,
                AccountSubControlName = "SubControl3",
                AccountControlID = 3,
                AccountControlName = "Control3",
                AccountHeadID = 3,
                AccountHeadName = "Head3",
                CompanyID = 2,
                BranchID = 2,
                UserID = 3,
                FullName = "User Three",
                IsGlobal = true
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenAccountSubControlRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountSubControlApiController(null, _accountControlRepositoryMock.Object, _accountHeadRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenAccountControlRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountSubControlApiController(_accountSubControlRepositoryMock.Object, null, _accountHeadRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenAccountHeadRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountSubControlApiController(_accountSubControlRepositoryMock.Object, _accountControlRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithAccountSubControls_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _accountSubControlRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                            .ReturnsAsync(_testAccountSubControls);
            var result = await _controller.GetAll(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSubControls);
            _accountSubControlRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _accountSubControlRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                            .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSubControlRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithAccountSubControl_WhenIdExists()
        {
            var id = 1;
            _accountSubControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                            .ReturnsAsync(_testAccountSubControls[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSubControls[0]);
            _accountSubControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _accountSubControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                            .ReturnsAsync((AccountSubControl)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _accountSubControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _accountSubControlRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                            .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSubControlRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _accountSubControlRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountSubControl>()))
                                            .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testAccountSubControl);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testAccountSubControl.AccountSubControlID);
            createdResult.Value.Should().BeEquivalentTo(_testAccountSubControl);
            _accountSubControlRepositoryMock.Verify(r => r.AddAsync(It.Is<AccountSubControl>(a => a == _testAccountSubControl)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _accountSubControlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountSubControl>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _accountSubControlRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountSubControl>()))
                                            .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testAccountSubControl);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSubControlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountSubControl>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testAccountSubControl.AccountSubControlID;
            _accountSubControlRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountSubControl>()))
                                            .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testAccountSubControl);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSubControl);
            _accountSubControlRepositoryMock.Verify(r => r.UpdateAsync(It.Is<AccountSubControl>(a => a == _testAccountSubControl)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _accountSubControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSubControl>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testAccountSubControl);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _accountSubControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSubControl>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testAccountSubControl.AccountSubControlID;
            var exceptionMessage = "Database error";
            _accountSubControlRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountSubControl>()))
                                            .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testAccountSubControl);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSubControlRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSubControl>()), Times.Once());
        }
    }
}