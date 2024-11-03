using CloudERP.Controllers;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class PurchaseReturnControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private PurchaseReturnController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _controller = new PurchaseReturnController(_mockDb.Object);
        }

        [Test]
        public void FindPurchase_SessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Arrange
            _controller.Session["CompanyID"] = null;

            // Act
            var result = _controller.FindPurchase() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void FindPurchase_ValidSession_ReturnsViewWithInvoice()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            var invoiceId = 1;
            var mockInvoice = new tblSupplierInvoice { SupplierInvoiceID = invoiceId, InvoiceNo = "INV001" };
            _mockDb.Setup(m => m.tblSupplierInvoice.Find(invoiceId)).Returns(mockInvoice);

            // Act
            var result = _controller.FindPurchase("INV001") as ViewResult;
            var model = result.Model as tblSupplierInvoice;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.EqualTo(mockInvoice));
        }

        [Test]
        public void ReturnConfirm_ValidReturn_ReturnsRedirectToFindPurchase()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            _controller.Session["BranchID"] = "1";
            _controller.Session["UserID"] = "1";
            var supplierInvoiceId = 1;
            var collection = new FormCollection
        {
            { "supplierInvoiceID", "1" },
            { "ProductID 1", "1,2" },
            { "ReturnQty 1", "2" },
            { "IsPayment", "on" }
        };
            var mockSupplierInvoice = new tblSupplierInvoice { SupplierInvoiceID = supplierInvoiceId };
            var mockSupplierReturnInvoice = new tblSupplierReturnInvoice { SupplierReturnInvoiceID = 1 };
            var mockPurchaseDetails = new List<tblSupplierInvoiceDetail>
        {
            new tblSupplierInvoiceDetail { SupplierInvoiceDetailID = 1, ProductID = 1, PurchaseUnitPrice = 10 },
            new tblSupplierInvoiceDetail { SupplierInvoiceDetailID = 2, ProductID = 2, PurchaseUnitPrice = 15 }
        };
            _mockDb.Setup(m => m.tblSupplierInvoice.Find(supplierInvoiceId)).Returns(mockSupplierInvoice);
            _mockDb.Setup(m => m.tblSupplierInvoiceDetail.Where(pd => pd.SupplierInvoiceID == supplierInvoiceId)).Returns(mockPurchaseDetails.AsQueryable());
            _mockDb.Setup(m => m.tblSupplierReturnInvoice.Add(It.IsAny<tblSupplierReturnInvoice>())).Returns(mockSupplierReturnInvoice);
            _mockDb.Setup(m => m.SaveChanges()).Returns(1);

            // Act
            var result = _controller.ReturnConfirm(collection) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("FindPurchase"));
        }

        [Test]
        public void ReturnConfirm_NoProductsToReturn_ReturnsRedirectToFindPurchaseWithMessage()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            var supplierInvoiceId = 1;
            var collection = new FormCollection
        {
            { "supplierInvoiceID", "1" }
        };
            var mockSupplierInvoice = new tblSupplierInvoice { SupplierInvoiceID = supplierInvoiceId };
            _mockDb.Setup(m => m.tblSupplierInvoice.Find(supplierInvoiceId)).Returns(mockSupplierInvoice);

            // Act
            var result = _controller.ReturnConfirm(collection) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("FindPurchase"));
            Assert.That(_controller.Session["ReturnMessage"], Is.EqualTo("Must be at least One Product Return Qty"));
        }

        [Test]
        public void ReturnConfirm_ExceptionOccurs_RedirectsToEP500()
        {
            // Arrange
            _controller.Session["CompanyID"] = "1";
            var collection = new FormCollection();
            _mockDb.Setup(m => m.SaveChanges()).Throws(new Exception("Test exception"));

            // Act
            var result = _controller.ReturnConfirm(collection) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("EP500"));
            Assert.That(_controller.ViewBag.ErrorMessage, Is.EqualTo("An unexpected error occurred while making changes: Test exception"));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}