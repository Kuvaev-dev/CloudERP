using API.Controllers.Company;
using API.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Services.Facades;

namespace UnitTests.API.Controllers.Company
{
    [TestFixture]
    public class CompanyEmployeeApiControllerTests
    {
        private Mock<CompanyEmployeeFacade> _companyEmployeeFacadeMock;
        private Mock<IFileAdapterFactory> _fileAdapterFactoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IEmployeeSalaryService> _employeeSalaryServiceMock;
        private Mock<IPayrollRepository> _payrollRepositoryMock;
        private CompanyEmployeeApiController _controller;
        private List<Employee> _testEmployees;
        private Employee _testEmployee;
        private SalaryMV _testSalaryMV;
        private List<Payroll> _testPayrolls;
        private Payroll _testPayroll;
        private const string DEFAULT_PHOTO_PATH = "~/EmployeePhoto/Default/default.png";

        [SetUp]
        public void SetUp()
        {
            _companyEmployeeFacadeMock = new Mock<CompanyEmployeeFacade>();
            _fileAdapterFactoryMock = new Mock<IFileAdapterFactory>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _employeeSalaryServiceMock = new Mock<IEmployeeSalaryService>();
            _payrollRepositoryMock = new Mock<IPayrollRepository>();

            // Setup facade properties
            _companyEmployeeFacadeMock.SetupGet(f => f.EmployeeRepository).Returns(_employeeRepositoryMock.Object);
            _companyEmployeeFacadeMock.SetupGet(f => f.EmailService).Returns(_emailServiceMock.Object);
            _companyEmployeeFacadeMock.SetupGet(f => f.EmployeeSalaryService).Returns(_employeeSalaryServiceMock.Object);
            _companyEmployeeFacadeMock.SetupGet(f => f.PayrollRepository).Returns(_payrollRepositoryMock.Object);

            _controller = new CompanyEmployeeApiController(_companyEmployeeFacadeMock.Object, _fileAdapterFactoryMock.Object);

            _testEmployees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 1,
                    FullName = "John Doe",
                    ContactNumber = "1234567890",
                    Email = "john.doe@example.com",
                    Address = "123 Main St",
                    Photo = DEFAULT_PHOTO_PATH,
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
                    Photo = DEFAULT_PHOTO_PATH,
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
                Photo = null,
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

            _testSalaryMV = new SalaryMV
            {
                EmployeeID = 3,
                EmployeeName = "Alice Johnson",
                TIN = "555555555",
                Designation = "Analyst",
                TransferAmount = 4500.0,
                SalaryMonth = "April",
                SalaryYear = "2025",
                BonusPercentage = 10.0,
                TotalAmount = 4950.0,
                CompanyID = 2,
                BranchID = 2,
                UserID = 3
            };

            _testPayrolls = new List<Payroll>
            {
                new Payroll
                {
                    PayrollID = 1,
                    EmployeeID = 1,
                    EmployeeName = "John Doe",
                    BranchID = 1,
                    BranchName = "Branch1",
                    BranchAddress = "123 Branch St",
                    BranchContact = "1234567890",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    CompanyLogo = "~/logos/company1.png",
                    TransferAmount = 5000.0,
                    PayrollInvoiceNo = "INV-001",
                    PaymentDate = new DateTime(2025, 3, 31),
                    SalaryMonth = "March",
                    SalaryYear = "2025",
                    UserID = 1,
                    UserName = "john_doe"
                },
                new Payroll
                {
                    PayrollID = 2,
                    EmployeeID = 2,
                    EmployeeName = "Jane Smith",
                    BranchID = 1,
                    BranchName = "Branch1",
                    BranchAddress = "123 Branch St",
                    BranchContact = "1234567890",
                    CompanyID = 1,
                    CompanyName = "Company1",
                    CompanyLogo = "~/logos/company1.png",
                    TransferAmount = 4000.0,
                    PayrollInvoiceNo = "INV-002",
                    PaymentDate = new DateTime(2025, 3, 31),
                    SalaryMonth = "March",
                    SalaryYear = "2025",
                    UserID = 2,
                    UserName = "jane_smith"
                }
            };

            _testPayroll = new Payroll
            {
                PayrollID = 3,
                EmployeeID = 3,
                EmployeeName = "Alice Johnson",
                BranchID = 2,
                BranchName = "Branch2",
                BranchAddress = "789 Branch St",
                BranchContact = "5555555555",
                CompanyID = 2,
                CompanyName = "Company2",
                CompanyLogo = "~/logos/company2.png",
                TransferAmount = 4950.0,
                PayrollInvoiceNo = "INV-003",
                PaymentDate = new DateTime(2025, 4, 30),
                SalaryMonth = "April",
                SalaryYear = "2025",
                UserID = 3,
                UserName = "alice_johnson"
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenDependenciesAreProvided()
        {
            var controller = new CompanyEmployeeApiController(_companyEmployeeFacadeMock.Object, _fileAdapterFactoryMock.Object);
            controller.Should().NotBeNull();
        }

        [Test]
        public async Task GetAll_ShouldReturnOkWithEmployees_WhenRepositorySucceeds()
        {
            int companyId = 1;
            _employeeRepositoryMock.Setup(r => r.GetByCompanyIdAsync(companyId))
                                   .ReturnsAsync(_testEmployees);
            var result = await _controller.GetAll(companyId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testEmployees);
            _employeeRepositoryMock.Verify(r => r.GetByCompanyIdAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetAll_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int companyId = 1;
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.GetByCompanyIdAsync(companyId))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetAll(companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.GetByCompanyIdAsync(companyId), Times.Once());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnCreatedAtAction_WhenModelIsValid()
        {
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "model", JsonConvert.SerializeObject(_testEmployee) }
            });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Form = formCollection } }
            };
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            _emailServiceMock.Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _controller.EmployeeRegistration();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(_testEmployee.EmployeeID);
            var returnedEmployee = createdResult.Value as Employee;
            returnedEmployee.Should().BeEquivalentTo(_testEmployee, options => options.Excluding(e => e.Photo));
            returnedEmployee.Photo.Should().Be(DEFAULT_PHOTO_PATH);
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.Is<Employee>(e => e.Photo == DEFAULT_PHOTO_PATH)), Times.Once());
            _emailServiceMock.Verify(s => s.SendEmail(_testEmployee.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnBadRequest_WhenFormIsMissing()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()) } }
            };
            var result = await _controller.EmployeeRegistration();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnBadRequest_WhenModelIsInvalidJson()
        {
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "model", "invalid json" }
            });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Form = formCollection } }
            };
            var result = await _controller.EmployeeRegistration();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid employee data format.");
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task EmployeeRegistration_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "model", JsonConvert.SerializeObject(_testEmployee) }
            });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Request = { Form = formCollection } }
            };
            var exceptionMessage = "Database error";
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.EmployeeRegistration();
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
            _employeeRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var id = 999;
            _employeeRepositoryMock.Setup(r => r.GetByIdAsync(id))
                                   .ReturnsAsync((Employee)null);
            var result = await _controller.GetById(id);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
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
        public async Task ProcessSalary_ShouldReturnOkWithSalaryMV_WhenEmployeeExists()
        {
            var tin = "555555555";
            _employeeRepositoryMock.Setup(r => r.GetByTINAsync(tin))
                                   .ReturnsAsync(_testEmployee);
            var result = await _controller.ProcessSalary(tin);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var salaryMV = okResult.Value as SalaryMV;
            salaryMV.Should().NotBeNull();
            salaryMV.EmployeeID.Should().Be(_testEmployee.EmployeeID);
            salaryMV.EmployeeName.Should().Be(_testEmployee.FullName);
            salaryMV.TIN.Should().Be(_testEmployee.TIN);
            salaryMV.Designation.Should().Be(_testEmployee.Designation);
            salaryMV.TransferAmount.Should().Be(_testEmployee.MonthlySalary);
            _employeeRepositoryMock.Verify(r => r.GetByTINAsync(tin), Times.Once());
        }

        [Test]
        public async Task ProcessSalary_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            var tin = "999999999";
            _employeeRepositoryMock.Setup(r => r.GetByTINAsync(tin))
                                   .ReturnsAsync((Employee)null);
            var result = await _controller.ProcessSalary(tin);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be($"Employee with TIN {tin} not found.");
            _employeeRepositoryMock.Verify(r => r.GetByTINAsync(tin), Times.Once());
        }

        [Test]
        public async Task ConfirmSalary_ShouldReturnOkWithMessage_WhenSalaryIsConfirmed()
        {
            var message = "Salary confirmed successfully";
            _employeeSalaryServiceMock.Setup(s => s.ConfirmSalaryAsync(It.IsAny<Salary>(), _testSalaryMV.UserID, _testSalaryMV.BranchID, _testSalaryMV.CompanyID))
                                      .ReturnsAsync(message);
            var result = await _controller.ConfirmSalary(_testSalaryMV);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be(message);
            _employeeSalaryServiceMock.Verify(s => s.ConfirmSalaryAsync(It.Is<Salary>(sal => sal.EmployeeID == _testSalaryMV.EmployeeID &&
                                                                                          sal.SalaryMonth == _testSalaryMV.SalaryMonth &&
                                                                                          sal.SalaryYear == _testSalaryMV.SalaryYear &&
                                                                                          sal.TransferAmount == _testSalaryMV.TotalAmount),
                                                                        _testSalaryMV.UserID, _testSalaryMV.BranchID, _testSalaryMV.CompanyID), Times.Once());
        }

        [Test]
        public async Task ConfirmSalary_ShouldReturnProblem_WhenServiceThrowsException()
        {
            var exceptionMessage = "Salary confirmation error";
            _employeeSalaryServiceMock.Setup(s => s.ConfirmSalaryAsync(It.IsAny<Salary>(), _testSalaryMV.UserID, _testSalaryMV.BranchID, _testSalaryMV.CompanyID))
                                      .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.ConfirmSalary(_testSalaryMV);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeSalaryServiceMock.Verify(s => s.ConfirmSalaryAsync(It.IsAny<Salary>(), _testSalaryMV.UserID, _testSalaryMV.BranchID, _testSalaryMV.CompanyID), Times.Once());
        }

        [Test]
        public async Task GetSalaryHistory_ShouldReturnOkWithPayrolls_WhenRepositorySucceeds()
        {
            int branchId = 1, companyId = 1;
            _payrollRepositoryMock.Setup(r => r.GetSalaryHistoryAsync(branchId, companyId))
                                  .ReturnsAsync(_testPayrolls);
            var result = await _controller.GetSalaryHistory(branchId, companyId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testPayrolls);
            _payrollRepositoryMock.Verify(r => r.GetSalaryHistoryAsync(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetSalaryHistory_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            int branchId = 1, companyId = 1;
            var exceptionMessage = "Database error";
            _payrollRepositoryMock.Setup(r => r.GetSalaryHistoryAsync(branchId, companyId))
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetSalaryHistory(branchId, companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _payrollRepositoryMock.Verify(r => r.GetSalaryHistoryAsync(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetLatestSalaryInvoice_ShouldReturnOkWithPayroll_WhenInvoiceExists()
        {
            _payrollRepositoryMock.Setup(r => r.GetLatestPayrollAsync())
                                  .ReturnsAsync(_testPayroll);
            var result = await _controller.GetLatestSalaryInvoice();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testPayroll);
            _payrollRepositoryMock.Verify(r => r.GetLatestPayrollAsync(), Times.Once());
        }

        [Test]
        public async Task GetLatestSalaryInvoice_ShouldReturnBadRequest_WhenInvoiceDoesNotExist()
        {
            _payrollRepositoryMock.Setup(r => r.GetLatestPayrollAsync())
                                  .ReturnsAsync((Payroll)null);
            var result = await _controller.GetLatestSalaryInvoice();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model not found.");
            _payrollRepositoryMock.Verify(r => r.GetLatestPayrollAsync(), Times.Once());
        }

        [Test]
        public async Task GetLatestSalaryInvoice_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            var exceptionMessage = "Database error";
            _payrollRepositoryMock.Setup(r => r.GetLatestPayrollAsync())
                                  .ThrowsAsync(new Exception(exceptionMessage));
            var result = await _controller.GetLatestSalaryInvoice();
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _payrollRepositoryMock.Verify(r => r.GetLatestPayrollAsync(), Times.Once());
        }
    }
}