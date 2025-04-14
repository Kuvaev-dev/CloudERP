using API.Controllers.Branch;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Branch
{
    [TestFixture]
    public class BranchEmployeeApiControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IFileAdapterFactory> _fileAdapterFactoryMock;
        private BranchEmployeeApiController _controller;
        private List<Employee> _testEmployees;
        private Employee _testEmployee;
        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/EmployeePhoto/Default/default.png";

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _fileAdapterFactoryMock = new Mock<IFileAdapterFactory>();
            _controller = new BranchEmployeeApiController(
                _employeeRepositoryMock.Object,
                _fileServiceMock.Object,
                _fileAdapterFactoryMock.Object);
            _testEmployees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 1,
                    FullName = "John Doe",
                    ContactNumber = "1234567890",
                    Email = "john.doe@example.com",
                    Address = "123 Main St",
                    Photo = DEFAULT_EMPLOYEE_AVATAR_PATH,
                    TIN = "123456789",
                    Designation = "Manager",
                    Description = "Team lead",
                    MonthlySalary = 5000.0,
                    IsFirstLogin = true,
                    RegistrationDate = new DateTime(2025, 4, 1),
                    CompanyID = 1,
                    BranchID = 1,
                    BranchTypeID = 1,
                    BrchID = 101,
                    BranchName = "Branch1",
                    UserID = 1
                },
                new Employee
                {
                    EmployeeID = 2,
                    FullName = "Jane Smith",
                    ContactNumber = "0987654321",
                    Email = "jane.smith@example.com",
                    Address = "456 Oak St",
                    Photo = DEFAULT_EMPLOYEE_AVATAR_PATH,
                    TIN = "987654321",
                    Designation = "Developer",
                    Description = "Software engineer",
                    MonthlySalary = 4000.0,
                    IsFirstLogin = false,
                    RegistrationDate = new DateTime(2025, 4, 2),
                    CompanyID = 1,
                    BranchID = 1,
                    BranchTypeID = 1,
                    BrchID = 101,
                    BranchName = "Branch1",
                    UserID = 2
                }
            };
            _testEmployee = new Employee
            {
                EmployeeID = 3,
                FullName = "Alice Johnson",
                ContactNumber = "5555555555",
                Email = "alice.johnson@example.com",
                Address = "789 Pine St",
                Photo = DEFAULT_EMPLOYEE_AVATAR_PATH,
                TIN = "555555555",
                Designation = "Analyst",
                Description = "Data analyst",
                MonthlySalary = 4500.0,
                IsFirstLogin = true,
                RegistrationDate = new DateTime(2025, 4, 3),
                CompanyID = 2,
                BranchID = 2,
                BranchTypeID = 2,
                BrchID = 102,
                BranchName = "Branch2",
                UserID = 3
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenEmployeeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchEmployeeApiController(null, _fileServiceMock.Object, _fileAdapterFactoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFileServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchEmployeeApiController(_employeeRepositoryMock.Object, null, _fileAdapterFactoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFileAdapterFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BranchEmployeeApiController(_employeeRepositoryMock.Object, _fileServiceMock.Object, null));
        }

        [Test]
        public async Task Employee_ShouldReturnOkWithEmployees_WhenRepositorySucceeds()
        {
            int companyId = 1, branchId = 1;
            _employeeRepositoryMock.Setup(r => r.GetByBranchAsync(companyId, branchId))
                                   .ReturnsAsync(_testEmployees);
            var result = await _controller.Employee(companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testEmployees);
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task Employee_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.GetByBranchAsync(companyId, branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.Employee(companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.GetByBranchAsync(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.EmployeeRegistration(_testEmployee);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testEmployee.EmployeeID);
            createdResult.Value.Should().BeEquivalentTo(_testEmployee);
            _testEmployee.Photo.Should().Be(DEFAULT_EMPLOYEE_AVATAR_PATH);
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.Is<Employee>(e => e == _testEmployee)), Times.Once());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldSetDefaultPhoto_WhenPhotoIsNull()
        {
            var employeeWithoutPhoto = new Employee
            {
                EmployeeID = _testEmployee.EmployeeID,
                FullName = _testEmployee.FullName,
                ContactNumber = _testEmployee.ContactNumber,
                Email = _testEmployee.Email,
                Address = _testEmployee.Address,
                Photo = null,
                TIN = _testEmployee.TIN,
                Designation = _testEmployee.Designation,
                Description = _testEmployee.Description,
                MonthlySalary = _testEmployee.MonthlySalary,
                IsFirstLogin = _testEmployee.IsFirstLogin,
                RegistrationDate = _testEmployee.RegistrationDate,
                CompanyID = _testEmployee.CompanyID,
                BranchID = _testEmployee.BranchID,
                BranchTypeID = _testEmployee.BranchTypeID,
                BrchID = _testEmployee.BrchID,
                BranchName = _testEmployee.BranchName,
                UserID = _testEmployee.UserID
            };
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.EmployeeRegistration(employeeWithoutPhoto);
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            var returnedEmployee = createdResult.Value as Employee;
            returnedEmployee.Photo.Should().Be(DEFAULT_EMPLOYEE_AVATAR_PATH);
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.Is<Employee>(e => e.Photo == DEFAULT_EMPLOYEE_AVATAR_PATH)), Times.Once());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.EmployeeRegistration(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.EmployeeRegistration(_testEmployee);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnOkWithEmployee_WhenIdExists()
        {
            var id = 1;
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync(_testEmployees[0]);
            var result = await _controller.GetById(id);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testEmployees[0]);
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Only());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((Employee)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var id = 1;
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetById(id);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task EmployeeUpdation_ShouldReturnOk_WhenModelIsValid()
        {
            _employeeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.EmployeeUpdation(_testEmployee);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testEmployee);
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Employee>(e => e == _testEmployee)), Times.Once());
        }

        [Test]
        public async Task EmployeeUpdation_ShouldSetDefaultPhoto_WhenPhotoIsNull()
        {
            var employeeWithoutPhoto = new Employee
            {
                EmployeeID = _testEmployee.EmployeeID,
                FullName = _testEmployee.FullName,
                ContactNumber = _testEmployee.ContactNumber,
                Email = _testEmployee.Email,
                Address = _testEmployee.Address,
                Photo = null,
                TIN = _testEmployee.TIN,
                Designation = _testEmployee.Designation,
                Description = _testEmployee.Description,
                MonthlySalary = _testEmployee.MonthlySalary,
                IsFirstLogin = _testEmployee.IsFirstLogin,
                RegistrationDate = _testEmployee.RegistrationDate,
                CompanyID = _testEmployee.CompanyID,
                BranchID = _testEmployee.BranchID,
                BranchTypeID = _testEmployee.BranchTypeID,
                BrchID = _testEmployee.BrchID,
                BranchName = _testEmployee.BranchName,
                UserID = _testEmployee.UserID
            };
            _employeeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            var result = await _controller.EmployeeUpdation(employeeWithoutPhoto);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var returnedEmployee = okResult.Value as Employee;
            returnedEmployee.Photo.Should().Be(DEFAULT_EMPLOYEE_AVATAR_PATH);
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Employee>(e => e.Photo == DEFAULT_EMPLOYEE_AVATAR_PATH)), Times.Once());
        }

        [Test]
        public async Task EmployeeUpdation_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.EmployeeUpdation(null);
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task EmployeeUpdation_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.EmployeeUpdation(_testEmployee);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once());
        }
    }
}