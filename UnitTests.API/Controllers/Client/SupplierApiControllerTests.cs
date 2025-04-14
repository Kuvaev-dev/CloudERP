using API.Controllers.Client;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Client
{
    [TestFixture]
    public class SupplierApiControllerTests
    {
        private Mock<ISupplierRepository> _supplierRepositoryMock;
        private SupplierApiController _controller;
        private List<Supplier> _testSuppliers;
        private Supplier _testSupplier;

        [SetUp]
        public void SetUp()
        {
            _supplierRepositoryMock = new Mock<ISupplierRepository>();
            _controller = new SupplierApiController(_supplierRepositoryMock.Object);
            _testSuppliers = new List<Supplier>
            {
                new Supplier
                {
                    SupplierID = 1,
                    SupplierName = "Supplier One",
                    SupplierAddress = "123 Main St",
                    SupplierConatctNo = "+1234567890",
                    SupplierEmail = "supplier1@example.com",
                    Discription = "Primary supplier",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    BranchID = 1,
                    BranchName = "Branch1",
                    UserID = 1,
                    UserName = "user1",
                    IsActive = true
                },
                new Supplier
                {
                    SupplierID = 2,
                    SupplierName = "Supplier Two",
                    SupplierAddress = "456 Oak St",
                    SupplierConatctNo = "+0987654321",
                    SupplierEmail = "supplier2@example.com",
                    Discription = "Secondary supplier",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    BranchID = 1,
                    BranchName = "Branch1",
                    UserID = 2,
                    UserName = "user2",
                    IsActive = false
                }
            };
            _testSupplier = new Supplier
            {
                SupplierID = 3,
                SupplierName = "Supplier Three",
                SupplierAddress = "789 Pine St",
                SupplierConatctNo = "+5555555555",
                SupplierEmail = "supplier3@example.com",
                Discription = "New supplier",
                CompanyID = 2,
                CompanyName = "Company2",
                BranchID = 2,
                BranchName = "Branch2",
                UserID = 3,
                UserName = "user3",
                IsActive = true
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SupplierApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithSuppliers_WhenRepositorySucceeds()
        {
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ReturnsAsync(_testSuppliers);
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSuppliers);
            _supplierRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll();
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetBySetting_ShouldReturnOkWithSuppliers_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _supplierRepositoryMock.Setup(r => r.GetByCompanyAndBranchAsync(companyId, branchId))
                                   .ReturnsAsync(_testSuppliers);
            var result = await _controller.GetBySetting(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSuppliers);
            _supplierRepositoryMock.Verify(r => r.GetByCompanyAndBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetBySetting_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.GetByCompanyAndBranchAsync(companyId, branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetBySetting(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.GetByCompanyAndBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithSupplier_WhenIdExists()
        {
            var id = 1;
            _supplierRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync(_testSuppliers[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSuppliers[0]);
            _supplierRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _supplierRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((Supplier)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _supplierRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _supplierRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Supplier>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testSupplier);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testSupplier.SupplierID);
            createdResult.Value.Should().BeEquivalentTo(_testSupplier);
            _supplierRepositoryMock.Verify(r => r.AddAsync(It.Is<Supplier>(s => s == _testSupplier)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _supplierRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Supplier>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Supplier>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testSupplier);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Supplier>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testSupplier.SupplierID;
            _supplierRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Supplier>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testSupplier);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSupplier);
            _supplierRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Supplier>(s => s == _testSupplier)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _supplierRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testSupplier);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _supplierRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testSupplier.SupplierID;
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Supplier>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testSupplier);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Once());
        }

        [Test]
        public async Task GetByBranch_ShouldReturnOkWithSuppliers_WhenRepositorySucceeds()
        {
            int branchId = 1;
            _supplierRepositoryMock.Setup(r => r.GetSuppliersByBranchesAsync(branchId))
                                   .ReturnsAsync(_testSuppliers);
            var result = await _controller.GetByBranch(branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSuppliers);
            _supplierRepositoryMock.Verify(r => r.GetSuppliersByBranchesAsync(branchId), Times.Once());
        }

        [Test]
        public async Task GetByBranch_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int branchId = 1;
            var exceptionMessage = "Database error";
            _supplierRepositoryMock.Setup(r => r.GetSuppliersByBranchesAsync(branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetByBranch(branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierRepositoryMock.Verify(r => r.GetSuppliersByBranchesAsync(branchId), Times.Once());
        }
    }
}