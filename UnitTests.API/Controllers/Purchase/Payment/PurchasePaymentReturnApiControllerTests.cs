using API.Controllers.Purchase.Payment;
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

namespace UnitTests.API.Controllers.Purchase.Payment
{
    [TestFixture]
    public class PurchasePaymentReturnApiControllerTests
    {
        private Mock<IPurchasePaymentReturnService> _purchasePaymentReturnServiceMock;
        private Mock<IPurchaseRepository> _purchaseRepositoryMock;
        private Mock<ISupplierReturnPaymentRepository> _supplierReturnPaymentRepositoryMock;
        private PurchasePaymentReturnApiController _controller;
        private PurchaseInfo _testPurchaseInfo;
        private SupplierReturnPayment _testSupplierReturnPayment;
        private PurchaseReturn _testPurchaseReturn;

        [SetUp]
        public void SetUp()
        {
            _purchasePaymentReturnServiceMock = new Mock<IPurchasePaymentReturnService>();
            _purchaseRepositoryMock = new Mock<IPurchaseRepository>();
            _supplierReturnPaymentRepositoryMock = new Mock<ISupplierReturnPaymentRepository>();
            _controller = new PurchasePaymentReturnApiController(
                _purchasePaymentReturnServiceMock.Object,
                _purchaseRepositoryMock.Object,
                _supplierReturnPaymentRepositoryMock.Object);

            _testPurchaseInfo = new PurchaseInfo
            {
                InvoiceID = 1,
                SupplierID = 1,
                CompanyID = 1,
                BranchID = 1,
                InvoiceDate = DateTime.Now,
                TotalAmount = 1000.0,
                PaidAmount = 500.0,
                SupplierInvoiceID = 1,
                UserID = 1
            };

            _testSupplierReturnPayment = new SupplierReturnPayment
            {
                SupplierReturnPaymentID = 1,
                SupplierReturnInvoiceID = 1,
                Amount = 200.0,
                PaymentDate = DateTime.Now,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };

            _testPurchaseReturn = new PurchaseReturn
            {
                InvoiceId = 1,
                ReturnAmount = 200.0,
                CompanyID = 1,
                BranchID = 1,
                UserID = 1
            };
        }

        [Test]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new PurchasePaymentReturnApiController(
                _purchasePaymentReturnServiceMock.Object,
                _purchaseRepositoryMock.Object,
                _supplierReturnPaymentRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenPurchasePaymentReturnServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PurchasePaymentReturnApiController(
                null,
                _purchaseRepositoryMock.Object,
                _supplierReturnPaymentRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenPurchaseRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PurchasePaymentReturnApiController(
                _purchasePaymentReturnServiceMock.Object,
                null,
                _supplierReturnPaymentRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSupplierReturnPaymentRepositoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PurchasePaymentReturnApiController(
                _purchasePaymentReturnServiceMock.Object,
                _purchaseRepositoryMock.Object,
                null));
        }

        [Test]
        public async Task GetReturnPurchasePendingAmount_ShouldReturnOkWithList_WhenPurchasesExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var purchaseList = new List<PurchaseInfo> { _testPurchaseInfo };
            _purchaseRepositoryMock.Setup(r => r.GetReturnPurchasesPaymentPending(companyId, branchId))
                                   .ReturnsAsync(purchaseList);

            // Act
            var result = await _controller.GetReturnPurchasePendingAmount(companyId, branchId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(purchaseList);
            _purchaseRepositoryMock.Verify(r => r.GetReturnPurchasesPaymentPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetReturnPurchasePendingAmount_ShouldReturnNotFound_WhenPurchasesDoNotExist()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            _purchaseRepositoryMock.Setup(r => r.GetReturnPurchasesPaymentPending(companyId, branchId))
                                   .ReturnsAsync((List<PurchaseInfo>)null);

            // Act
            var result = await _controller.GetReturnPurchasePendingAmount(companyId, branchId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _purchaseRepositoryMock.Verify(r => r.GetReturnPurchasesPaymentPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetReturnPurchasePendingAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Database error";
            _purchaseRepositoryMock.Setup(r => r.GetReturnPurchasesPaymentPending(companyId, branchId))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetReturnPurchasePendingAmount(companyId, branchId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchaseRepositoryMock.Verify(r => r.GetReturnPurchasesPaymentPending(companyId, branchId), Times.Once());
        }

        [Test]
        public async Task GetSupplierReturnPayments_ShouldReturnOkWithPayments_WhenPaymentsExist()
        {
            // Arrange
            int id = 1;
            var paymentList = new List<SupplierReturnPayment> { _testSupplierReturnPayment };
            _supplierReturnPaymentRepositoryMock.Setup(r => r.GetBySupplierReturnInvoiceId(id))
                                               .ReturnsAsync(paymentList);

            // Act
            var result = await _controller.GetSupplierReturnPayments(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(paymentList);
            _supplierReturnPaymentRepositoryMock.Verify(r => r.GetBySupplierReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task GetSupplierReturnPayments_ShouldReturnNotFound_WhenPaymentsDoNotExist()
        {
            // Arrange
            int id = 1;
            _supplierReturnPaymentRepositoryMock.Setup(r => r.GetBySupplierReturnInvoiceId(id))
                                               .ReturnsAsync((IEnumerable<SupplierReturnPayment>)null);

            // Act
            var result = await _controller.GetSupplierReturnPayments(id);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            _supplierReturnPaymentRepositoryMock.Verify(r => r.GetBySupplierReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task GetSupplierReturnPayments_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            var exceptionMessage = "Repository error";
            _supplierReturnPaymentRepositoryMock.Setup(r => r.GetBySupplierReturnInvoiceId(id))
                                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetSupplierReturnPayments(id);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supplierReturnPaymentRepositoryMock.Verify(r => r.GetBySupplierReturnInvoiceId(id), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnOkWithMessage_WhenProcessingIsSuccessful()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var message = "Return amount processed successfully";
            _purchasePaymentReturnServiceMock.Setup(s => s.ProcessReturnPaymentAsync(_testPurchaseReturn, branchId, companyId, userId))
                                            .ReturnsAsync(message);

            // Act
            var result = await _controller.ProcessReturnAmount(_testPurchaseReturn, companyId, branchId, userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(message);
            _purchasePaymentReturnServiceMock.Verify(s => s.ProcessReturnPaymentAsync(_testPurchaseReturn, branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            var exceptionMessage = "Processing error";
            _purchasePaymentReturnServiceMock.Setup(s => s.ProcessReturnPaymentAsync(_testPurchaseReturn, branchId, companyId, userId))
                                            .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ProcessReturnAmount(_testPurchaseReturn, companyId, branchId, userId);

            // Assert
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _purchasePaymentReturnServiceMock.Verify(s => s.ProcessReturnPaymentAsync(_testPurchaseReturn, branchId, companyId, userId), Times.Once());
        }

        [Test]
        public async Task ProcessReturnAmount_ShouldReturnBadRequest_WhenReturnAmountDtoIsNull()
        {
            // Arrange
            int companyId = 1, branchId = 1, userId = 1;
            PurchaseReturn nullReturnAmount = null;

            // Act
            var result = await _controller.ProcessReturnAmount(nullReturnAmount, companyId, branchId, userId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid data.");
            _purchasePaymentReturnServiceMock.Verify(s => s.ProcessReturnPaymentAsync(It.IsAny<PurchaseReturn>(), branchId, companyId, userId), Times.Never());
        }
    }
}