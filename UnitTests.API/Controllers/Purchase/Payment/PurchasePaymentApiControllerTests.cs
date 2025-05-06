using API.Controllers.Purchase.Payment;
using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Purchase.Payment
{
    [TestFixture]
    public class PurchasePaymentApiControllerTests
    {
        private Mock<PurchasePaymentFacade> _purchasePaymentFacadeMock;
        private Mock<IPurchasePaymentService> _purchasePaymentServiceMock;
        private PurchasePaymentApiController _controller;
        private PurchaseInfo _testPurchaseInfo;
        private SupplierReturnInvoice _testSupplierReturnInvoice;
        private PurchaseAmount _testPurchaseAmount;
        private PurchaseItemDetailDto _testPurchaseItemDetail;
        private SupplierInvoiceDetail _testSupplierInvoiceDetail;

        [SetUp]
        public void SetUp()
        {
            _purchasePaymentFacadeMock = new Mock<PurchasePaymentFacade>();
            _purchasePaymentServiceMock = new Mock<IPurchasePaymentService>();
            _controller = new PurchasePaymentApiController(
                _purchasePaymentFacadeMock.Object,
                _purchasePaymentServiceMock.Object);

            _testPurchaseInfo = new PurchaseInfo
            {
                SupplierInvoiceID = 1,
                CompanyID = 1,
                BranchID = 1,
                TotalAmount = 1000.0,
                PaymentAmount = 500.0
            };

            _testSupplierReturnInvoice = new SupplierReturnInvoice
            {
                SupplierReturnInvoiceID = 1,
                TotalAmount = 200.0,
                CompanyID = 1,
                BranchID = 1
            };

            _testPurchaseAmount = new PurchaseAmount
            {
                CompanyID = 1,
                BranchID = 1,
                UserID = 1,
                PaidAmount = 500
            };

            _testPurchaseItemDetail = new PurchaseItemDetailDto
            {
                InvoiceNo = "INV001",
                Products = new List<PurchaseProductDetail>
                {
                    new PurchaseProductDetail
                    {
                        ProductName = "Test Product",
                        Quantity = 10,
                        UnitPrice = 50.0,
                        ItemCost = 500.0
                    }
                },
                Total = 500.0,
                Returns = new List<PurchaseProductDetail>
                {
                    new PurchaseProductDetail
                    {
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 50.0,
                        ItemCost = 100.0
                    }
                }
            };

            _testSupplierInvoiceDetail = new SupplierInvoiceDetail
            {
                SupplierInvoiceDetailID = 1,
                SupplierInvoiceID = 1,
                ProductID = 101,
                ProductName = "Test Product",
                PurchaseQuantity = 10,
                SaleQuantity = 8,
                SaleUnitPrice = 60.0,
                PurchaseUnitPrice = 50.0,
                UserName = "test_user",
                SaleCartDetailID = 1,
                ItemCost = 500.0,
                SupplierInvoiceNo = "INV001",
                SupplierInvoiceDate = DateTime.Now,
                CompanyName = "Test Company",
                ReturnedQuantity = 2,
                Qty = 10
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new PurchasePaymentApiController(
                _purchasePaymentFacadeMock.Object,
                _purchasePaymentServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenPurchasePaymentFacadeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PurchasePaymentApiController(
                null,
                _purchasePaymentServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenPurchasePaymentServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PurchasePaymentApiController(
                _purchasePaymentFacadeMock.Object,
                null));
        }

        [Test]
        public async Task GetRemainingPaymentList_ShouldReturnOkWithList_WhenListExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var purchaseList = new List<PurchaseInfo> { _testPurchaseInfo };
            _purchasePaymentFacadeMock.Setup(f => f.PurchaseRepository.RemainingPaymentList(companyId, branchId))
                                     .ReturnsAsync(purchaseList);

            // Act
            var result = await _controller.GetRemainingPaymentList(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(purchaseList);
            _purchasePaymentFacadeMock.Verify(f => f.PurchaseRepository.RemainingPaymentList(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetPaidHistory_ShouldReturnOkWithHistory_WhenHistoryExists()
        {
            // Arrange
            int id = 1;
            var historyList = new List<PurchaseInfo> { _testPurchaseInfo };
            _purchasePaymentServiceMock.Setup(s => s.GetPurchasePaymentHistoryAsync(id))
                                      .ReturnsAsync(historyList);

            // Act
            var result = await _controller.GetPaidHistory(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(historyList);
            _purchasePaymentServiceMock.Verify(s => s.GetPurchasePaymentHistoryAsync(id), Times.Once());
        }

        [Test]
        public async Task GetReturnPurchaseDetails_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int invoiceId = 1;
            var returnDetails = new List<SupplierReturnInvoice> { _testSupplierReturnInvoice };
            _purchasePaymentFacadeMock.Setup(f => f.SupplierReturnInvoiceRepository.GetReturnDetails(invoiceId))
                                     .ReturnsAsync(returnDetails);

            // Act
            var result = await _controller.GetReturnPurchaseDetails(invoiceId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(returnDetails);
            _purchasePaymentFacadeMock.Verify(f => f.SupplierReturnInvoiceRepository.GetReturnDetails(invoiceId), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnOk_WhenPaymentIsSuccessful()
        {
            // Arrange
            var message = "Payment processed successfully";
            _purchasePaymentServiceMock.Setup(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount))
                                      .ReturnsAsync(message);

            // Act
            var result = await _controller.ProcessPayment(_testPurchaseAmount);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(message);
            _purchasePaymentServiceMock.Verify(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnBadRequest_WhenRemainingAmountError()
        {
            // Arrange
            var errorMessage = Localization.CloudERP.Messages.PurchasePaymentRemainingAmountError;
            _purchasePaymentServiceMock.Setup(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount))
                                      .ReturnsAsync(errorMessage);

            // Act
            var result = await _controller.ProcessPayment(_testPurchaseAmount);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(errorMessage);
            _purchasePaymentServiceMock.Verify(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount), Times.Once());
        }

        [Test]
        public async Task GetCustomPurchasesHistory_ShouldReturnOkWithList_WhenListExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 12, 31);
            var purchaseList = new List<PurchaseInfo> { _testPurchaseInfo };
            _purchasePaymentFacadeMock.Setup(f => f.PurchaseRepository.CustomPurchasesList(companyId, branchId, fromDate, toDate))
                                     .ReturnsAsync(purchaseList);

            // Act
            var result = await _controller.GetCustomPurchasesHistory(companyId, branchId, fromDate, toDate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(purchaseList);
            _purchasePaymentFacadeMock.Verify(f => f.PurchaseRepository.CustomPurchasesList(companyId, branchId, fromDate, toDate), Times.Once());
        }

        [Test]
        public async Task GetPurchaseItemDetail_ShouldReturnOkWithDetail_WhenDetailExists()
        {
            // Arrange
            int id = 1;
            _purchasePaymentFacadeMock.Setup(f => f.PurchaseService.GetPurchaseItemDetailAsync(id))
                                     .ReturnsAsync(_testPurchaseItemDetail);

            // Act
            var result = await _controller.GetPurchaseItemDetail(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testPurchaseItemDetail);
            _purchasePaymentFacadeMock.Verify(f => f.PurchaseService.GetPurchaseItemDetailAsync(id), Times.Once());
        }

        [Test]
        public async Task GetPurchaseInvoice_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int id = 1;
            var invoiceDetails = new List<SupplierInvoiceDetail> { _testSupplierInvoiceDetail };
            _purchasePaymentFacadeMock.Setup(f => f.SupplierInvoiceDetailRepository.GetListByIdAsync(id))
                                     .ReturnsAsync(invoiceDetails);

            // Act
            var result = await _controller.GetPurchaseInvoice(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(invoiceDetails);
            _purchasePaymentFacadeMock.Verify(f => f.SupplierInvoiceDetailRepository.GetListByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetRemainingAmount_ShouldReturnOkWithAmount_WhenAmountsExist()
        {
            // Arrange
            int id = 1;
            double totalInvoiceAmount = 1000.0;
            double totalPaidAmount = 500.0;
            double expectedRemaining = totalInvoiceAmount - totalPaidAmount;
            _purchasePaymentServiceMock.Setup(s => s.GetTotalAmountByIdAsync(id))
                                      .ReturnsAsync(totalInvoiceAmount);
            _purchasePaymentServiceMock.Setup(s => s.GetTotalPaidAmountByIdAsync(id))
                                      .ReturnsAsync(totalPaidAmount);

            // Act
            var result = await _controller.GetRemainingAmount(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(expectedRemaining);
            _purchasePaymentServiceMock.Verify(s => s.GetTotalAmountByIdAsync(id), Times.Once());
            _purchasePaymentServiceMock.Verify(s => s.GetTotalPaidAmountByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetTotalAmount_ShouldReturnOkWithAmount_WhenAmountExists()
        {
            // Arrange
            int id = 1;
            double? totalInvoiceAmount = 1000.0;
            _purchasePaymentServiceMock.Setup(s => s.GetTotalAmountByIdAsync(id))
                                      .ReturnsAsync(totalInvoiceAmount);

            // Act
            var result = await _controller.GetTotalAmount(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(totalInvoiceAmount);
            _purchasePaymentServiceMock.Verify(s => s.GetTotalAmountByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetRemainingPaymentList_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _purchasePaymentFacadeMock.Setup(f => f.PurchaseRepository.RemainingPaymentList(companyId, branchId))
                                     .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetRemainingPaymentList(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchasePaymentFacadeMock.Verify(f => f.PurchaseRepository.RemainingPaymentList(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Payment processing error";
            _purchasePaymentServiceMock.Setup(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount))
                                      .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessPayment(_testPurchaseAmount);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchasePaymentServiceMock.Verify(s => s.ProcessPaymentAsync(
                _testPurchaseAmount.CompanyID,
                _testPurchaseAmount.BranchID,
                _testPurchaseAmount.UserID,
                _testPurchaseAmount), Times.Once());
        }
    }
}