using API.Controllers.Purchase.Cart;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utils.Helpers;

namespace UnitTests.API.Controllers.Purchase.Cart
{
    [TestFixture]
    public class PurchaseCartApiControllerTests
    {
        private Mock<IPurchaseCartDetailRepository> _purchaseCartDetailRepositoryMock;
        private Mock<IStockRepository> _stockRepositoryMock;
        private Mock<ISupplierRepository> _supplierRepositoryMock;
        private Mock<IPurchaseCartService> _purchaseCartServiceMock;
        private PurchaseCartApiController _controller;
        private PurchaseCartDetail _testPurchaseCartDetail;
        private Stock _testStock;
        private List<PurchaseCartDetail> _testPurchaseCartDetails;
        private PurchaseConfirm _testPurchaseConfirm;

        [SetUp]
        public void SetUp()
        {
            _purchaseCartDetailRepositoryMock = new Mock<IPurchaseCartDetailRepository>();
            _stockRepositoryMock = new Mock<IStockRepository>();
            _supplierRepositoryMock = new Mock<ISupplierRepository>();
            _purchaseCartServiceMock = new Mock<IPurchaseCartService>();
            _controller = new PurchaseCartApiController(
                _purchaseCartDetailRepositoryMock.Object,
                _stockRepositoryMock.Object,
                _purchaseCartServiceMock.Object,
                _supplierRepositoryMock.Object);

            _testPurchaseCartDetail = new PurchaseCartDetail
            {
                PurchaseCartDetailID = 1,
                ProductID = 101,
                ProductName = "Test Product",
                PurchaseQuantity = 10,
                PurchaseUnitPrice = 50.0,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1,
                UserName = "test_user"
            };

            _testStock = new Stock
            {
                ProductID = 101,
                ProductName = "Test Product",
                Quantity = 100,
                SaleUnitPrice = 75.0,
                CurrentPurchaseUnitPrice = 50.0,
                ExpiryDate = new DateTime(2026, 1, 1),
                Manufacture = new DateTime(2024, 1, 1),
                StockTreshHoldQuantity = 20,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testPurchaseCartDetails = new List<PurchaseCartDetail>
            {
                _testPurchaseCartDetail,
                new PurchaseCartDetail
                {
                    PurchaseCartDetailID = 2,
                    ProductID = 102,
                    ProductName = "Another Product",
                    PurchaseQuantity = 5,
                    PurchaseUnitPrice = 30.0,
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 1,
                    UserName = "test_user"
                }
            };

            _testPurchaseConfirm = new PurchaseConfirm
            {
                SupplierId = 1,
                Description = "Test purchase",
                IsPayment = true,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new PurchaseCartApiController(
                _purchaseCartDetailRepositoryMock.Object,
                _stockRepositoryMock.Object,
                _purchaseCartServiceMock.Object,
                _supplierRepositoryMock.Object));
        }

        [Test]
        public async Task GetPurchaseCartDetails_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ReturnsAsync(_testPurchaseCartDetails);

            // Act
            var result = await _controller.GetPurchaseCartDetails(branchId, companyId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testPurchaseCartDetails);
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task GetPurchaseCartDetails_ShouldReturnNotFound_WhenDetailsDoNotExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ReturnsAsync((IEnumerable<PurchaseCartDetail>)null);

            // Act
            var result = await _controller.GetPurchaseCartDetails(branchId, companyId, userId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task GetPurchaseCartDetails_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            var exceptionMessage = "Database error";
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetPurchaseCartDetails(branchId, companyId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task GetProductDetails_ShouldReturnOkWithPrice_WhenProductExists()
        {
            // Arrange
            int productId = 101;
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                .ReturnsAsync(_testStock);

            // Act
            var result = await _controller.GetProductDetails(productId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.CurrentPurchaseUnitPrice.Should().Be(_testStock.CurrentPurchaseUnitPrice);
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once());
        }

        [Test]
        public async Task GetProductDetails_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                .ReturnsAsync((Stock)null);

            // Act
            var result = await _controller.GetProductDetails(productId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once());
        }

        [Test]
        public async Task GetProductDetails_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int productId = 101;
            var exceptionMessage = "Stock retrieval error";
            _stockRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetProductDetails(productId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once());
        }

        [Test]
        public async Task AddItem_ShouldReturnOk_WhenItemIsValid()
        {
            // Arrange
            _purchaseCartDetailRepositoryMock.Setup(r => r.AddAsync(_testPurchaseCartDetail))
                                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddItem(_testPurchaseCartDetail);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Item added successfully");
            _purchaseCartDetailRepositoryMock.Verify(r => r.AddAsync(_testPurchaseCartDetail), Times.Once());
        }

        [Test]
        public async Task AddItem_ShouldReturnBadRequest_WhenItemIsNull()
        {
            // Act
            var result = await _controller.AddItem(null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _purchaseCartDetailRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PurchaseCartDetail>()), Times.Never());
        }

        [Test]
        public async Task AddItem_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Add item error";
            _purchaseCartDetailRepositoryMock.Setup(r => r.AddAsync(_testPurchaseCartDetail))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.AddItem(_testPurchaseCartDetail);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseCartDetailRepositoryMock.Verify(r => r.AddAsync(_testPurchaseCartDetail), Times.Once());
        }

        [Test]
        public async Task DeleteItem_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            int id = 1;
            _purchaseCartDetailRepositoryMock.Setup(r => r.DeleteAsync(id))
                                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteItem(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Deleted successfully");
            _purchaseCartDetailRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once());
        }

        [Test]
        public async Task DeleteItem_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Delete item error";
            _purchaseCartDetailRepositoryMock.Setup(r => r.DeleteAsync(id))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteItem(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseCartDetailRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once());
        }

        [Test]
        public async Task CancelPurchase_ShouldReturnOk_WhenDetailsExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ReturnsAsync(_testPurchaseCartDetails);
            _purchaseCartDetailRepositoryMock.Setup(r => r.DeleteListAsync(_testPurchaseCartDetails))
                                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelPurchase(branchId, companyId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Purchase canceled");
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
            _purchaseCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(_testPurchaseCartDetails), Times.Once());
        }

        [Test]
        public async Task CancelPurchase_ShouldReturnBadRequest_WhenDetailsDoNotExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ReturnsAsync((IEnumerable<PurchaseCartDetail>)null);

            // Act
            var result = await _controller.CancelPurchase(branchId, companyId, userId);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
            _purchaseCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(It.IsAny<IEnumerable<PurchaseCartDetail>>()), Times.Never());
        }

        [Test]
        public async Task CancelPurchase_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            var exceptionMessage = "Cancel purchase error";
            _purchaseCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId))
                                             .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CancelPurchase(branchId, companyId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingsAsync(branchId, companyId, userId), Times.Once());
            _purchaseCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(It.IsAny<IEnumerable<PurchaseCartDetail>>()), Times.Never());
        }

        [Test]
        public async Task ConfirmPurchase_ShouldReturnOkWithInvoiceId_WhenPurchaseIsSuccessful()
        {
            // Arrange
            var result = new Result<int> { IsSuccess = true, Value = 123 };
            _purchaseCartServiceMock.Setup(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm))
                                    .ReturnsAsync(result);

            // Act
            var resultAction = await _controller.ConfirmPurchase(_testPurchaseConfirm);

            // Assert
            var okResult = resultAction as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as Dictionary<string, int>;
            response["invoiceId"].Should().Be(123);
            _purchaseCartServiceMock.Verify(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm), Times.Once());
        }

        [Test]
        public async Task ConfirmPurchase_ShouldReturnBadRequest_WhenPurchaseFails()
        {
            // Arrange
            var errorMessage = "Purchase confirmation failed";
            var result = new Result<int> { IsSuccess = false, ErrorMessage = errorMessage };
            _purchaseCartServiceMock.Setup(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm))
                                    .ReturnsAsync(result);

            // Act
            var resultAction = await _controller.ConfirmPurchase(_testPurchaseConfirm);

            // Assert
            var badRequestResult = resultAction as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var response = badRequestResult.Value as dynamic;
            response.error.Should().Be(errorMessage);
            _purchaseCartServiceMock.Verify(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm), Times.Once());
        }

        [Test]
        public async Task ConfirmPurchase_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Confirm purchase error";
            _purchaseCartServiceMock.Setup(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm))
                                    .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ConfirmPurchase(_testPurchaseConfirm);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseCartServiceMock.Verify(s => s.ConfirmPurchaseAsync(_testPurchaseConfirm), Times.Once());
        }
    }
}
