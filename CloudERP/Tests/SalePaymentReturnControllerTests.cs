using CloudERP.Controllers;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class SalePaymentReturnControllerTests
    {
        private SalePaymentReturnController _controller;
        private Mock<CloudDBEntities> _mockDb;
        private Mock<ControllerContext> _mockControllerContext;
        private Mock<HttpSessionStateBase> _mockSession;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _controller = new SalePaymentReturnController(_mockDb.Object);

            _mockSession = new Mock<HttpSessionStateBase>();
            _mockControllerContext = new Mock<ControllerContext>();
            _mockControllerContext.Setup(c => c.HttpContext.Session).Returns(_mockSession.Object);

            _controller.ControllerContext = _mockControllerContext.Object;

            _mockSession.Setup(s => s["CompanyID"]).Returns(null);
        }

        [Test]
        public void ReturnSalePendingAmount_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.ReturnSalePendingAmount(null) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void AllReturnSalesPendingAmount_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.AllReturnSalesPendingAmount() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void ReturnAmount_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.ReturnAmount(null) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void ReturnAmount_Post_WhenSessionCompanyIDIsNull_RedirectsToLogin()
        {
            // Act
            var result = _controller.ReturnAmount(null, 0, 0) as RedirectToRouteResult;

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