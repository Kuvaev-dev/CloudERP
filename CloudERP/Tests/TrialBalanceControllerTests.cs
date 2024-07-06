using CloudERP.Controllers;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using DatabaseAccess;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class TrialBalanceControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private Mock<SP_TrialBalance> _mockTrialBalance;
        private TrialBalanceController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _mockTrialBalance = new Mock<SP_TrialBalance>();

            _controller = new TrialBalanceController(_mockDb.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new Mock<HttpContextBase>().Object
                }
            };
        }

        [Test]
        public void GetTrialBalance_WithValidFinancialYearId_ReturnsViewWithModel()
        {
            // Arrange
            int branchID = 1;
            int companyID = 1;
            int financialYearID = 2024;
            var expectedModel = new List<TrialBalanceModel>();

            _mockTrialBalance.Setup(tb => tb.TriaBalance(branchID, companyID, financialYearID))
                             .Returns(expectedModel);

            // Act
            var result = _controller.GetTrialBalance(financialYearID) as ViewResult;
            var model = result.Model as List<TrialBalanceModel>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("GetTrialBalance"));
            Assert.That(model, Is.EqualTo(expectedModel));
        }

        [Test]
        public void GetTrialBalance_WithInvalidFinancialYearId_RedirectsToEP500()
        {
            // Arrange
            int branchID = 1;
            int companyID = 1;
            int financialYearID = 0; // Invalid financial year ID
            var expectedErrorMessage = "An unexpected error occurred while making changes: Financial year ID is invalid.";

            // Act
            var result = _controller.GetTrialBalance(financialYearID) as RedirectToRouteResult;
            var errorMessage = _controller.ViewBag.ErrorMessage as string;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["controller"], Is.EqualTo("EP"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("EP500"));
            Assert.That(errorMessage, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void GetFinancialYear_ReturnsJsonResultWithFinancialYears()
        {
            // Arrange
            var financialYears = new List<tblFinancialYear>
            {
                new tblFinancialYear { FinancialYearID = 1, FinancialYear = "2023-2024" },
                new tblFinancialYear { FinancialYearID = 2, FinancialYear = "2024-2025" }
            };

            _mockDb.Setup(db => db.tblFinancialYear.ToList()).Returns(financialYears);

            // Act
            var result = _controller.GetFinancialYear() as JsonResult;
            var data = result.Data as List<tblFinancialYear>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count, Is.EqualTo(financialYears.Count));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}