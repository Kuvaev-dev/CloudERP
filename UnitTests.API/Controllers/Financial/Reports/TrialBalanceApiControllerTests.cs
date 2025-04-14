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
    public class TrialBalanceApiControllerTests
    {
        private Mock<ITrialBalanceRepository> _trialBalanceRepositoryMock;
        private Mock<IFinancialYearRepository> _financialYearRepositoryMock;
        private TrialBalanceApiController _controller;
        private FinancialYear _testFinancialYear;
        private List<TrialBalanceModel> _testTrialBalanceModels;

        [SetUp]
        public void SetUp()
        {
            _trialBalanceRepositoryMock = new Mock<ITrialBalanceRepository>();
            _financialYearRepositoryMock = new Mock<IFinancialYearRepository>();
            _controller = new TrialBalanceApiController(
                _trialBalanceRepositoryMock.Object,
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

            _testTrialBalanceModels = new List<TrialBalanceModel>
            {
                new TrialBalanceModel
                {
                    FinancialYearID = 1,
                    AccountSubControl = "Cash",
                    AccountSubControlID = 101,
                    Debit = 10000.0,
                    Credit = 0.0,
                    BranchID = 1,
                    CompanyID = 1
                },
                new TrialBalanceModel
                {
                    FinancialYearID = 1,
                    AccountSubControl = "Sales",
                    AccountSubControlID = 102,
                    Debit = 0.0,
                    Credit = 5000.0,
                    BranchID = 1,
                    CompanyID = 1
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenTrialBalanceRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TrialBalanceApiController(null, _financialYearRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFinancialYearRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TrialBalanceApiController(_trialBalanceRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetTrialBalance_ShouldReturnOkWithTrialBalanceModels_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _trialBalanceRepositoryMock.Setup(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID))
                .ReturnsAsync(_testTrialBalanceModels);

            // Act
            var result = await _controller.GetTrialBalance(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTrialBalanceModels);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _trialBalanceRepositoryMock.Verify(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID), Times.Once());
        }

        [Test]
        public async Task GetTrialBalance_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Trial balance retrieval error";
            _financialYearRepositoryMock.Setup(r => r.GetSingleActiveAsync())
                                        .ReturnsAsync(_testFinancialYear);
            _trialBalanceRepositoryMock.Setup(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTrialBalance(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Once());
            _trialBalanceRepositoryMock.Verify(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                _testFinancialYear.FinancialYearID), Times.Once());
        }

        [Test]
        public async Task GetTrialBalanceByFinancialYear_ShouldReturnOkWithTrialBalanceModels_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            _trialBalanceRepositoryMock.Setup(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                financialYearId))
                .ReturnsAsync(_testTrialBalanceModels);

            // Act
            var result = await _controller.GetTrialBalanceByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testTrialBalanceModels);
            _trialBalanceRepositoryMock.Verify(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                financialYearId), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }

        [Test]
        public async Task GetTrialBalanceByFinancialYear_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1, financialYearId = 1;
            var exceptionMessage = "Trial balance retrieval error";
            _trialBalanceRepositoryMock.Setup(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                financialYearId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTrialBalanceByFinancialYear(companyId, branchId, financialYearId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _trialBalanceRepositoryMock.Verify(r => r.GetTrialBalanceAsync(
                companyId,
                branchId,
                financialYearId), Times.Once());
            _financialYearRepositoryMock.Verify(r => r.GetSingleActiveAsync(), Times.Never());
        }
    }
}