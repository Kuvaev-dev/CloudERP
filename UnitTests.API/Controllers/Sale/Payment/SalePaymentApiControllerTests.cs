using API.Controllers.Sale.Payment;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Services.Facades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.API.Controllers.Sale.Payment
{
    [TestFixture]
    public class SalePaymentApiControllerTests
    {
        private Mock<SalePaymentFacade> _salePaymentFacadeMock;
        private SalePaymentApiController _controller;
        private SaleInfo _testSaleInfo;
        private CustomerReturnInvoice _testCustomerReturnInvoice;
        private SaleAmount _testSaleAmount;
        private PurchaseItemDetailDto _testSaleItemDetail;
        private CustomerInvoiceDetail _testCustomerInvoiceDetail;

        [SetUp]
        public void SetUp()
        {
            _salePaymentFacadeMock = new Mock<SalePaymentFacade>();
            _salePaymentFacadeMock.Setup(f => f.SaleRepository).Returns(new Mock<ISaleRepository>().Object);
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService).Returns(new Mock<ISalePaymentService>().Object);
            _salePaymentFacadeMock.Setup(f => f.CustomerReturnInvoiceRepository).Returns(new Mock<ICustomerReturnInvoiceRepository>().Object);
            _salePaymentFacadeMock.Setup(f => f.SaleService).Returns(new Mock<ISaleService>().Object);
            _salePaymentFacadeMock.Setup(f => f.CustomerInvoiceDetailRepository).Returns(new Mock<ICustomerInvoiceDetailRepository>().Object);

            _controller = new SalePaymentApiController(_salePaymentFacadeMock.Object);

            _testSaleInfo = new SaleInfo
            {
                InvoiceID = 1,
                CustomerID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceDate = DateTime.Now,
                TotalAmount = 1000.0,
                PaidAmount = 500.0,
                RemainingBalance = 500.0,
                UserID = 1
            };

            _testCustomerReturnInvoice = new CustomerReturnInvoice
            {
                CustomerReturnInvoiceID = 1,
                CustomerInvoiceID = 1,
                CustomerID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceNo = "RET001",
                TotalAmount = 200.0,
                InvoiceDate = DateTime.Now,
                UserID = 1
            };

            _testSaleAmount = new SaleAmount
            {
                InvoiceId = 1,
                PreviousRemainingAmount = 500.0,
                PaidAmount = 200.0,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testSaleItemDetail = new PurchaseItemDetailDto
            {
                InvoiceNo = "INV001",
                Products = new List<PurchaseProductDetail>
                {
                    new PurchaseProductDetail
                    {
                        ProductName = "Test Product",
                        Quantity = 5,
                        UnitPrice = 100.0,
                        ItemCost = 500.0
                    }
                },
                Total = 500.0
            };

            _testCustomerInvoiceDetail = new CustomerInvoiceDetail
            {
                CustomerInvoiceDetailID = 1,
                CustomerInvoiceID = 1,
                ProductID = 101,
                ProductName = "Test Product",
                SaleQuantity = 5,
                SaleUnitPrice = 100.0,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1,
                UserName = "test_user"
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenSalePaymentFacadeIsProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new SalePaymentApiController(_salePaymentFacadeMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSalePaymentFacadeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SalePaymentApiController(null));
        }

        [Test]
        public async Task GetRemainingPaymentList_ShouldReturnOkWithList_WhenListExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var saleList = new List<SaleInfo> { _testSaleInfo };
            _salePaymentFacadeMock.Setup(f => f.SaleRepository.RemainingPaymentList(companyId, branchId))
                                  .ReturnsAsync(saleList);

            // Act
            var result = await _controller.GetRemainingPaymentList(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(saleList);
            _salePaymentFacadeMock.Verify(f => f.SaleRepository.RemainingPaymentList(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetRemainingPaymentList_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _salePaymentFacadeMock.Setup(f => f.SaleRepository.RemainingPaymentList(companyId, branchId))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetRemainingPaymentList(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SaleRepository.RemainingPaymentList(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetPaidHistory_ShouldReturnOkWithHistory_WhenHistoryExists()
        {
            // Arrange
            int id = 1;
            var historyList = new List<SaleInfo> { _testSaleInfo };
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetSalePaymentHistoryAsync(id))
                                  .ReturnsAsync(historyList);

            // Act
            var result = await _controller.GetPaidHistory(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(historyList);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetSalePaymentHistoryAsync(id), Times.Once());
        }

        [Test]
        public async Task GetPaidHistory_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "History retrieval error";
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetSalePaymentHistoryAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetPaidHistory(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetSalePaymentHistoryAsync(id), Times.Once());
        }

        [Test]
        public async Task GetReturnSaleDetails_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int invoiceId = 1;
            var returnDetails = new List<CustomerReturnInvoice> { _testCustomerReturnInvoice };
            _salePaymentFacadeMock.Setup(f => f.CustomerReturnInvoiceRepository.GetReturnDetails(invoiceId))
                                  .ReturnsAsync(returnDetails);

            // Act
            var result = await _controller.GetReturnSaleDetails(invoiceId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(returnDetails);
            _salePaymentFacadeMock.Verify(f => f.CustomerReturnInvoiceRepository.GetReturnDetails(invoiceId), Times.Once());
        }

        [Test]
        public async Task GetReturnSaleDetails_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int invoiceId = 1;
            var exceptionMessage = "Return details error";
            _salePaymentFacadeMock.Setup(f => f.CustomerReturnInvoiceRepository.GetReturnDetails(invoiceId))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetReturnSaleDetails(invoiceId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.CustomerReturnInvoiceRepository.GetReturnDetails(invoiceId), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnOk_WhenPaymentIsSuccessful()
        {
            // Arrange
            var message = "Payment processed successfully";
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount))
                                  .ReturnsAsync(message);

            // Act
            var result = await _controller.ProcessPayment(_testSaleAmount);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(message);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnBadRequest_WhenRemainingAmountError()
        {
            // Arrange
            var errorMessage = Localization.CloudERP.Messages.PurchasePaymentRemainingAmountError;
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount))
                                  .ReturnsAsync(errorMessage);

            // Act
            var result = await _controller.ProcessPayment(_testSaleAmount);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(errorMessage);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount), Times.Once());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnBadRequest_WhenPaymentDtoIsNull()
        {
            // Arrange
            SaleAmount nullPaymentDto = null;

            // Act
            var result = await _controller.ProcessPayment(nullPaymentDto);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.ProcessPaymentAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SaleAmount>()), Times.Never());
        }

        [Test]
        public async Task ProcessPayment_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var exceptionMessage = "Payment processing error";
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessPayment(_testSaleAmount);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.ProcessPaymentAsync(
                _testSaleAmount.CompanyID,
                _testSaleAmount.BranchID,
                _testSaleAmount.UserID,
                _testSaleAmount), Times.Once());
        }

        [Test]
        public async Task GetCustomSalesHistory_ShouldReturnOkWithList_WhenListExists()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 12, 31);
            var saleList = new List<SaleInfo> { _testSaleInfo };
            _salePaymentFacadeMock.Setup(f => f.SaleRepository.CustomSalesList(companyId, branchId, fromDate, toDate))
                                  .ReturnsAsync(saleList);

            // Act
            var result = await _controller.GetCustomSalesHistory(companyId, branchId, fromDate, toDate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(saleList);
            _salePaymentFacadeMock.Verify(f => f.SaleRepository.CustomSalesList(companyId, branchId, fromDate, toDate), Times.Once());
        }

        [Test]
        public async Task GetCustomSalesHistory_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 12, 31);
            var exceptionMessage = "History retrieval error";
            _salePaymentFacadeMock.Setup(f => f.SaleRepository.CustomSalesList(companyId, branchId, fromDate, toDate))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetCustomSalesHistory(companyId, branchId, fromDate, toDate);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SaleRepository.CustomSalesList(companyId, branchId, fromDate, toDate), Times.Once());
        }

        [Test]
        public async Task GetSaleItemDetail_ShouldReturnOkWithDetail_WhenDetailExists()
        {
            // Arrange
            int id = 1;
            _salePaymentFacadeMock.Setup(f => f.SaleService.GetSaleItemDetailAsync(id))
                                  .ReturnsAsync(_testSaleItemDetail);

            // Act
            var result = await _controller.GetSaleItemDetail(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_testSaleItemDetail);
            _salePaymentFacadeMock.Verify(f => f.SaleService.GetSaleItemDetailAsync(id), Times.Once());
        }

        [Test]
        public async Task GetSaleItemDetail_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Item detail error";
            _salePaymentFacadeMock.Setup(f => f.SaleService.GetSaleItemDetailAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetSaleItemDetail(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SaleService.GetSaleItemDetailAsync(id), Times.Once());
        }

        [Test]
        public async Task GetSaleInvoice_ShouldReturnOkWithDetails_WhenDetailsExist()
        {
            // Arrange
            int id = 1;
            var invoiceDetails = new List<CustomerInvoiceDetail> { _testCustomerInvoiceDetail };
            _salePaymentFacadeMock.Setup(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id))
                                  .ReturnsAsync(invoiceDetails);

            // Act
            var result = await _controller.GetSaleInvoice(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(invoiceDetails);
            _salePaymentFacadeMock.Verify(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetSaleInvoice_ShouldReturnNotFound_WhenDetailsDoNotExist()
        {
            // Arrange
            int id = 1;
            _salePaymentFacadeMock.Setup(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id))
                                  .ReturnsAsync((IEnumerable<CustomerInvoiceDetail>)null);

            // Act
            var result = await _controller.GetSaleInvoice(id);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _salePaymentFacadeMock.Verify(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetSaleInvoice_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Invoice details error";
            _salePaymentFacadeMock.Setup(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetSaleInvoice(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.CustomerInvoiceDetailRepository.GetListByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetRemainingAmount_ShouldReturnOkWithAmount_WhenAmountsExist()
        {
            // Arrange
            int id = 1;
            double totalInvoiceAmount = 1000.0;
            double totalPaidAmount = 500.0;
            double expectedRemaining = totalInvoiceAmount - totalPaidAmount;
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetTotalAmountByIdAsync(id))
                                  .ReturnsAsync(totalInvoiceAmount);
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetTotalPaidAmountByIdAsync(id))
                                  .ReturnsAsync(totalPaidAmount);

            // Act
            var result = await _controller.GetRemainingAmount(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(expectedRemaining);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalAmountByIdAsync(id), Times.Once());
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalPaidAmountByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetRemainingAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Amount retrieval error";
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetTotalAmountByIdAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetRemainingAmount(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalAmountByIdAsync(id), Times.Once());
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalPaidAmountByIdAsync(id), Times.Never());
        }

        [Test]
        public async Task GetTotalAmount_ShouldReturnOkWithAmount_WhenAmountExists()
        {
            // Arrange
            int id = 1;
            double? totalInvoiceAmount = 1000.0;
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetTotalAmountByIdAsync(id))
                                  .ReturnsAsync(totalInvoiceAmount);

            // Act
            var result = await _controller.GetTotalAmount(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(totalInvoiceAmount);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalAmountByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetTotalAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Total amount error";
            _salePaymentFacadeMock.Setup(f => f.SalePaymentService.GetTotalAmountByIdAsync(id))
                                  .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTotalAmount(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentFacadeMock.Verify(f => f.SalePaymentService.GetTotalAmountByIdAsync(id), Times.Once());
        }
    }
}