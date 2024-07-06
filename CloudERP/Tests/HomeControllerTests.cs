using CloudERP.Controllers;
using CloudERP.Helpers;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private Mock<SP_Dashboard> _mockSPDashboard;
        private HomeController _controller;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpSessionStateBase> _mockSession;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _mockSPDashboard = new Mock<SP_Dashboard>(_mockDb.Object);
            _controller = new HomeController(_mockDb.Object)
            {
                ControllerContext = new ControllerContext()
            };

            _mockHttpContext = new Mock<HttpContextBase>();
            _mockSession = new Mock<HttpSessionStateBase>();
            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);
            _controller.ControllerContext.HttpContext = _mockHttpContext.Object;
        }

        [Test]
        public void Index_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
        }

        [Test]
        public void Index_ShouldReturnViewWhenSessionIsNotNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");

            _mockSPDashboard.Setup(d => d.GetDashboardValues(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new DashboardModel());

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Login_ShouldReturnViewWithRememberedEmail()
        {
            // Arrange
            var mockRequest = new Mock<HttpRequestBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var cookie = new HttpCookie("RememberMe");
            cookie.Values["Email"] = "test@example.com";
            mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection { cookie });
            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new System.Web.Routing.RouteData(), _controller);

            _mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            _mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewBag.RememberedEmail, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void LoginUser_ShouldRedirectToIndexWhenCredentialsAreValid()
        {
            // Arrange
            var user = new tblUser { Email = "test@example.com", Password = PasswordHelper.HashPassword("password", out string salt), Salt = salt };
            _mockDb.Setup(db => db.tblUser.SingleOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns(user);
            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new System.Web.Routing.RouteData(), _controller);

            // Act
            var result = _controller.LoginUser("test@example.com", "password", true) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void LoginUser_ShouldReturnViewWithMessageWhenCredentialsAreInvalid()
        {
            // Arrange
            _mockDb.Setup(db => db.tblUser.SingleOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns((tblUser)null);
            _controller.ControllerContext = new ControllerContext(_mockHttpContext.Object, new System.Web.Routing.RouteData(), _controller);

            // Act
            var result = _controller.LoginUser("test@example.com", "password", false) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewBag.Message, Is.EqualTo("Invalid credentials. Please try again."));
        }

        [Test]
        public void Logout_ShouldClearSessionAndReturnLoginView()
        {
            // Arrange
            _mockSession.Setup(s => s.Clear());

            // Act
            var result = _controller.Logout() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("Login"));
        }

        [Test]
        public void ForgotPassword_ShouldReturnEmailSentViewWhenEmailExists()
        {
            // Arrange
            var user = new tblUser { Email = "test@example.com", LastPasswordResetRequest = DateTime.Now.AddMinutes(-10) };
            _mockDb.Setup(db => db.tblUser.FirstOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns(user);

            // Act
            var result = _controller.ForgotPassword("test@example.com") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("ForgotPasswordEmailSent"));
        }

        [Test]
        public void ForgotPassword_ShouldReturnViewWithMessageWhenEmailDoesNotExist()
        {
            // Arrange
            _mockDb.Setup(db => db.tblUser.FirstOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns((tblUser)null);

            // Act
            var result = _controller.ForgotPassword("test@example.com") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("ForgotPassword"));
            Assert.That(result.ViewData.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo("Адрес электронной почты не найден."));
        }

        [Test]
        public void ResetPassword_ShouldReturnViewWhenResetCodeIsValid()
        {
            // Arrange
            var user = new tblUser { ResetPasswordCode = "validcode", ResetPasswordExpiration = DateTime.Now.AddHours(1) };
            _mockDb.Setup(db => db.tblUser.FirstOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns(user);

            // Act
            var result = _controller.ResetPassword("validcode") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ResetPassword_ShouldReturnLinkExpiredViewWhenResetCodeIsInvalid()
        {
            // Arrange
            _mockDb.Setup(db => db.tblUser.FirstOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns((tblUser)null);

            // Act
            var result = _controller.ResetPassword("invalidcode") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("ResetPasswordLinkExpired"));
        }

        [Test]
        public void ResetPassword_Post_ShouldReturnSuccessViewWhenPasswordsMatch()
        {
            // Arrange
            var user = new tblUser { ResetPasswordCode = "validcode", ResetPasswordExpiration = DateTime.Now.AddHours(1) };
            _mockDb.Setup(db => db.tblUser.FirstOrDefault(It.IsAny<Func<tblUser, bool>>())).Returns(user);

            // Act
            var result = _controller.ResetPassword("validcode", "newpassword", "newpassword") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("ResetPasswordSuccess"));
        }

        [Test]
        public void ResetPassword_Post_ShouldReturnViewWithMessageWhenPasswordsDoNotMatch()
        {
            // Act
            var result = _controller.ResetPassword("validcode", "newpassword", "differentpassword") as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewData.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo("Пароли не совпадают."));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}