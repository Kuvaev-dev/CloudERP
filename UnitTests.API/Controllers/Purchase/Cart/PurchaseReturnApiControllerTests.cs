using API.Controllers.Purchase.Cart;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers.API.Purchase.Cart
{
    [TestFixture]
    public class PurchaseReturnApiControllerTests
    {
        private Mock<ISupplierInvoiceRepository> _supplierInvoiceRepositoryMock;
        private Mock<ISupplierReturnInvoiceDetailRepository> _supplierReturnInvoiceDetailRepositoryMock;
        private Mock<IPurchaseReturnService> _purchaseReturnServiceMock;
        private PurchaseReturnApiController _controller;
        private SupplierInvoice _testInvoice;
        private List<SupplierInvoiceDetail> _testInvoiceDetails;
        private PurchaseReturnConfirm _testPurchaseReturnConfirm;
        private PurchaseReturnConfirmResult _testSuccessResult;
        private PurchaseReturnConfirmResult _testFailureResult;

        [SetUp]
        public void SetUp()
        {
            _supplierInvoiceRepositoryMock = new Mock<ISupplierInvoiceRepository>();
            _supplierReturnInvoiceDetailRepositoryMock = new Mock<ISupplierReturnInvoiceDetailRepository>();
            _purchaseReturnServiceMock = new Mock<IPurchaseReturnService>();
            _controller = new PurchaseReturnApiController(
                _supplierInvoiceRepositoryMock.Object,
                _purchaseReturnServiceMock.Object,
                _supplierReturnInvoiceDetailRepositoryMock.Object);

            _testInvoice = new SupplierInvoice
            {
                InvoiceNo = "INV001",
                SupplierID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceDate = new DateTime(2025, 1, 1),
                TotalAmount = 1000
            };

            _testInvoiceDetails = new List<SupplierInvoiceDetail>
            {
                new SupplierInvoiceDetail
                {
                    SupplierInvoiceNo = "INV001",
                    ProductID = 101,
                    ProductName = "Test Product",
                    PurchaseQuantity = 10,
                    PurchaseUnitPrice = 100
                },
                new SupplierInvoiceDetail
                {
                    SupplierInvoiceNo = "INV001",
                    ProductID = 102,
                    ProductName = "Another Product",
                    PurchaseQuantity = 5,
                    PurchaseUnitPrice = 50
                }
            };

            _testPurchaseReturnConfirm = new PurchaseReturnConfirm
            {
                ProductIDs = new List<int> { 1, 2, 3 },
                ReturnQty = new List<int> { 1, 2, 3 },
                SupplierInvoiceID = 1,
                IsPayment = true,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testSuccessResult = new PurchaseReturnConfirmResult
            {
                InvoiceNo = "RET001",
                IsSuccess = true,
                Message = "Return processed successfully"
            };

            _testFailureResult = new PurchaseReturnConfirmResult
            {
                InvoiceNo = string.Empty,
                IsSuccess = false,
                Message = "InvalID return data"
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvIDed()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new PurchaseReturnApiController(
                _supplierInvoiceRepositoryMock.Object,
                _purchaseReturnServiceMock.Object,
                _supplierReturnInvoiceDetailRepositoryMock.Object));
        }

        [Test]
        public async Task FindPurchase_ShouldReturnOkWithResponse_WhenInvoiceAndDetailsExist()
        {
            // Arrange
            string invoiceID = "INV001";
            _supplierInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceID))
                                          .ReturnsAsync(_testInvoice);
            _supplierReturnInvoiceDetailRepositoryMock.Setup(r => r.GetInvoiceDetails(invoiceID))
                                                     .Returns(_testInvoiceDetails);

            // Act
            var result = await _controller.FindPurchase(invoiceID);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as FindPuchaseResponse;
            response.Invoice.Should().BeEquivalentTo(_testInvoice);
            response.InvoiceDetails.Should().BeEquivalentTo(_testInvoiceDetails);
            _supplierInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceID), Times.Once());
            _supplierReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(invoiceID), Times.Once());
        }

        [Test]
        public async Task FindPurchase_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            string invoiceID = "INV999";
            _supplierInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceID))
                                          .ReturnsAsync((SupplierInvoice)null);

            // Act
            var result = await _controller.FindPurchase(invoiceID);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supplierInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceID), Times.Once());
            _supplierReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task FindPurchase_ShouldReturnNotFound_WhenInvoiceDetailsDoNotExist()
        {
            // Arrange
            string invoiceID = "INV001";
            _supplierInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceID))
                                          .ReturnsAsync(_testInvoice);
            _supplierReturnInvoiceDetailRepositoryMock.Setup(r => r.GetInvoiceDetails(invoiceID))
                                                     .Returns((List<SupplierInvoiceDetail>)(IEnumerable<SupplierReturnInvoiceDetail>)null);

            // Act
            var result = await _controller.FindPurchase(invoiceID);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supplierInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceID), Times.Once());
            _supplierReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(invoiceID), Times.Once());
        }

        [Test]
        public async Task FindPurchase_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            string invoiceID = "INV001";
            var exceptionMessage = "Database error";
            _supplierInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceID))
                                          .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.FindPurchase(invoiceID);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceID), Times.Once());
            _supplierReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ProcessPurchaseReturn_ShouldReturnOkWithSuccessResult_WhenReturnIsSuccessful()
        {
            // Arrange
            _purchaseReturnServiceMock.Setup(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm))
                                      .ReturnsAsync(_testSuccessResult);

            // Act
            var result = await _controller.ProcessPurchaseReturn(_testPurchaseReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as PurchaseReturnConfirmResult;
            response.Should().BeEquivalentTo(_testSuccessResult);
            _purchaseReturnServiceMock.Verify(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm), Times.Once());
        }

        [Test]
        public async Task ProcessPurchaseReturn_ShouldReturnOkWithFailureResult_WhenReturnFails()
        {
            // Arrange
            _purchaseReturnServiceMock.Setup(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm))
                                      .ReturnsAsync(_testFailureResult);

            // Act
            var result = await _controller.ProcessPurchaseReturn(_testPurchaseReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as PurchaseReturnConfirmResult;
            response.Should().BeEquivalentTo(_testFailureResult);
            _purchaseReturnServiceMock.Verify(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm), Times.Once());
        }

        [Test]
        public async Task ProcessPurchaseReturn_ShouldReturnOkWithErrorResult_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Return processing error";
            _purchaseReturnServiceMock.Setup(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm))
                                      .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessPurchaseReturn(_testPurchaseReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as PurchaseReturnConfirmResult;
            response.IsSuccess.Should().BeFalse();
            response.InvoiceNo.Should().BeEmpty();
            response.Message.Should().Be("Unexpected error: " + exceptionMessage);
            _purchaseReturnServiceMock.Verify(s => s.ProcessReturnAsync(_testPurchaseReturnConfirm), Times.Once());
        }
    }
}