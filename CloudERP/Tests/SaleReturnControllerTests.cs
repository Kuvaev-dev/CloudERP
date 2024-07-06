using CloudERP.Controllers;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class SaleReturnControllerTests
    {
        private SaleReturnController _controller;
        private Mock<CloudDBEntities> _mockDb;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            var mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();

            mockSession.Setup(s => s["CompanyID"]).Returns(null); // Simulate null CompanyID in session
            mockControllerContext.Setup(c => c.HttpContext.Session).Returns(mockSession.Object);

            _controller = new SaleReturnController(_mockDb.Object)
            {
                ControllerContext = mockControllerContext.Object
            };
        }

        [Test]
        public void FindSale_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.FindSale() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void FindSalePost_WhenSessionCompanyIDIsNull_ReturnsViewWithModel()
        {
            // Act
            var result = _controller.FindSale("invoice123") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.TypeOf<tblCustomerInvoice>());
        }

        [Test]
        public void ReturnConfirm_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.ReturnConfirm(new FormCollection()) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}