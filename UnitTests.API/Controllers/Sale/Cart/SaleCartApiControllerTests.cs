using API.Controllers.Sale.Cart;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Helpers;

namespace UnitTests.API.Controllers.Sale.Cart
{
    [TestFixture]
    public class SaleCartApiControllerTests
    {
        private Mock<ISaleCartDetailRepository> _saleCartDetailRepositoryMock;
        private Mock<IStockRepository> _stockRepositoryMock;
        private Mock<ISaleCartService> _saleCartServiceMock;
        private SaleCartApiController _controller;
        private SaleCartDetail _testSaleCartDetail;
        private Stock _testStock;
        private List<SaleCartDetail> _testSaleCartDetails;
        private SaleConfirm _testSaleConfirm;

        [SetUp]
        public void SetUp()
        {
            _saleCartDetailRepositoryMock = new Mock<ISaleCartDetailRepository>();
            _stockRepositoryMock = new Mock<IStockRepository>();
            _saleCartServiceMock = new Mock<ISaleCartService>();
            _controller = new SaleCartApiController(
                _saleCartDetailRepositoryMock.Object,
                _stockRepositoryMock.Object,
                _saleCartServiceMock.Object);

            _testSaleCartDetail = new SaleCartDetail
            {
                SaleCartDetailID = 1,
                ProductID = 101,
                ProductName = "Test Product",
                SaleQuantity = 5,
                SaleUnitPrice = 100.0,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1,
                UserName = "test_user"
            };

            _testStock = new Stock
            {
                ProductID = 101,
                ProductName = "Test Product",
                Quantity = 50,
                SaleUnitPrice = 100.0,
                CurrentPurchaseUnitPrice = 60.0,
                ExpiryDate = new DateTime(2026, 1, 1),
                Manufacture = new DateTime(2024, 1, 1),
                StockTreshHoldQuantity = 10,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testSaleCartDetails = new List<SaleCartDetail>
            {
                _testSaleCartDetail,
                new SaleCartDetail
                {
                    SaleCartDetailID = 2,
                    ProductID = 102,
                    ProductName = "Another Product",
                    SaleQuantity = 3,
                    SaleUnitPrice = 150.0,
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 1,
                    UserName = "test_user"
                }
            };

            _testSaleConfirm = new SaleConfirm
            {
                CustomerID = 1,
                Description = "Test sale",
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
            Assert.DoesNotThrow(() => new SaleCartApiController(
                _saleCartDetailRepositoryMock.Object,
                _stockRepositoryMock.Object,
                _saleCartServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSaleCartDetailRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleCartApiController(
                null,
                _stockRepositoryMock.Object,
                _saleCartServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenStockRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleCartApiController(
                _saleCartDetailRepositoryMock.Object,
                null,
                _saleCartServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSaleCartServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleCartApiController(
                _saleCartDetailRepositoryMock.Object,
                _stockRepositoryMock.Object,
                null));
        }

        [Test]
        public async Task GetSaleCartDetails_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _saleCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingAsync(branchId, companyId, userId))
                                         .ReturnsAsync(_testSaleCartDetails);

            // Act
            var result = await _controller.GetSaleCartDetails(branchId, companyId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSaleCartDetails);
            _saleCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingAsync(branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task GetSaleCartDetails_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            var exceptionMessage = "Database error";
            _saleCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingAsync(branchId, companyId, userId))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetSaleCartDetails(branchId, companyId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingAsync(branchId, companyId, userId), Times.Once());
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
            response.SaleUnitPrice.Should().Be(_testStock.SaleUnitPrice);
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
            _saleCartDetailRepositoryMock.Setup(r => r.AddAsync(_testSaleCartDetail))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddItem(_testSaleCartDetail);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Item added successfully");
            _saleCartDetailRepositoryMock.Verify(r => r.AddAsync(_testSaleCartDetail), Times.Once());
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
            _saleCartDetailRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SaleCartDetail>()), Times.Never());
        }

        [Test]
        public async Task AddItem_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Add item error";
            _saleCartDetailRepositoryMock.Setup(r => r.AddAsync(_testSaleCartDetail))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.AddItem(_testSaleCartDetail);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleCartDetailRepositoryMock.Verify(r => r.AddAsync(_testSaleCartDetail), Times.Once());
        }

        [Test]
        public async Task DeleteItem_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            int id = 1;
            _saleCartDetailRepositoryMock.Setup(r => r.DeleteAsync(id))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteItem(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Deleted successfully");
            _saleCartDetailRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once());
        }

        [Test]
        public async Task DeleteItem_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Delete item error";
            _saleCartDetailRepositoryMock.Setup(r => r.DeleteAsync(id))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteItem(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleCartDetailRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once());
        }

        [Test]
        public async Task CancelSale_ShouldReturnOk_WhenDetailsExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _saleCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingAsync(branchId, companyId, userId))
                                         .ReturnsAsync(_testSaleCartDetails);
            _saleCartDetailRepositoryMock.Setup(r => r.DeleteListAsync(_testSaleCartDetails))
                                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelSale(branchId, companyId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.message.Should().Be("Sale canceled");
            _saleCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingAsync(branchId, companyId, userId), Times.Once());
            _saleCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(_testSaleCartDetails), Times.Once());
        }

        [Test]
        public async Task CancelSale_ShouldReturnBadRequest_WhenDetailsDoNotExist()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            _saleCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingAsync(branchId, companyId, userId))
                                         .ReturnsAsync((IEnumerable<SaleCartDetail>)null);

            // Act
            var result = await _controller.CancelSale(branchId, companyId, userId);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _saleCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingAsync(branchId, companyId, userId), Times.Once());
            _saleCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(It.IsAny<IEnumerable<SaleCartDetail>>()), Times.Never());
        }

        [Test]
        public async Task CancelSale_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int branchId = 1, companyId = 1, userId = 1;
            var exceptionMessage = "Cancel sale error";
            _saleCartDetailRepositoryMock.Setup(r => r.GetByDefaultSettingAsync(branchId, companyId, userId))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CancelSale(branchId, companyId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleCartDetailRepositoryMock.Verify(r => r.GetByDefaultSettingAsync(branchId, companyId, userId), Times.Once());
            _saleCartDetailRepositoryMock.Verify(r => r.DeleteListAsync(It.IsAny<IEnumerable<SaleCartDetail>>()), Times.Never());
        }

        [Test]
        public async Task ConfirmSale_ShouldReturnOkWithInvoiceId_WhenSaleIsSuccessful()
        {
            // Arrange
            var result = new Result<int>(true, 123, null); ;
            _saleCartServiceMock.Setup(s => s.ConfirmSaleAsync(_testSaleConfirm))
                                .ReturnsAsync(result);

            // Act
            var resultAction = await _controller.ConfirmSale(_testSaleConfirm);

            // Assert
            var okResult = resultAction as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as Dictionary<string, int>;
            response["invoiceId"].Should().Be(123);
            _saleCartServiceMock.Verify(s => s.ConfirmSaleAsync(_testSaleConfirm), Times.Once());
        }

        [Test]
        public async Task ConfirmSale_ShouldReturnBadRequest_WhenSaleFails()
        {
            // Arrange
            var errorMessage = "Sale confirmation failed";
            var result = new Result<int>(false, 0, errorMessage);
            _saleCartServiceMock.Setup(s => s.ConfirmSaleAsync(_testSaleConfirm))
                                .ReturnsAsync(result);

            // Act
            var resultAction = await _controller.ConfirmSale(_testSaleConfirm);

            // Assert
            var badRequestResult = resultAction as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var response = badRequestResult.Value as dynamic;
            response.error.Should().Be(errorMessage);
            _saleCartServiceMock.Verify(s => s.ConfirmSaleAsync(_testSaleConfirm), Times.Once());
        }

        [Test]
        public async Task ConfirmSale_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Confirm sale error";
            _saleCartServiceMock.Setup(s => s.ConfirmSaleAsync(_testSaleConfirm))
                                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ConfirmSale(_testSaleConfirm);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleCartServiceMock.Verify(s => s.ConfirmSaleAsync(_testSaleConfirm), Times.Once());
        }
    }
}