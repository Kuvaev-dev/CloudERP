using API.Controllers.Client;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Client
{
    [TestFixture]
    public class CustomerApiControllerTests
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private CustomerApiController _controller;
        private List<Customer> _testCustomers;
        private Customer _testCustomer;

        [SetUp]
        public void SetUp()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _controller = new CustomerApiController(_customerRepositoryMock.Object);
            _testCustomers = new List<Customer>
            {
                new Customer
                {
                    CustomerID = 1,
                    Customername = "John Doe",
                    CustomerContact = "+1234567890",
                    CustomerArea = "Downtown",
                    CustomerAddress = "123 Main St",
                    Description = "Regular customer",
                    BranchID = 1,
                    BranchName = "Branch1",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    UserID = 1,
                    UserName = "john_doe"
                },
                new Customer
                {
                    CustomerID = 2,
                    Customername = "Jane Smith",
                    CustomerContact = "+0987654321",
                    CustomerArea = "Uptown",
                    CustomerAddress = "456 Oak St",
                    Description = "VIP customer",
                    BranchID = 1,
                    BranchName = "Branch1",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    UserID = 2,
                    UserName = "jane_smith"
                }
            };
            _testCustomer = new Customer
            {
                CustomerID = 3,
                Customername = "Alice Johnson",
                CustomerContact = "+5555555555",
                CustomerArea = "Midtown",
                CustomerAddress = "789 Pine St",
                Description = "New customer",
                BranchID = 2,
                BranchName = "Branch2",
                CompanyID = 2,
                CompanyName = "Company2",
                UserID = 3,
                UserName = "alice_johnson"
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerApiController(null));
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithCustomers_WhenRepositorySucceeds()
        {
            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ReturnsAsync(_testCustomers);
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCustomers);
            _customerRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll();
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once());
        }

        [Test]
        public async Task GetBySetting_ShouldReturnOkWithCustomers_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _customerRepositoryMock.Setup(r => r.GetByCompanyAndBranchAsync(companyId, branchId))
                                   .ReturnsAsync(_testCustomers);
            var result = await _controller.GetBySetting(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCustomers);
            _customerRepositoryMock.Verify(r => r.GetByCompanyAndBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetBySetting_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _customerRepositoryMock.Setup(r => r.GetByCompanyAndBranchAsync(companyId, branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetBySetting(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerRepositoryMock.Verify(r => r.GetByCompanyAndBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithCustomer_WhenIdExists()
        {
            var id = 1;
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync(_testCustomers[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCustomers[0]);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((Customer)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.Create(_testCustomer);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testCustomer.CustomerID);
            createdResult.Value.Should().BeEquivalentTo(_testCustomer);
            _customerRepositoryMock.Verify(r => r.AddAsync(It.Is<Customer>(c => c == _testCustomer)), Times.Once());
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Create(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never());
        }

        [Test]
        public async Task Create_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Create(_testCustomer);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnOk_WhenModelIsValid()
        {
            var id = _testCustomer.CustomerID;
            _customerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.Update(id, _testCustomer);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testCustomer);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Customer>(c => c == _testCustomer)), Times.Once());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Update(1, null);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            var id = 999;
            var result = await _controller.Update(id, _testCustomer);
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("ID in the request does not match the model ID.");
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never());
        }

        [Test]
        public async Task Update_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = _testCustomer.CustomerID;
            var exceptionMessage = "Database error";
            _customerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Update(id, _testCustomer);
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once());
        }
    }
}