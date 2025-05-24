using API.Controllers.Company;
using API.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Domain.UtilsAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Facades;
using Utils.Helpers;

namespace UnitTests.API.Controllers.Company
{
    [TestFixture]
    public class CompanyRegistrationApiControllerTests
    {
        private Mock<CompanyRegistrationFacade> _companyRegistrationFacadeMock;
        private Mock<IPasswordHelper> _passwordHelperMock;
        private Mock<ICompanyRepository> _companyRepositoryMock;
        private Mock<IBranchRepository> _branchRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private CompanyRegistrationApiController _controller;
        private RegistrationMV _testRegistrationMV;
        private PasswordHelper _passwordHelper;
        private const string DEFAULT_COMPANY_LOGO_PATH = "~/CompanyLogo/erp-logo.png";
        private const string DEFAULT_EMPLOYEE_PHOTO_PATH = "~/EmployeePhoto/Default/default.png";

        [SetUp]
        public void SetUp()
        {
            _companyRegistrationFacadeMock = new Mock<CompanyRegistrationFacade>();
            _passwordHelperMock = new Mock<IPasswordHelper>();
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _branchRepositoryMock = new Mock<IBranchRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            // Setup facade properties
            _companyRegistrationFacadeMock.SetupGet(f => f.CompanyRepository).Returns(_companyRepositoryMock.Object);
            _companyRegistrationFacadeMock.SetupGet(f => f.BranchRepository).Returns(_branchRepositoryMock.Object);
            _companyRegistrationFacadeMock.SetupGet(f => f.UserRepository).Returns(_userRepositoryMock.Object);
            _companyRegistrationFacadeMock.SetupGet(f => f.EmployeeRepository).Returns(_employeeRepositoryMock.Object);
            _companyRegistrationFacadeMock.SetupGet(f => f.EmailService).Returns(_emailServiceMock.Object);

            _controller = new CompanyRegistrationApiController(_companyRegistrationFacadeMock.Object, _passwordHelperMock.Object);

            _testRegistrationMV = new RegistrationMV
            {
                CompanyName = "Test Company",
                BranchName = "Main Branch",
                BranchAddress = "123 Test St",
                BranchContact = "1234567890",
                EmployeeName = "John Doe",
                EmployeeEmail = "john.doe@example.com",
                EmployeeContactNo = "1234567890",
                EmployeeAddress = "456 Employee St",
                EmployeeTIN = "123456789",
                EmployeeDesignation = "Manager",
                EmployeeMonthlySalary = 5000,
                UserName = "johndoe"
            };
            _passwordHelper = new PasswordHelper();
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFacadeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyRegistrationApiController(null, _passwordHelperMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenPasswordHelperIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyRegistrationApiController(_companyRegistrationFacadeMock.Object, null));
        }

        [Test]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(_testRegistrationMV.EmployeeEmail))
                               .ReturnsAsync((User)null);
            _companyRepositoryMock.Setup(r => r.GetByNameAsync(_testRegistrationMV.CompanyName))
                                  .ReturnsAsync((Domain.Models.Company)null);
            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Company>()))
                                  .Callback<Domain.Models.Company>(c => c.CompanyID = 1)
                                  .Returns(Task.CompletedTask);
            _branchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()))
                                 .Callback<Domain.Models.Branch>(b => b.BranchID = 1)
                                 .Returns(Task.CompletedTask);
            _passwordHelperMock.Setup(p => _passwordHelper.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny))
                               .Returns("hashedPassword")
                               .Callback<string, string>((pwd, salt) => salt = "salt");
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                               .Callback<User>(u => u.UserID = 1)
                               .Returns(Task.CompletedTask);
            _employeeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                                   .Returns(Task.CompletedTask);
            _emailServiceMock.Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            var result = await _controller.Register(_testRegistrationMV);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Registration succeeded");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.Is<Domain.Models.Company>(c => c.Name == _testRegistrationMV.CompanyName && c.Logo == DEFAULT_COMPANY_LOGO_PATH)), Times.Once());
            _branchRepositoryMock.Verify(r => r.AddAsync(It.Is<Domain.Models.Branch>(b => b.BranchName == _testRegistrationMV.BranchName && b.BranchAddress == _testRegistrationMV.BranchAddress)), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == _testRegistrationMV.EmployeeEmail && u.UserName == _testRegistrationMV.UserName)), Times.Once());
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.Is<Employee>(e => e.FullName == _testRegistrationMV.EmployeeName && e.Photo == DEFAULT_EMPLOYEE_PHOTO_PATH)), Times.Once());
            _emailServiceMock.Verify(s => s.SendEmail(_testRegistrationMV.EmployeeEmail, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Register(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Incorrect details.");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Never());
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(_testRegistrationMV.EmployeeEmail))
                               .ReturnsAsync(new User { UserID = 1, Email = _testRegistrationMV.EmployeeEmail });

            // Act
            var result = await _controller.Register(_testRegistrationMV);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            conflictResult.Should().NotBeNull();
            conflictResult.StatusCode.Should().Be(409);
            conflictResult.Value.Should().Be("User with provided Email already exists.");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Never());
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task Register_ShouldReturnConflict_WhenCompanyNameAlreadyExists()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(_testRegistrationMV.EmployeeEmail))
                               .ReturnsAsync((User)null);
            _companyRepositoryMock.Setup(r => r.GetByNameAsync(_testRegistrationMV.CompanyName))
                                  .ReturnsAsync(new Domain.Models.Company { CompanyID = 1, Name = _testRegistrationMV.CompanyName });

            // Act
            var result = await _controller.Register(_testRegistrationMV);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            conflictResult.Should().NotBeNull();
            conflictResult.StatusCode.Should().Be(409);
            conflictResult.Value.Should().Be("Company with provided Name already exists.");
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Never());
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }

        [Test]
        public async Task Register_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Database error";
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(_testRegistrationMV.EmployeeEmail))
                               .ReturnsAsync((User)null);
            _companyRepositoryMock.Setup(r => r.GetByNameAsync(_testRegistrationMV.CompanyName))
                                  .ReturnsAsync((Domain.Models.Company)null);
            _companyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Models.Company>()))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Register(_testRegistrationMV);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Company>()), Times.Once());
            _branchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Branch>()), Times.Never());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
            _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never());
        }
    }
}