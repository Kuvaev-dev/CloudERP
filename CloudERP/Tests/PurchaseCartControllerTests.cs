using CloudERP.Controllers;
using CloudERP.Helpers;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloudERP.Tests
{
    [TestFixture]
    public class PurchaseCartControllerTests
    {
        private Mock<CloudDBEntities> _mockDbContext;
        private PurchaseCartController _controller;
        private Mock<HttpSessionStateBase> _mockSession;
        private Mock<HttpContextBase> _mockHttpContext;

        [SetUp]
        public void SetUp()
        {
            _mockDbContext = new Mock<CloudDBEntities>();

            _controller = new PurchaseCartController(_mockDbContext.Object);

            _mockSession = new Mock<HttpSessionStateBase>();
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);

            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new RouteData(), _controller);
        }

        [Test]
        public void NewPurchase_Returns_View_With_PurchaseCartDetails()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("1");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");
            _mockSession.Setup(s => s["UserID"]).Returns("1");

            var purchaseCartDetails = new List<tblPurchaseCartDetail>
            {
                new tblPurchaseCartDetail { ProductID = 1, PurchaseQuantity = 1, PurchaseUnitPrice = 100 },
                new tblPurchaseCartDetail { ProductID = 2, PurchaseQuantity = 2, PurchaseUnitPrice = 200 }
            }.AsQueryable();

            _mockDbContext.Setup(db => db.tblPurchaseCartDetail).Returns(MockHelper.GetQueryableMockDbSet(purchaseCartDetails).Object);

            // Act
            var result = _controller.NewPurchase() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<List<tblPurchaseCartDetail>>());
            Assert.That(result.ViewBag.TotalAmount, Is.EqualTo(500));
        }

        [Test]
        public void AddItem_Adds_New_Item_To_PurchaseCart()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("1");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");
            _mockSession.Setup(s => s["UserID"]).Returns("1");

            var purchaseCartDetails = new List<tblPurchaseCartDetail>().AsQueryable();

            _mockDbContext.Setup(db => db.tblPurchaseCartDetail).Returns(MockHelper.GetQueryableMockDbSet(purchaseCartDetails).Object);

            // Act
            var result = _controller.AddItem(1, 2, 100) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _mockDbContext.Verify(db => db.tblPurchaseCartDetail.Add(It.IsAny<tblPurchaseCartDetail>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
            Assert.That(result.RouteValues["action"], Is.EqualTo("NewPurchase"));
        }

        [Test]
        public void GetProduct_Returns_Product_List()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("1");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");

            var products = new List<tblStock>
            {
                new tblStock { ProductID = 1, ProductName = "Product1", BranchID = 1, CompanyID = 1 },
                new tblStock { ProductID = 2, ProductName = "Product2", BranchID = 1, CompanyID = 1 }
            }.AsQueryable();

            _mockDbContext.Setup(db => db.tblStock).Returns(MockHelper.GetQueryableMockDbSet(products).Object);

            // Act
            var result = _controller.GetProduct() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            dynamic data = result.Data;
            Assert.That(data.data.Count, Is.EqualTo(2));
        }

        [Test]
        public void DeleteConfirm_Deletes_Item_From_PurchaseCart()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("1");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");
            _mockSession.Setup(s => s["UserID"]).Returns("1");

            var purchaseCartDetails = new List<tblPurchaseCartDetail>
            {
                new tblPurchaseCartDetail { PurchaseCartDetailID = 1, ProductID = 1, PurchaseQuantity = 1, PurchaseUnitPrice = 100 }
            }.AsQueryable();

            var mockSet = MockHelper.GetQueryableMockDbSet(purchaseCartDetails);
            _mockDbContext.Setup(db => db.tblPurchaseCartDetail).Returns(mockSet.Object);

            var mockEntry = new Mock<DbEntityEntry<tblPurchaseCartDetail>>();
            mockEntry.SetupProperty(e => e.State);
            _mockDbContext.Setup(db => db.Entry(It.IsAny<tblPurchaseCartDetail>())).Returns(mockEntry.Object);

            // Act
            var result = _controller.DeleteConfirm(1) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(mockEntry.Object.State, Is.EqualTo(EntityState.Deleted));
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
            Assert.That(result.RouteValues["action"], Is.EqualTo("NewPurchase"));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}