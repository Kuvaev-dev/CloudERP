using API.Controllers.Financial.Reports;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Financial.Reports
{
    [TestFixture]
    public class IncomeStatementApiControllerTests
    {
        private Mock<IIncomeStatementService> _incomeStatementServiceMock;
        private Mock<IFinancialYearRepository> _financialYearRepositoryMock;
        private IncomeStatementApiController _controller;
        private FinancialYear _testFinancialYear;
        private IncomeStatementModel _testIncomeStatement;

        [SetUp]
        public void SetUp()
        {
            _incomeStatementServiceMock = new Mock<IIncomeStatementService>();
            _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
            _controller = new IncomeStatementApiController(
                _incomeStatementServiceMock.Object,
                _financialYearRepositoryMock.Object);

            _testFinancialYear = new FinancialYear
            {
                FinancialYearID = 1,
                FinancialYearName = "2025",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                IsActive = true,
                UserID = 1,
                UserName = "test_user"
            };

            _testIncomeStatement = new IncomeStatementModel
            {
                Title = "Income Statement 2025",
                NetIncome = 50000.0,
                IncomeStatementHeads = new List<IncomeStatementHead>
                {
                    new IncomeStatementHead
                    {
                        Title = "Revenue",
                        TotalAmount = 100000.0,
                        AccountHead = new AccountHeadTotal
                        {
                            AccountHeadTitle = "Sales",
                            TotalAmount = 100000.0,
                            AccountHeadDetails = new List<AccountHeadDetail>
                            {
                                new AccountHeadDetail { AccountSubTitle = "Product Sales", TotalAmount = 80000.0, Status = "Active" },
                                new AccountHeadDetail { AccountSubTitle = "Service Sales", TotalAmount = 20000.0, Status = "Active" }
                            }
                        }
                    },
                    new IncomeStatementHead
                    {
                        Title = "Expenses",
                        TotalAmount = 50000.0,
                        AccountHead = new AccountHeadTotal
                        {
                            AccountHeadTitle = "Operating Expenses",
                            TotalAmount = 50000.0,
                            AccountHeadDetails = new List<AccountHeadDetail>
                            {
                                new AccountHeadDetail { AccountSubTitle = "Salaries", TotalAmount = 30000.0, Status = "Active" },
                                new AccountHeadDetail { AccountSubTitle = "Rent", TotalAmount = 20000.0, Status = "Active" }
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenIncomeStatementServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new IncomeStatementApiController(null, _financialYearRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFinancialYearRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new IncomeStatementApiController(_incomeStatementServiceMock.Object, null));
        }

        [Test]
        public async Task GetIncomeStatement_ShouldReturnOkWithIncomeStatement_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _incomeStatementServiceMock.Setup(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID))
                .ReturnsAsync(_testIncomeStatement);

            // Act
            var result = await _controller.GetIncomeStatement(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testIncomeStatement);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _incomeStatementServiceMock.Verify(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID), Times.Once());
        }

        [Test]
        public async Task GetIncomeStatement_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Income statement generation error";
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _incomeStatementServiceMock.Setup(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetIncomeStatement(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _incomeStatementServiceMock.Verify(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID), Times.Once());
        }

        [Test]
        public async Task GetIncomeStatementByFinancialYear_ShouldReturnOkWithIncomeStatement_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _incomeStatementServiceMock.Setup(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                financialYearId))
                .ReturnsAsync(_testIncomeStatement);

            // Act
            var result = await _controller.GetIncomeStatementByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testIncomeStatement);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _incomeStatementServiceMock.Verify(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                financialYearId), Times.Once());
        }

        [Test]
        public async Task GetIncomeStatementByFinancialYear_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            var exceptionMessage = "Income statement generation error";
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _incomeStatementServiceMock.Setup(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                financialYearId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetIncomeStatementByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _incomeStatementServiceMock.Verify(s => s.GetIncomeStatementAsync(
                companyId,
                branchId,
                financialYearId), Times.Once());
        }
    }
}