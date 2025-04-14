using API.Controllers.Financial.Reports;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Financial.Reports
{
    [TestFixture]
    public class LedgerApiControllerTests
    {
        private Mock<ILedgerRepository> _ledgerRepositoryMock;
        private Mock<IFinancialYearRepository> _financialYearRepositoryMock;
        private LedgerApiController _controller;
        private FinancialYear _testFinancialYear;
        private List<AccountLedgerModel> _testLedgerModels;
        private List<FinancialYear> _testFinancialYears;

        [SetUp]
        public void SetUp()
        {
            _ledgerRepositoryMock = new Mock<ILedgerRepository>();
            _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
            _controller = new LedgerApiController(
                _ledgerRepositoryMock.Object,
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

            _testLedgerModels = new List<AccountLedgerModel>
            {
                new AccountLedgerModel
                {
                    SNo = 1,
                    Account = "Cash",
                    Date = "2025-01-01",
                    Description = "Initial deposit",
                    Debit = "10000.00",
                    Credit = "0.00"
                },
                new AccountLedgerModel
                {
                    SNo = 2,
                    Account = "Sales",
                    Date = "2025-01-02",
                    Description = "Product sale",
                    Debit = "0.00",
                    Credit = "5000.00"
                }
            };

            _testFinancialYears = new List<FinancialYear>
            {
                _testFinancialYear,
                new FinancialYear
                {
                    FinancialYearID = 2,
                    FinancialYearName = "2026",
                    StartDate = new DateTime(2026, 1, 1),
                    EndDate = new DateTime(2026, 12, 31),
                    IsActive = false,
                    UserID = 2,
                    UserName = "test_user2"
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenLedgerRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LedgerApiController(null, _financialYearRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFinancialYearRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LedgerApiController(_ledgerRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetLedger_ShouldReturnOkWithLedgerModels_WhenFinancialYearExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _financialYearRepositoryMock.Setup(r => r.GetAllActiveAsync())
                                        .ReturnsAsync(_testFinancialYears);
            _ledgerRepositoryMock.Setup(r => r.GetLedgerAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID))
                .ReturnsAsync(_testLedgerModels);

            // Act
            var result = await _controller.GetLedger(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testLedgerModels);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once());
            _ledgerRepositoryMock.Verify(r => r.GetLedgerAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID), Times.Once());
        }

        [Test]
        public async Task GetLedger_ShouldReturnOkWithEmptyList_WhenNoActiveFinancialYear()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync((FinancialYear)null);
            _financialYearRepositoryMock.Setup(r => r.GetAllActiveAsync())
                                        .ReturnsAsync(_testFinancialYears);

            // Act
            var result = await _controller.GetLedger(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(new List<AccountLedgerModel>());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once());
            _ledgerRepositoryMock.Verify(r => r.GetLedgerAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task GetLedger_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Ledger retrieval error";
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _financialYearRepositoryMock.Setup(r => r.GetAllActiveAsync())
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetLedger(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once());
            _ledgerRepositoryMock.Verify(r => r.GetLedgerAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task GetLedgerByFinancialYear_ShouldReturnOkWithLedgerModels_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetAllActiveAsync())
                                        .ReturnsAsync(_testFinancialYears);
            _ledgerRepositoryMock.Setup(r => r.GetLedgerAsync(
                companyId,
                branchId,
                financialYearId))
                .ReturnsAsync(_testLedgerModels);

            // Act
            var result = await _controller.GetLedgerByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testLedgerModels);
            _financialYearRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once());
            _ledgerRepositoryMock.Verify(r => r.GetLedgerAsync(
                companyId,
                branchId,
                financialYearId), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }

        [Test]
        public async Task GetLedgerByFinancialYear_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            var exceptionMessage = "Ledger retrieval error";
            _financialYearRepositoryMock.Setup(r => r.GetAllActiveAsync())
                                        .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetLedgerByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetAllActiveAsync(), Times.Once());
            _ledgerRepositoryMock.Verify(r => r.GetLedgerAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }
    }
}