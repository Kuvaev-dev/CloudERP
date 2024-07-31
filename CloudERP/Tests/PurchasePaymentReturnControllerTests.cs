using CloudERP.Controllers;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DatabaseAccess.Models;
using CloudERP.Helpers;

namespace CloudERP.Tests
{
    [TestFixture]
    public class PurchasePaymentReturnControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private Mock<SP_Purchase> _mockPurchase;
        private Mock<PurchaseEntry> _mockPurchaseEntry;
        private PurchasePaymentReturnController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _mockPurchase = new Mock<SP_Purchase>(_mockDb.Object);
            _mockPurchaseEntry = new Mock<PurchaseEntry>(_mockDb.Object);
            _controller = new PurchasePaymentReturnController(_mockDb.Object)
            {
                _purchase = _mockPurchase.Object,
                _purchaseEntry = _mockPurchaseEntry.Object
            };
        }

        [Test]
        public void ReturnPurchasePendingAmount_SessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Arrange
            _controller.Session["CompanyID"] = null;

            // Act
            var result = _controller.ReturnPurchasePendingAmount(null) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void ReturnPurchasePendingAmount_ValidSession_ReturnsViewWithList()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            var expectedList = new List<SupplierReturnInvoiceModel> { new SupplierReturnInvoiceModel() };
            _mockPurchase.Setup(p => p.PurchaseReturnPaymentPending(It.IsAny<int?>())).Returns(expectedList);

            // Act
            var result = _controller.ReturnPurchasePendingAmount(1) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(expectedList));
        }

        [Test]
        public void AllPurchasesPendingPayment_SessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Arrange
            _controller.Session["CompanyID"] = null;

            // Act
            var result = _controller.AllPurchasesPendingPayment() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void AllPurchasesPendingPayment_ValidSession_ReturnsViewWithList()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            _controller.Session["BranchID"] = "1";
            _controller.Session["UserID"] = "1";
            var expectedList = new List<PurchasePaymentModel> { new PurchasePaymentModel() };
            _mockPurchase.Setup(p => p.GetReturnPurchasesPaymentPending(It.IsAny<int>(), It.IsAny<int>())).Returns(expectedList);

            // Act
            var result = _controller.AllPurchasesPendingPayment() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(expectedList));
        }

        [Test]
        public void ReturnAmount_IdIsNull_RedirectsToAllPurchasesPendingPayment()
        {
            // Arrange
            var list = new List<tblSupplierReturnPayment> { new tblSupplierReturnPayment { RemainingBalance = 0 } }.AsQueryable();
            var mockSupplierReturnPaymentSet = MockHelper.GetQueryableMockDbSet(list);
            _mockDb.Setup(db => db.tblSupplierReturnPayment).Returns(mockSupplierReturnPaymentSet.Object);

            // Act
            var result = _controller.ReturnAmount(null) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("AllPurchasesPendingPayment"));
        }

        [Test]
        public void ReturnAmount_ValidId_ReturnsViewWithList()
        {
            // Arrange
            int id = 1;
            var list = new List<tblSupplierReturnPayment> { new tblSupplierReturnPayment { SupplierReturnInvoiceID = id, RemainingBalance = 50 } }.AsQueryable();
            var mockSupplierReturnPaymentSet = MockHelper.GetQueryableMockDbSet(list);
            _mockDb.Setup(db => db.tblSupplierReturnPayment).Returns(mockSupplierReturnPaymentSet.Object);
            _mockDb.Setup(db => db.tblSupplierReturnInvoice.Find(id)).Returns(new tblSupplierReturnInvoice { TotalAmount = 100 });

            // Act
            var result = _controller.ReturnAmount(id) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(list));
            Assert.That(result.ViewBag.PreviousRemainingAmount, Is.EqualTo(50));
            Assert.That(result.ViewBag.InvoiceID, Is.EqualTo(id));
        }

        [Test]
        public void ReturnAmountPost_SessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Arrange
            _controller.Session["CompanyID"] = null;

            // Act
            var result = _controller.ReturnAmount(1, 50, 10) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void ReturnAmountPost_InvalidPaymentAmount_ReturnsViewWithMessage()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            var id = 1;
            var previousRemainingAmount = 50f;
            var paymentAmount = 60f;
            var list = new List<tblSupplierReturnPayment> { new tblSupplierReturnPayment { SupplierReturnInvoiceID = id, RemainingBalance = previousRemainingAmount } }.AsQueryable();
            var mockSupplierReturnPaymentSet = MockHelper.GetQueryableMockDbSet(list);
            _mockDb.Setup(db => db.tblSupplierReturnPayment).Returns(mockSupplierReturnPaymentSet.Object);

            // Act
            var result = _controller.ReturnAmount(id, previousRemainingAmount, paymentAmount) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewBag.Message, Is.EqualTo("Payment must be less than or equal to the previous remaining amount."));
            Assert.That(result.Model, Is.EqualTo(list));
            Assert.That(result.ViewBag.PreviousRemainingAmount, Is.EqualTo(previousRemainingAmount));
            Assert.That(result.ViewBag.InvoiceID, Is.EqualTo(id));
        }

        [Test]
        public void ReturnAmountPost_ValidPayment_ReturnsRedirectToPurchasePaymentReturn()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            _controller.Session["BranchID"] = "1";
            _controller.Session["UserID"] = "1";
            var id = 1;
            var previousRemainingAmount = 50f;
            var paymentAmount = 40f;
            var supplier = new tblSupplier { SupplierID = 1, SupplierName = "Test Supplier" };
            var invoice = new tblSupplierReturnInvoice { SupplierInvoiceID = 1, SupplierReturnInvoiceID = id, TotalAmount = 100 };
            _mockDb.Setup(db => db.tblSupplierReturnInvoice.Find(id)).Returns(invoice);
            _mockDb.Setup(db => db.tblSupplier.Find(invoice.SupplierID)).Returns(supplier);
            _mockPurchaseEntry.Setup(pe => pe.ReturnPurchasePayment(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>())).Returns("Success");

            // Act
            var result = _controller.ReturnAmount(id, previousRemainingAmount, paymentAmount) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("PurchasePaymentReturn"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(id));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}