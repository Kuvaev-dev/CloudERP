using API.Controllers.Branch;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Branch
{
    [TestFixture]
    public class BranchTypeApiControllerTests
    {
        private Mock<IBranchTypeRepository> _branchTypeRepositoryMock;
        private BranchTypeApiController _controller;
        private List<BranchType> _testBranchTypes;
        private BranchType _testBranchType;

        [SetUp]
        public void SetUp()
        {
            _branchTypeRepositoryMock = new Mock<IBranchTypeRepository>();
            _controller = new BranchTypeApiController(_branchTypeRepositoryMock.Object);
            _testBranchTypes = new List<BranchType>
            {
                new BranchType
                {
                    BranchTypeID = 1,
                    BranchTypeName = "Main"
                },
                new BranchType
                {
                    BranchTypeID = 2,
                    BranchTypeName = "Sub"
                }
            };
            _testBranchType = new BranchType
            {
                BranchTypeID = 3,
                BranchTypeName = "Regional"
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenRepositoryIsProvided()
        {
            var controller = new BranchTypeApiController(_branchTypeRepositoryMock.Object);
            controller.Should().NotBeNull();
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithBranchTypes_WhenRepositorySucceeds()
        {
            _branchTypeRepositoryMock.Setup(r => r.GetAllAsync())
                                    .ReturnsAsync(_testBranchTypes);
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranchTypes);
            _branchTypeRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _branchTypeRepositoryMock.Setup(r => r.GetAllAsync())
                                    .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll();
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchTypeRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithBranchType_WhenIdExists()
        {
            var id = 1;
            _branchTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                    .ReturnsAsync(_testBranchTypes[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranchTypes[0]);
            _branchTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _branchTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                    .ReturnsAsync((BranchType)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model cannot be null.");
            _branchTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _branchTypeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                    .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchTypeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _branchTypeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<BranchType>()))
                                    .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testBranchType);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testBranchType.BranchTypeID);
            createdResult.Value.Should().BeEquivalentTo(_testBranchType);
            _branchTypeRepositoryMock.Verify(r => r.AddAsync(It.Is<BranchType>(b => b == _testBranchType)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _branchTypeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BranchType>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _branchTypeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<BranchType>()))
                                    .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testBranchType);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchTypeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BranchType>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testBranchType.BranchTypeID;
            _branchTypeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<BranchType>()))
                                    .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testBranchType);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBranchType);
            _branchTypeRepositoryMock.Verify(r => r.UpdateAsync(It.Is<BranchType>(b => b == _testBranchType)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _branchTypeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BranchType>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testBranchType);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _branchTypeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BranchType>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testBranchType.BranchTypeID;
            var exceptionMessage = "Database error";
            _branchTypeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<BranchType>()))
                                    .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testBranchType);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _branchTypeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BranchType>()), Times.Once());
        }
    }
}