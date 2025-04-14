using API.Controllers.General;
using Domain.Models;
using Domain.Models.FinancialModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Services.Facades;

namespace UnitTests.API.Controllers.General
{
    [TestFixture]
    public class HomeApiControllerTests
    {
        private Mock<HomeFacade> _homeFacadeMock;
        private Mock<IConfiguration> _configurationMock;
        private HomeApiController _controller;
        private User _testUser;
        private Employee _testEmployee;
        private Domain.Models.Company _testCompany;
        private DashboardModel _testDashboardModel;
        private Dictionary<string, object> _testExchangeRates;
        private LoginRequest _testLoginRequest;
        private ResetPasswordRequest _testResetPasswordRequest;

        [SetUp]
        public void SetUp()
        {
            _homeFacadeMock = new Mock<HomeFacade>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new HomeApiController(_homeFacadeMock.Object, _configurationMock.Object);

            _testUser = new User
            {
                UserID = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                ContactNo = "1234567890",
                UserName = "johndoe",
                Password = "hashedpassword",
                Salt = "somesalt",
                UserTypeID = 1,
                IsActive = true,
                UserTypeName = "Admin"
            };

            _testEmployee = new Employee
            {
                EmployeeID = 1,
                FullName = "John Doe",
                Photo = "photo.jpg",
                RegistrationDate = new DateTime(2024, 1, 1),
                Designation = "Manager",
                BranchID = 1,
                BranchTypeID = 1,
                BrchID = 1,
                CompanyID = 1
            };

            _testCompany = new Domain.Models.Company
            {
                CompanyID = 1,
                Name = "Test Company",
                Logo = "logo.png"
            };

            _testDashboardModel = new DashboardModel
            {
                CurrentMonthRevenue = 10000.0,
                CurrentMonthExpenses = 5000.0,
                NetIncome = 5000.0,
                CashPlusBankAccountBalance = 20000.0,
                TotalReceivable = 3000.0,
                TotalPayable = 2000.0,
                TotalSalesToday = 1000.0,
                TotalPurchasesToday = 500.0
            };

            _testExchangeRates = new Dictionary<string, object>
            {
                { "EUR", 1.1 },
                { "GBP", 1.3 }
            };

            _testLoginRequest = new LoginRequest
            {
                Email = "john.doe@example.com",
                Password = "password",
                RememberMe = true
            };

            _testResetPasswordRequest = new ResetPasswordRequest
            {
                ResetCode = "reset-code-123",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Налаштування конфігурації
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("audience");
            _configurationMock.Setup(c => c["CurrencyApi:DefaultCurrency"]).Returns("USD");
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFacadeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HomeApiController(null, _configurationMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HomeApiController(_homeFacadeMock.Object, null));
        }

        [Test]
        public async Task LoginUser_ShouldReturnOkWithLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password))
                           .ReturnsAsync(_testUser);
            _homeFacadeMock.Setup(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID))
                           .ReturnsAsync(_testEmployee);
            _homeFacadeMock.Setup(f => f.CompanyRepository.GetByIdAsync(_testEmployee.CompanyID))
                           .ReturnsAsync(_testCompany);

            // Act
            var result = await _controller.LoginUser(_testLoginRequest);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as LoginResponse;
            response.User.Should().BeEquivalentTo(_testUser, options => options.Excluding(u => u.BranchName).Excluding(u => u.ResetPasswordCode).Excluding(u => u.LastPasswordResetRequest).Excluding(u => u.ResetPasswordExpiration));
            response.Employee.Should().BeEquivalentTo(_testEmployee);
            response.Company.Should().BeEquivalentTo(_testCompany);
            response.Token.Should().NotBeNullOrEmpty();
            _homeFacadeMock.Verify(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password), Times.Once());
            _homeFacadeMock.Verify(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID), Times.Once());
            _homeFacadeMock.Verify(f => f.CompanyRepository.GetByIdAsync(_testEmployee.CompanyID), Times.Once());
        }

        [Test]
        public async Task LoginUser_ShouldReturnUnauthorized_WhenUserAuthenticationFails()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _controller.LoginUser(_testLoginRequest);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.Should().Be("User authentication failed");
            _homeFacadeMock.Verify(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password), Times.Once());
            _homeFacadeMock.Verify(f => f.EmployeeRepository.GetByUserIdAsync(It.IsAny<int>()), Times.Never());
            _homeFacadeMock.Verify(f => f.CompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task LoginUser_ShouldReturnUnauthorized_WhenEmployeeAuthenticationFails()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password))
                           .ReturnsAsync(_testUser);
            _homeFacadeMock.Setup(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID))
                           .ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.LoginUser(_testLoginRequest);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.Should().Be("Employee authentication failed");
            _homeFacadeMock.Verify(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password), Times.Once());
            _homeFacadeMock.Verify(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID), Times.Once());
            _homeFacadeMock.Verify(f => f.CompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task LoginUser_ShouldReturnUnauthorized_WhenCompanyAuthenticationFails()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password))
                           .ReturnsAsync(_testUser);
            _homeFacadeMock.Setup(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID))
                           .ReturnsAsync(_testEmployee);
            _homeFacadeMock.Setup(f => f.CompanyRepository.GetByIdAsync(_testEmployee.CompanyID))
                           .ReturnsAsync((Domain.Models.Company)null);

            // Act
            var result = await _controller.LoginUser(_testLoginRequest);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.Should().Be("Company authentication failed");
            _homeFacadeMock.Verify(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password), Times.Once());
            _homeFacadeMock.Verify(f => f.EmployeeRepository.GetByUserIdAsync(_testUser.UserID), Times.Once());
            _homeFacadeMock.Verify(f => f.CompanyRepository.GetByIdAsync(_testEmployee.CompanyID), Times.Once());
        }

        [Test]
        public async Task LoginUser_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Authentication error";
            _homeFacadeMock.Setup(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.LoginUser(_testLoginRequest);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.AuthService.AuthenticateUserAsync(_testLoginRequest.Email, _testLoginRequest.Password), Times.Once());
        }

        [Test]
        public async Task GetDashboardValues_ShouldReturnOkWithDashboardModel_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _homeFacadeMock.Setup(f => f.DashboardService.GetDashboardValues(branchId, companyId))
                           .ReturnsAsync(_testDashboardModel);

            // Act
            var result = await _controller.GetDashboardValues(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testDashboardModel);
            _homeFacadeMock.Verify(f => f.DashboardService.GetDashboardValues(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetDashboardValues_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Dashboard retrieval error";
            _homeFacadeMock.Setup(f => f.DashboardService.GetDashboardValues(branchId, companyId))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetDashboardValues(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.DashboardService.GetDashboardValues(branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetCurrencies_ShouldReturnOkWithCurrencies_WhenDataIsValid()
        {
            // Arrange
            //_homeFacadeMock.Setup(f => f.CurrencyService.GetExchangeRatesAsync("USD"))
            //               .ReturnsAsync(_testExchangeRates);

            // Act
            var result = await _controller.GetCurrencies();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var currencies = okResult.Value as Dictionary<string, decimal>;
            currencies.Should().ContainKey("EUR").WhoseValue.Should().Be(1.1m);
            currencies.Should().ContainKey("GBP").WhoseValue.Should().Be(1.3m);
            _homeFacadeMock.Verify(f => f.CurrencyService.GetExchangeRatesAsync("USD"), Times.Once());
        }

        [Test]
        public async Task GetCurrencies_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Currency retrieval error";
            _homeFacadeMock.Setup(f => f.CurrencyService.GetExchangeRatesAsync("USD"))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetCurrencies();

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.CurrencyService.GetExchangeRatesAsync("USD"), Times.Once());
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnOk_WhenEmailIsValid()
        {
            // Arrange
            var email = "john.doe@example.com";
            _homeFacadeMock.Setup(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                           .ReturnsAsync(false);
            _homeFacadeMock.Setup(f => f.UserRepository.GetByEmailAsync(email))
                           .ReturnsAsync(_testUser);
            _homeFacadeMock.Setup(f => f.UserRepository.UpdateAsync(It.IsAny<User>()))
                           .Returns(Task.CompletedTask);
            _homeFacadeMock.Setup(f => f.AuthService.SendPasswordResetEmailAsync(It.IsAny<string>(), email, It.IsAny<string>()));

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Password reset email sent.");
            _homeFacadeMock.Verify(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email), Times.Once());
            _homeFacadeMock.Verify(f => f.UserRepository.GetByEmailAsync(email), Times.Once());
            _homeFacadeMock.Verify(f => f.UserRepository.UpdateAsync(It.Is<User>(u => u.ResetPasswordCode != null && u.ResetPasswordExpiration != null)), Times.Once());
            _homeFacadeMock.Verify(f => f.AuthService.SendPasswordResetEmailAsync(It.IsAny<string>(), email, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenEmailIsEmpty()
        {
            // Arrange
            string email = null;

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _homeFacadeMock.Verify(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(It.IsAny<string>()), Times.Never());
            _homeFacadeMock.Verify(f => f.UserRepository.GetByEmailAsync(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenResetRecentlyRequested()
        {
            // Arrange
            var email = "john.doe@example.com";
            _homeFacadeMock.Setup(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                           .ReturnsAsync(true);

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _homeFacadeMock.Verify(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email), Times.Once());
            _homeFacadeMock.Verify(f => f.UserRepository.GetByEmailAsync(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnNotFound_WhenUserNotFound()
        {
            // Arrange
            var email = "john.doe@example.com";
            _homeFacadeMock.Setup(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                           .ReturnsAsync(false);
            _homeFacadeMock.Setup(f => f.UserRepository.GetByEmailAsync(email))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("User not found");
            _homeFacadeMock.Verify(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email), Times.Once());
            _homeFacadeMock.Verify(f => f.UserRepository.GetByEmailAsync(email), Times.Once());
            _homeFacadeMock.Verify(f => f.UserRepository.UpdateAsync(It.IsAny<User>()), Times.Never());
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var email = "john.doe@example.com";
            var exceptionMessage = "Password reset error";
            _homeFacadeMock.Setup(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.AuthService.IsPasswordResetRequestedRecentlyAsync(email), Times.Once());
        }

        [Test]
        public async Task GetResetPassword_ShouldReturnOk_WhenResetCodeIsValid()
        {
            // Arrange
            var resetCode = "reset-code-123";
            _homeFacadeMock.Setup(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()))
                           .ReturnsAsync(_testUser);

            // Act
            var result = await _controller.GetResetPassword(resetCode);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.ResetCode.Should().Be(resetCode);
            _homeFacadeMock.Verify(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()), Times.Once());
        }

        [Test]
        public async Task GetResetPassword_ShouldReturnNotFound_WhenResetCodeIsInvalid()
        {
            // Arrange
            var resetCode = "invalid-code";
            _homeFacadeMock.Setup(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetResetPassword(resetCode);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be("Model not found.");
            _homeFacadeMock.Verify(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()), Times.Once());
        }

        [Test]
        public async Task GetResetPassword_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var resetCode = "reset-code-123";
            var exceptionMessage = "Reset code retrieval error";
            _homeFacadeMock.Setup(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetResetPassword(resetCode);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.UserRepository.GetByPasswordCodesAsync(resetCode, It.IsAny<DateTime>()), Times.Once());
        }

        [Test]
        public async Task ResetPassword_ShouldReturnOk_WhenResetIsSuccessful()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ResetPassword(_testResetPasswordRequest);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Password reset successfully.");
            _homeFacadeMock.Verify(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword), Times.Once());
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenResetFails()
        {
            // Arrange
            _homeFacadeMock.Setup(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ResetPassword(_testResetPasswordRequest);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Passwords do not match or link expired.");
            _homeFacadeMock.Verify(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword), Times.Once());
        }

        [Test]
        public async Task ResetPassword_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Password reset error";
            _homeFacadeMock.Setup(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ResetPassword(_testResetPasswordRequest);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _homeFacadeMock.Verify(f => f.AuthService.ResetPasswordAsync(
                _testResetPasswordRequest.ResetCode,
                _testResetPasswordRequest.NewPassword,
                _testResetPasswordRequest.ConfirmPassword), Times.Once());
        }
    }
}