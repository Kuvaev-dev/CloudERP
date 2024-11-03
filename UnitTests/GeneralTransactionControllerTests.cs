using CloudERP.Controllers;
using CloudERP.Models;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Models;

namespace CloudERP.Tests
{
    [TestFixture]
    public class GeneralTransactionControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private Mock<SP_GeneralTransaction> _mockSPGeneralTransaction;
        private Mock<GeneralTransactionEntry> _mockGeneralTransactionEntry;
        private GeneralTransactionController _controller;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpSessionStateBase> _mockSession;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _mockSPGeneralTransaction = new Mock<SP_GeneralTransaction>(_mockDb.Object);
            _mockGeneralTransactionEntry = new Mock<GeneralTransactionEntry>(_mockDb.Object);

            _controller = new GeneralTransactionController(_mockDb.Object)
            {
                ControllerContext = new ControllerContext()
            };

            _mockHttpContext = new Mock<HttpContextBase>();
            _mockSession = new Mock<HttpSessionStateBase>();
            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);
            _controller.ControllerContext.HttpContext = _mockHttpContext.Object;
        }

        [Test]
        public void GeneralTransaction_Get_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.GeneralTransaction() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void GeneralTransaction_Get_ShouldReturnViewWhenSessionIsNotNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");
            _mockSession.Setup(s => s["UserID"]).Returns("1");

            _mockSPGeneralTransaction.Setup(a => a.GetAllAccounts(It.IsAny<int>(), It.IsAny<int>()))
                                     .Returns(new List<AllAccountModel>());

            // Act
            var result = _controller.GeneralTransaction() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void SaveGeneralTransaction_Post_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            var transaction = new GeneralTransactionMV();
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.SaveGeneralTransaction(transaction) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void SaveGeneralTransaction_Post_ShouldReturnViewWithMessageWhenModelStateIsInvalid()
        {
            // Arrange
            var transaction = new GeneralTransactionMV();
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");
            _controller.ModelState.AddModelError("Error", "Sample Error");

            // Act
            var result = _controller.SaveGeneralTransaction(transaction) as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("GeneralTransaction"));
        }

        [Test]
        public void Journal_Get_ShouldRedirectToLoginWhenSessionIsNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns(null);

            // Act
            var result = _controller.Journal() as RedirectToRouteResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Login"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Home"));
        }

        [Test]
        public void Journal_Get_ShouldReturnViewWhenSessionIsNotNull()
        {
            // Arrange
            _mockSession.Setup(s => s["CompanyID"]).Returns("123");
            _mockSession.Setup(s => s["BranchID"]).Returns("1");
            _mockSession.Setup(s => s["UserID"]).Returns("1");

            _mockSPGeneralTransaction.Setup(a => a.GetJournal(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                     .Returns(new List<JournalModel>());

            // Act
            var result = _controller.Journal() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}