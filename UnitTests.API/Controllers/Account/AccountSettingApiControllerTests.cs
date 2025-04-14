using API.Controllers.Account;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Facades;

namespace UnitTests.API.Controllers.Account
{
    [TestFixture]
    public class AccountSettingApiControllerTests
    {
        private Mock<AccountSettingFacade> _accountSettingFacadeMock;
        private Mock<IAccountSettingRepository> _accountSettingRepositoryMock;
        private AccountSettingApiController _controller;
        private List<AccountSetting> _testAccountSettings;
        private AccountSetting _testAccountSetting;

        [SetUp]
        public void SetUp()
        {
            _accountSettingRepositoryMock = new Mock<IAccountSettingRepository>();
            _accountSettingFacadeMock = new Mock<AccountSettingFacade>();
            _accountSettingFacadeMock.Setup(f => f.AccountSettingRepository).Returns(_accountSettingRepositoryMock.Object);
            _controller = new AccountSettingApiController(_accountSettingFacadeMock.Object);
            _testAccountSettings = new List<AccountSetting>
            {
                new AccountSetting
                {
                    AccountSettingID = 1,
                    AccountHeadID = 1,
                    AccountHeadName = "Head1",
                    AccountControlID = 1,
                    AccountControlName = "Control1",
                    AccountSubControlID = 1,
                    AccountSubControlName = "SubControl1",
                    AccountActivityID = 1,
                    AccountActivityName = "Activity1",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    BranchID = 1,
                    BranchName = "Branch1",
                    UserID = 1,
                    FullName = "User One",
                    IsGlobal = true
                },
                new AccountSetting
                {
                    AccountSettingID = 2,
                    AccountHeadID = 2,
                    AccountHeadName = "Head2",
                    AccountControlID = 2,
                    AccountControlName = "Control2",
                    AccountSubControlID = 2,
                    AccountSubControlName = "SubControl2",
                    AccountActivityID = 2,
                    AccountActivityName = "Activity2",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    BranchID = 1,
                    BranchName = "Branch1",
                    UserID = 2,
                    FullName = "User Two",
                    IsGlobal = false
                }
            };
            _testAccountSetting = new AccountSetting
            {
                AccountSettingID = 3,
                AccountHeadID = 3,
                AccountHeadName = "Head3",
                AccountControlID = 3,
                AccountControlName = "Control3",
                AccountSubControlID = 3,
                AccountSubControlName = "SubControl3",
                AccountActivityID = 3,
                AccountActivityName = "Activity3",
                CompanyID = 2,
                CompanyName = "Company2",
                BranchID = 2,
                BranchName = "Branch2",
                UserID = 3,
                FullName = "User Three",
                IsGlobal = true
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentException_WhenFacadeIsNull()
        {
            Assert.Throws<ArgumentException>(() => new AccountSettingApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithAccountSettings_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _accountSettingRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                        .ReturnsAsync(_testAccountSettings);
            var result = await _controller.GetAll(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSettings);
            _accountSettingRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _accountSettingRepositoryMock.Setup(r => r.GetAllAsync(companyId, branchId))
                                        .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSettingRepositoryMock.Verify(r => r.GetAllAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithAccountSetting_WhenIdExists()
        {
            var id = 1;
            _accountSettingRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ReturnsAsync(_testAccountSettings[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSettings[0]);
            _accountSettingRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _accountSettingRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ReturnsAsync((AccountSetting)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _accountSettingRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _accountSettingRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                        .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSettingRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _accountSettingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountSetting>()))
                                        .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testAccountSetting);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testAccountSetting.AccountSettingID);
            createdResult.Value.Should().BeEquivalentTo(_testAccountSetting);
            _accountSettingRepositoryMock.Verify(r => r.AddAsync(It.Is<AccountSetting>(a => a == _testAccountSetting)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _accountSettingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountSetting>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _accountSettingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AccountSetting>()))
                                        .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testAccountSetting);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSettingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AccountSetting>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testAccountSetting.AccountSettingID;
            _accountSettingRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountSetting>()))
                                        .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testAccountSetting);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountSetting);
            _accountSettingRepositoryMock.Verify(r => r.UpdateAsync(It.Is<AccountSetting>(a => a == _testAccountSetting)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _accountSettingRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSetting>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testAccountSetting);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _accountSettingRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSetting>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testAccountSetting.AccountSettingID;
            var exceptionMessage = "Database error";
            _accountSettingRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AccountSetting>()))
                                        .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testAccountSetting);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountSettingRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AccountSetting>()), Times.Once());
        }
    }
}