using API.Controllers.Sale.Payment;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace UnitTests.API.Controllers.Sale.Payment
{
    [TestFixture]
    public class SalePaymentReturnApiControllerTests
    {
        private Mock<ISaleRepository> _saleRepositoryMock;
        private Mock<ICustomerReturnPaymentRepository> _customerReturnPaymentRepositoryMock;
        private Mock<ISalePaymentReturnService> _salePaymentReturnServiceMock;
        private SalePaymentReturnApiController _controller;
        private SaleInfo _testSaleInfo;
        private CustomerReturnPayment _testCustomerReturnPayment;
        private SaleReturn _testSaleReturn;

        [SetUp]
        public void SetUp()
        {
            _saleRepositoryMock = new Mock<ISaleRepository>();
            _customerReturnPaymentRepositoryMock = new Mock<ICustomerReturnPaymentRepository>();
            _salePaymentReturnServiceMock = new Mock<ISalePaymentReturnService>();
            _controller = new SalePaymentReturnApiController(
                _saleRepositoryMock.Object,
                _customerReturnPaymentRepositoryMock.Object,
                _salePaymentReturnServiceMock.Object);

            _testSaleInfo = new SaleInfo
            {
                PaymentID = 1,
                CustomerID = 1,
                CustomerName = "Test Name",
                CustomerContactNo = "1234567890",
                CustomerAddress = "Test Address",
                CustomerInvoiceID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceDate = DateTime.Now,
                InvoiceNo = "INV123",
                TotalAmount = 1000.0,
                ReturnProductAmount = 200.0,
                AfterReturnTotalAmount = 800.0,
                PaymentAmount = 500.0,
                ReturnPaymentAmount = 200.0,
                RemainingBalance = 300.0,
                UserID = 1,
            };

            _testCustomerReturnPayment = new CustomerReturnPayment
            {
                CustomerReturnPaymentID = 1,
                CustomerReturnInvoiceID = 1,
                CustomerID = 1,
                CustomerName = "Test Customer",
                CustomerInvoiceID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceNo = "INV123",
                TotalAmount = 1000.0,
                PaidAmount = 200.0,
                RemainingBalance = 800.0,
                UserID = 1,
                InvoiceDate = DateTime.Now
            };

            _testSaleReturn = new SaleReturn
            {
                InvoiceId = 1,
                PreviousRemainingAmount = 800,
                PaymentAmount = 200,
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new SalePaymentReturnApiController(
                _saleRepositoryMock.Object,
                _customerReturnPaymentRepositoryMock.Object,
                _salePaymentReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSaleRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SalePaymentReturnApiController(
                null,
                _customerReturnPaymentRepositoryMock.Object,
                _salePaymentReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenCustomerReturnPaymentRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SalePaymentReturnApiController(
                _saleRepositoryMock.Object,
                null,
                _salePaymentReturnServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSalePaymentReturnServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SalePaymentReturnApiController(
                _saleRepositoryMock.Object,
                _customerReturnPaymentRepositoryMock.Object,
                null));
        }

        [Test]
        public async Task GetReturnSalePendingAmount_ShouldReturnOkWithList_WhenSalesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var saleList = new List<SaleInfo> { _testSaleInfo };
            _saleRepositoryMock.Setup(r => r.GetReturnSaleAmountPending(companyId, branchId))
                               .ReturnsAsync(saleList);

            // Act
            var result = await _controller.GetReturnSalePendingAmount(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(saleList);
            _saleRepositoryMock.Verify(r => r.GetReturnSaleAmountPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetReturnSalePendingAmount_ShouldReturnNotFound_WhenSalesDoNotExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _saleRepositoryMock.Setup(r => r.GetReturnSaleAmountPending(companyId, branchId))
                               .ReturnsAsync((List<SaleInfo>)null);

            // Act
            var result = await _controller.GetReturnSalePendingAmount(companyId, branchId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _saleRepositoryMock.Verify(r => r.GetReturnSaleAmountPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetReturnSalePendingAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _saleRepositoryMock.Setup(r => r.GetReturnSaleAmountPending(companyId, branchId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetReturnSalePendingAmount(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _saleRepositoryMock.Verify(r => r.GetReturnSaleAmountPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetCustomerReturnPayments_ShouldReturnOkWithPayments_WhenPaymentsExist()
        {
            // Arrange
            int id = 1;
            var paymentList = new List<CustomerReturnPayment> { _testCustomerReturnPayment };
            _customerReturnPaymentRepositoryMock.Setup(r => r.GetByCustomerReturnInvoiceId(id))
                                               .ReturnsAsync(paymentList);

            // Act
            var result = await _controller.GetCustomerReturnPayments(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(paymentList);
            _customerReturnPaymentRepositoryMock.Verify(r => r.GetByCustomerReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task GetCustomerReturnPayments_ShouldReturnNotFound_WhenPaymentsDoNotExist()
        {
            // Arrange
            int id = 1;
            _customerReturnPaymentRepositoryMock.Setup(r => r.GetByCustomerReturnInvoiceId(id))
                                               .ReturnsAsync((IEnumerable<CustomerReturnPayment>)null);

            // Act
            var result = await _controller.GetCustomerReturnPayments(id);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _customerReturnPaymentRepositoryMock.Verify(r => r.GetByCustomerReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task GetCustomerReturnPayments_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Repository error";
            _customerReturnPaymentRepositoryMock.Setup(r => r.GetByCustomerReturnInvoiceId(id))
                                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetCustomerReturnPayments(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _customerReturnPaymentRepositoryMock.Verify(r => r.GetByCustomerReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnOkWithMessage_WhenProcessingIsSuccessful()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var message = "Return amount processed successfully";
            _salePaymentReturnServiceMock.Setup(s => s.ProcessReturnAmountAsync(_testSaleReturn, branchId, companyId, userId))
                                         .ReturnsAsync(message);

            // Act
            var result = await _controller.ProcessReturnAmount(_testSaleReturn, branchId, companyId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(message);
            _salePaymentReturnServiceMock.Verify(s => s.ProcessReturnAmountAsync(_testSaleReturn, branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var exceptionMessage = "Processing error";
            _salePaymentReturnServiceMock.Setup(s => s.ProcessReturnAmountAsync(_testSaleReturn, branchId, companyId, userId))
                                         .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessReturnAmount(_testSaleReturn, branchId, companyId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _salePaymentReturnServiceMock.Verify(s => s.ProcessReturnAmountAsync(_testSaleReturn, branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnBadRequest_WhenPaymentReturnDtoIsNull()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            SaleReturn nullPaymentReturn = null;

            // Act
            var result = await _controller.ProcessReturnAmount(nullPaymentReturn, branchId, companyId, userId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _salePaymentReturnServiceMock.Verify(s => s.ProcessReturnAmountAsync(It.IsAny<SaleReturn>(), branchId, companyId, userId), Times.Never());
        }
    }
}