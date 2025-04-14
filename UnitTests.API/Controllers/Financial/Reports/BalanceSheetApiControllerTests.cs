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
    public class BalanceSheetApiControllerTests
    {
        private Mock<IBalanceSheetService> _balanceSheetServiceMock;
        private Mock<IFinancialYearRepository> _financialYearRepositoryMock;
        private Mock<IAccountHeadRepository> _accountHeadRepositoryMock;
        private BalanceSheetApiController _controller;
        private FinancialYear _testFinancialYear;
        private BalanceSheetModel _testBalanceSheet;
        private List<int> _testAccountHeadIds;

        [SetUp]
        public void SetUp()
        {
            _balanceSheetServiceMock = new Mock<IBalanceSheetService>();
            _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
            _accountHeadRepositoryMock = new Mock<IAccountHeadRepository>();
            _controller = new BalanceSheetApiController(
                _balanceSheetServiceMock.Object,
                _financialYearRepositoryMock.Object,
                _accountHeadRepositoryMock.Object);

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

            _testBalanceSheet = new BalanceSheetModel
            {
                Title = "Balance Sheet 2025",
                TotalAssets = 100000.0,
                ReturnEarning = 20000.0,
                Total_Liabilities_OwnerEquity_ReturnEarning = 120000.0,
                AccountHeadTotals = new List<AccountHeadTotal>
                {
                    new AccountHeadTotal
                    {
                        AccountHeadTitle = "Assets",
                        TotalAmount = 100000.0,
                        AccountHeadDetails = new List<AccountHeadDetail>
                        {
                            new AccountHeadDetail { AccountSubTitle = "Cash", TotalAmount = 50000.0, Status = "Active" },
                            new AccountHeadDetail { AccountSubTitle = "Inventory", TotalAmount = 50000.0, Status = "Active" }
                        }
                    },
                    new AccountHeadTotal
                    {
                        AccountHeadTitle = "Liabilities",
                        TotalAmount = 50000.0,
                        AccountHeadDetails = new List<AccountHeadDetail>
                        {
                            new AccountHeadDetail { AccountSubTitle = "Loans", TotalAmount = 50000.0, Status = "Active" }
                        }
                    }
                }
            };

            _testAccountHeadIds = new List<int> { 1, 2, 3 };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenBalanceSheetServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BalanceSheetApiController(null, _financialYearRepositoryMock.Object, _accountHeadRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFinancialYearRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BalanceSheetApiController(_balanceSheetServiceMock.Object, null, _accountHeadRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenAccountHeadRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BalanceSheetApiController(_balanceSheetServiceMock.Object, _financialYearRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetBalanceSheet_ShouldReturnOkWithBalanceSheet_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _accountHeadRepositoryMock.Setup(r => r.GetAllIdsAsync())
                                      .ReturnsAsync(_testAccountHeadIds);
            _balanceSheetServiceMock.Setup(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID,
                _testAccountHeadIds))
                .ReturnsAsync(_testBalanceSheet);

            // Act
            var result = await _controller.GetBalanceSheet(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBalanceSheet);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _accountHeadRepositoryMock.Verify(r => r.GetAllIdsAsync(), Times.Once());
            _balanceSheetServiceMock.Verify(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID,
                _testAccountHeadIds), Times.Once());
        }

        [Test]
        public async Task GetBalanceSheet_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Balance sheet generation error";
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _accountHeadRepositoryMock.Setup(r => r.GetAllIdsAsync())
                                      .ReturnsAsync(_testAccountHeadIds);
            _balanceSheetServiceMock.Setup(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID,
                _testAccountHeadIds))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetBalanceSheet(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _accountHeadRepositoryMock.Verify(r => r.GetAllIdsAsync(), Times.Once());
            _balanceSheetServiceMock.Verify(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID,
                _testAccountHeadIds), Times.Once());
        }

        [Test]
        public async Task GetBalanceSheetByFinancialYear_ShouldReturnOkWithBalanceSheet_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            _accountHeadRepositoryMock.Setup(r => r.GetAllIdsAsync())
                                      .ReturnsAsync(_testAccountHeadIds);
            _balanceSheetServiceMock.Setup(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                financialYearId,
                _testAccountHeadIds))
                .ReturnsAsync(_testBalanceSheet);

            // Act
            var result = await _controller.GetBalanceSheetByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testBalanceSheet);
            _accountHeadRepositoryMock.Verify(r => r.GetAllIdsAsync(), Times.Once());
            _balanceSheetServiceMock.Verify(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                financialYearId,
                _testAccountHeadIds), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }

        [Test]
        public async Task GetBalanceSheetByFinancialYear_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            var exceptionMessage = "Balance sheet generation error";
            _accountHeadRepositoryMock.Setup(r => r.GetAllIdsAsync())
                                      .ReturnsAsync(_testAccountHeadIds);
            _balanceSheetServiceMock.Setup(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                financialYearId,
                _testAccountHeadIds))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetBalanceSheetByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _accountHeadRepositoryMock.Verify(r => r.GetAllIdsAsync(), Times.Once());
            _balanceSheetServiceMock.Verify(s => s.GetBalanceSheetAsync(
                companyId,
                branchId,
                financialYearId,
                _testAccountHeadIds), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }
    }
}