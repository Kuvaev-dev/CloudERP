using API.Controllers.Financial.Transactions;
using API.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Financial.Transactions
{
    [TestFixture]
    public class GeneralTransactionApiControllerTests
    {
        private Mock<IGeneralTransactionService> _generalTransactionServiceMock;
        private Mock<IGeneralTransactionRepository> _generalTransactionRepositoryMock;
        private GeneralTransactionApiController _controller;
        private GeneralTransactionMV _testTransactionMV;
        private List<JournalModel> _testJournalModels;
        private List<AllAccountModel> _testAccountModels;

        [SetUp]
        public void SetUp()
        {
            _generalTransactionServiceMock = new Mock<IGeneralTransactionService>();
            _generalTransactionRepositoryMock = new Mock<IGeneralTransactionRepository>();
            _controller = new GeneralTransactionApiController(
                _generalTransactionServiceMock.Object,
                _generalTransactionRepositoryMock.Object);

            _testTransactionMV = new GeneralTransactionMV
            {
                DebitAccountControlID = 101,
                CreditAccountControlID = 102,
                TransferAmount = 5000.0f,
                Reason = "Test transaction"
            };

            _testJournalModels = new List<JournalModel>
            {
                new JournalModel
                {
                    SNO = 1,
                    TransectionDate = new DateTime(2025, 1, 1),
                    AccountSubControl = "Cash",
                    TransectionTitle = "Deposit",
                    AccountSubControlID = 101,
                    InvoiceNo = "INV001",
                    Debit = 5000.0,
                    Credit = 0.0
                },
                new JournalModel
                {
                    SNO = 2,
                    TransectionDate = new DateTime(2025, 1, 2),
                    AccountSubControl = "Sales",
                    TransectionTitle = "Sale",
                    AccountSubControlID = 102,
                    InvoiceNo = "INV002",
                    Debit = 0.0,
                    Credit = 5000.0
                }
            };

            _testAccountModels = new List<AllAccountModel>
            {
                new AllAccountModel
                {
                    AccountHeadID = 1,
                    AccountHeadName = "Assets",
                    AccountControlID = 101,
                    AccountControlName = "Cash",
                    BranchID = 1,
                    CompanyID = 1,
                    AccountSubControlID = 101,
                    AccountSubControl = "Cash"
                },
                new AllAccountModel
                {
                    AccountHeadID = 2,
                    AccountHeadName = "Revenue",
                    AccountControlID = 102,
                    AccountControlName = "Sales",
                    BranchID = 1,
                    CompanyID = 1,
                    AccountSubControlID = 102,
                    AccountSubControl = "Sales"
                }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GeneralTransactionApiController(null, _generalTransactionRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GeneralTransactionApiController(_generalTransactionServiceMock.Object, null));
        }

        [Test]
        public async Task SaveTransaction_ShouldReturnOkWithMessage_WhenTransactionIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var successMessage = "Transaction confirmed successfully";
            _generalTransactionServiceMock.Setup(s => s.ConfirmTransactionAsync(
                _testTransactionMV.TransferAmount,
                userId,
                branchId,
                companyId,
                _testTransactionMV.DebitAccountControlID,
                _testTransactionMV.CreditAccountControlID,
                _testTransactionMV.Reason))
                .ReturnsAsync(successMessage);

            // Act
            var result = await _controller.SaveTransaction(_testTransactionMV, companyId, branchId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.Message.Should().Be(successMessage);
            _generalTransactionServiceMock.Verify(s => s.ConfirmTransactionAsync(
                _testTransactionMV.TransferAmount,
                userId,
                branchId,
                companyId,
                _testTransactionMV.DebitAccountControlID,
                _testTransactionMV.CreditAccountControlID,
                _testTransactionMV.Reason), Times.Once());
        }

        [Test]
        public async Task SaveTransaction_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;

            // Act
            var result = await _controller.SaveTransaction(null, companyId, branchId, userId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Model cannot be null.");
            _generalTransactionServiceMock.Verify(s => s.ConfirmTransactionAsync(
                It.IsAny<float>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task SaveTransaction_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var exceptionMessage = "Transaction processing error";
            _generalTransactionServiceMock.Setup(s => s.ConfirmTransactionAsync(
                _testTransactionMV.TransferAmount,
                userId,
                branchId,
                companyId,
                _testTransactionMV.DebitAccountControlID,
                _testTransactionMV.CreditAccountControlID,
                _testTransactionMV.Reason))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.SaveTransaction(_testTransactionMV, companyId, branchId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _generalTransactionServiceMock.Verify(s => s.ConfirmTransactionAsync(
                _testTransactionMV.TransferAmount,
                userId,
                branchId,
                companyId,
                _testTransactionMV.DebitAccountControlID,
                _testTransactionMV.CreditAccountControlID,
                _testTransactionMV.Reason), Times.Once());
        }

        [Test]
        public async Task GetJournal_ShouldReturnOkWithJournalModels_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var fromDate = new DateTime(2025, 1, 1);
            var toDate = new DateTime(2025, 1, 31);
            _generalTransactionRepositoryMock.Setup(r => r.GetJournal(companyId, branchId, fromDate, toDate))
                                             .ReturnsAsync(_testJournalModels);

            // Act
            var result = await _controller.GetJournal(companyId, branchId, fromDate, toDate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testJournalModels);
            _generalTransactionRepositoryMock.Verify(r => r.GetJournal(companyId, branchId, fromDate, toDate), Times.Once());
        }

        [Test]
        public async Task GetJournal_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var fromDate = new DateTime(2025, 1, 1);
            var toDate = new DateTime(2025, 1, 31);
            var exceptionMessage = "Journal retrieval error";
            _generalTransactionRepositoryMock.Setup(r => r.GetJournal(companyId, branchId, fromDate, toDate))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetJournal(companyId, branchId, fromDate, toDate);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _generalTransactionRepositoryMock.Verify(r => r.GetJournal(companyId, branchId, fromDate, toDate), Times.Once());
        }

        [Test]
        public async Task GetAccounts_ShouldReturnOkWithAccountModels_WhenDataIsValid()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _generalTransactionRepositoryMock.Setup(r => r.GetAllAccounts(companyId, branchId))
                                             .ReturnsAsync(_testAccountModels);

            // Act
            var result = await _controller.GetAccounts(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testAccountModels);
            _generalTransactionRepositoryMock.Verify(r => r.GetAllAccounts(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetAccounts_ShouldReturnProblem_WhenRepositoryThrowsException()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Accounts retrieval error";
            _generalTransactionRepositoryMock.Setup(r => r.GetAllAccounts(companyId, branchId))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAccounts(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _generalTransactionRepositoryMock.Verify(r => r.GetAllAccounts(companyId, branchId), Times.Once());
        }
    }
}