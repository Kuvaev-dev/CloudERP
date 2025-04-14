using API.Controllers.Branch;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Branch
{
    [TestFixture]
    public class BranchApiControllerTests
    {
        private Mock<IBranchRepository> _branchRepositoryMock;
        private Mock<IBranchTypeRepository> _branchTypeRepositoryMock;
        private Mock<IAccountSettingRepository> _accountSettingRepositoryMock;
        private BranchApiController _controller;
        private List<Domain.Models.Branch> _testBranches;
        private Domain.Models.Branch _testBranch;
        private const int MAIN_BRANCH_TYPE_ID = 1;

        [SetUp]
        public void SetUp()
        {
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _branchTypeRepositoryMock = new Mock<IBranchTypeRepository>();
            _accountSettingRepositoryMock = new Mock<IAccountSettingRepository>();
            _controller = new BranchApiController(
                _branchRepositoryMock.Object,
                _branchTypeRepositoryMock.Object,
                _accountSettingRepositoryMock.Object);
            _testBranches = new List<Domain.Models.Branch>
            {
                new Domain.Models.Branch
                {
                    BranchID = 1,
                    BrchID = 101,
                    BranchName = "Branch1",
                    BranchContact = "1234567890",
                    BranchAddress = "123 Main St",
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    CompanyID = 1,
                    ParentBranchID = null,
                    BranchTypeID = 1,
                    BranchTypeName = "Main"
                },
                new Domain.Models.Branch
                {
                    BranchID = 2,
                    BrchID = 102,
                    BranchName = "Branch2",
                    BranchContact = "0987654321",
                    BranchAddress = "456 Oak St",
                    Latitude = 34.0522,
                    Longitude = -118.2437,
                    CompanyID = 1,
                    ParentBranchID = 1,
                    BranchTypeID = 2,
                    BranchTypeName = "Sub"
                }
            };
            _testBranch = new Domain.Models.Branch
            {
                BranchID = 3,
                BrchID = 103,
                BranchName = "Branch3",
                BranchContact = "5555555555",
                BranchAddress = "789 Pine St",
                Latitude = 51.5074,
                Longitude = -0.1278,
                CompanyID = 2,
                ParentBranchID = null,
                BranchTypeID = 1,
                BranchTypeName = "Main"
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenBranchRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchApiController(null, _branchTypeRepositoryMock.Object, _accountSettingRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenBranchTypeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchApiController(_branchRepositoryMock.Object, null, _accountSettingRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenAccountSettingRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchApiController(_branchRepositoryMock.Object, _branchTypeRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithMainBranches_WhenMainBranchTypeId()
        {
            int companyId = 1, branchId = 1;
            _branchRepositoryMock.Setup(r => r.GetByCompanyAsync(companyId))
                                 .ReturnsAsync(_testBranches);
            var result = await _controller.GetAll(companyId, branchId, MAIN_BRANCH_TYPE_ID);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranches);
            _branchRepositoryMock.Verify(r => r.GetByCompanyAsync(companyId), Times.Once());
            _branchRepositoryMock.Verify(r => r.GetSubBranchAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithSubBranches_WhenNotMainBranchTypeId()
        {
            int companyId = 1, branchId = 1, otherBranchTypeId = 2;
            _branchRepositoryMock.Setup(r => r.GetSubBranchAsync(companyId, branchId))
                                 .ReturnsAsync(_testBranches);
            var result = await _controller.GetAll(companyId, branchId, otherBranchTypeId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranches);
            _branchRepositoryMock.Verify(r => r.GetSubBranchAsync(companyId, branchId), Times.Once());
            _branchRepositoryMock.Verify(r => r.GetByCompanyAsync(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.GetByCompanyAsync(companyId))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll(companyId, branchId, MAIN_BRANCH_TYPE_ID);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.GetByCompanyAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetSubBranches_ShouldReturnOkWithSubBranches_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _branchRepositoryMock.Setup(r => r.GetSubBranchAsync(companyId, branchId))
                                 .ReturnsAsync(_testBranches);
            var result = await _controller.GetSubBranches(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranches);
            _branchRepositoryMock.Verify(r => r.GetSubBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetSubBranches_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.GetSubBranchAsync(companyId, branchId))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetSubBranches(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.GetSubBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetByCompany_ShouldReturnOkWithBranches_WhenRepositorySucceeds()
        {
            int companyId = 1;
            _branchRepositoryMock.Setup(r => r.GetByCompanyAsync(companyId))
                                 .ReturnsAsync(_testBranches);
            var result = await _controller.GetByCompany(companyId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranches);
            _branchRepositoryMock.Verify(r => r.GetByCompanyAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetByCompany_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1;
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.GetByCompanyAsync(companyId))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetByCompany(companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.GetByCompanyAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithBranch_WhenIdExists()
        {
            var id = 1;
            _branchRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                 .ReturnsAsync(_testBranches[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranches[0]);
            _branchRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _branchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()))
                                 .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testBranch);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetByCompany));
            createdResult.RouteValues["companyId"].Should().Be(_testBranch.CompanyID);
            createdResult.Value.Should().BeEquivalentTo(_testBranch);
            _branchRepositoryMock.Verify(r => r.AddAsync(It.Is<Domain.Models.Branch>(b => b == _testBranch)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testBranch);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testBranch.BranchID;
            _branchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Branch>()))
                                 .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testBranch);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranch);
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Domain.Models.Branch>(b => b == _testBranch)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testBranch);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testBranch.BranchID;
            var exceptionMessage = "Database error";
            _branchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Branch>()))
                                 .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testBranch);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Branch>()), Times.Once());
        }
    }
}