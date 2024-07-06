using CloudERP.Controllers;
using CloudERP.Models;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class CompanyRegistrationControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private CompanyRegistrationController _controller;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpSessionStateBase> _mockSession;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _controller = new CompanyRegistrationController(_mockDb.Object);

            _mockHttpContext = new Mock<HttpContextBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };
        }

        [Test]
        public void RegistrationForm_Get_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.RegistrationForm() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void RegistrationForm_Get_ShouldReturnViewWhenSessionIsNotNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");

            // Act
            var result = _controller.RegistrationForm() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void RegistrationForm_Post_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            var model = new RegistrationMV();
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.RegistrationForm(model) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void RegistrationForm_Post_ShouldReturnViewWithErrorMessageWhenModelStateIsInvalid()
        {
            // Arrange
            var model = new RegistrationMV();
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");
            _controller.ModelState.AddModelError("Error", "Sample Error");

            // Act
            var result = _controller.RegistrationForm(model) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ViewBag.Message, Is.EqualTo("Please provide correct details."));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}