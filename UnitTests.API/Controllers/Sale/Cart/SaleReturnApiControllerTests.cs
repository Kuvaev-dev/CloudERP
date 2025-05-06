using API.Controllers.Sale.Cart;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Sale.Cart
{
    [TestFixture]
    public class SaleReturnApiControllerTests
    {
        private Mock<ICustomerInvoiceRepository> _customerInvoiceRepositoryMock;
        private Mock<ICustomerReturnInvoiceDetailRepository> _customerReturnInvoiceDetailRepositoryMock;
        private Mock<ISaleReturnService> _saleReturnServiceMock;
        private SaleReturnApiController _controller;
        private CustomerInvoice _testCustomerInvoice;
        private List<CustomerReturnInvoiceDetail> _testInvoiceDetails;
        private SaleReturnConfirm _testSaleReturnConfirm;
        private SaleReturnConfirmResult _testSuccessResult;
        private SaleReturnConfirmResult _testFailureResult;

        [SetUp]
        public void SetUp()
        {
            _customerInvoiceRepositoryMock = new Mock<ICustomerInvoiceRepository>();
            _customerReturnInvoiceDetailRepositoryMock = new Mock<ICustomerReturnInvoiceDetailRepository>();
            _saleReturnServiceMock = new Mock<ISaleReturnService>();
            _controller = new SaleReturnApiController(
                _customerInvoiceRepositoryMock.Object,
                _customerReturnInvoiceDetailRepositoryMock.Object,
                _saleReturnServiceMock.Object);

            _testCustomerInvoice = new CustomerInvoice
            {
                CustomerInvoiceID = 1,
                InvoiceNo = "INV001",
                CustomerID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceDate = DateTime.Now,
                TotalAmount = 1000.0,
                UserID = 1
            };

            _testInvoiceDetails = new List<CustomerReturnInvoiceDetail>
            {
                new CustomerReturnInvoiceDetail
                {
                    CustomerReturnInvoiceDetailID = 1,
                    InvoiceNo = "INV001",
                    ProductID = 101,
                    ProductName = "Test Product",
                    ReturnQuantity = 2,
                    UnitPrice = 100.0,
                    CompanyID = 1,
                    BranchID = 1,
                    UserID = 1
                }
            };

            _testSaleReturnConfirm = new SaleReturnConfirm
            {
                InvoiceNo = "INV001",
                ReturnItems = new List<SaleReturnItem>
                {
                    new SaleReturnItem
                    {
                        ProductID = 101,
                        ReturnQuantity = 2,
                        UnitPrice = 100.0
                    }
                },
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testSuccessResult = new SaleReturnConfirmResult
            {
                InvoiceNo = "RET001",
                IsSuccess = true,
                Message = "Return processed successfully"
            };

            _testFailureResult = new SaleReturnConfirmResult
            {
                InvoiceNo = string.Empty,
                IsSuccess = false,
                Message = "Invalid return data"
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new SaleReturnApiController(
                _customerInvoiceRepositoryMock.Object,
                _customerReturnInvoiceDetailRepositoryMock.Object,
                _saleReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenCustomerInvoiceRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleReturnApiController(
                null,
                _customerReturnInvoiceDetailRepositoryMock.Object,
                _saleReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenCustomerReturnInvoiceDetailRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleReturnApiController(
                _customerInvoiceRepositoryMock.Object,
                null,
                _saleReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSaleReturnServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SaleReturnApiController(
                _customerInvoiceRepositoryMock.Object,
                _customerReturnInvoiceDetailRepositoryMock.Object,
                null));
        }

        [Test]
        public async Task FindSale_ShouldReturnOkWithResponse_WhenInvoiceAndDetailsExist()
        {
            // Arrange
            string invoiceNo = "INV001";
            _customerInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceNo))
                                          .ReturnsAsync(_testCustomerInvoice);
            _customerReturnInvoiceDetailRepositoryMock.Setup(r => r.GetInvoiceDetails(invoiceNo))
                                                     .Returns(_testInvoiceDetails);

            // Act
            var result = await _controller.FindSale(invoiceNo);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as FindSaleResponse;
            response.Invoice.Should().BeEquivalentTo(_testCustomerInvoice);
            response.InvoiceDetails.Should().BeEquivalentTo(_testInvoiceDetails);
            _customerInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceNo), Times.Once());
            _customerReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(invoiceNo), Times.Once());
        }

        [Test]
        public async Task FindSale_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            string invoiceNo = "INV999";
            _customerInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceNo))
                                          .ReturnsAsync((CustomerInvoice)null);

            // Act
            var result = await _controller.FindSale(invoiceNo);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _customerInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceNo), Times.Once());
            _customerReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task FindSale_ShouldReturnNotFound_WhenInvoiceDetailsDoNotExist()
        {
            // Arrange
            string invoiceNo = "INV001";
            _customerInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceNo))
                                          .ReturnsAsync(_testCustomerInvoice);
            _customerReturnInvoiceDetailRepositoryMock.Setup(r => r.GetInvoiceDetails(invoiceNo))
                                                     .Returns((List<CustomerInvoiceDetail>)(IEnumerable<CustomerReturnInvoiceDetail>)null);

            // Act
            var result = await _controller.FindSale(invoiceNo);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _customerInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceNo), Times.Once());
            _customerReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(invoiceNo), Times.Once());
        }

        [Test]
        public async Task FindSale_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            string invoiceNo = "INV001";
            var exceptionMessage = "Database error";
            _customerInvoiceRepositoryMock.Setup(r => r.GetByInvoiceNoAsync(invoiceNo))
                                          .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.FindSale(invoiceNo);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerInvoiceRepositoryMock.Verify(r => r.GetByInvoiceNoAsync(invoiceNo), Times.Once());
            _customerReturnInvoiceDetailRepositoryMock.Verify(r => r.GetInvoiceDetails(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ProcessSaleReturn_ShouldReturnOkWithSuccessResult_WhenProcessingIsSuccessful()
        {
            // Arrange
            _saleReturnServiceMock.Setup(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm))
                                  .ReturnsAsync(_testSuccessResult);

            // Act
            var result = await _controller.ProcessSaleReturn(_testSaleReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as SaleReturnConfirmResult;
            response.Should().BeEquivalentTo(_testSuccessResult);
            _saleReturnServiceMock.Verify(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm), Times.Once());
        }

        [Test]
        public async Task ProcessSaleReturn_ShouldReturnOkWithFailureResult_WhenProcessingFails()
        {
            // Arrange
            _saleReturnServiceMock.Setup(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm))
                                  .ReturnsAsync(_testFailureResult);

            // Act
            var result = await _controller.ProcessSaleReturn(_testSaleReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as SaleReturnConfirmResult;
            response.Should().BeEquivalentTo(_testFailureResult);
            _saleReturnServiceMock.Verify(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm), Times.Once());
        }

        [Test]
        public async Task ProcessSaleReturn_ShouldReturnOkWithFailureResult_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Processing error";
            _saleReturnServiceMock.Setup(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessSaleReturn(_testSaleReturnConfirm);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as SaleReturnConfirmResult;
            response.IsSuccess.Should().BeFalse();
            response.InvoiceNo.Should().BeEmpty();
            response.Message.Should().Be("Unexpected error: " + exceptionMessage);
            _saleReturnServiceMock.Verify(s => s.ProcessReturnConfirmAsync(_testSaleReturnConfirm), Times.Once());
        }

        [Test]
        public async Task ProcessSaleReturn_ShouldReturnBadRequest_WhenReturnConfirmDtoIsNull()
        {
            // Arrange
            SaleReturnConfirm nullReturnConfirm = null;

            // Act
            var result = await _controller.ProcessSaleReturn(nullReturnConfirm);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _saleReturnServiceMock.Verify(s => s.ProcessReturnConfirmAsync(It.IsAny<SaleReturnConfirm>()), Times.Never());
        }
    }
}