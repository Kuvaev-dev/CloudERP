using CloudERP.Controllers;
using DatabaseAccess;
using DatabaseAccess.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Tests
{
    [TestFixture]
    public class SalePaymentControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private SalePaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _controller = new SalePaymentController(_mockDb.Object);
        }

        private static Mock<DbSet<T>> GetQueryableMockDbSet<T>(IQueryable<T> sourceList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.GetEnumerator());
            return mockSet;
        }

        [Test]
        public void RemainingPaymentList_ReturnsView()
        {
            // Arrange
            var data = new List<tblCustomerInvoice>().AsQueryable();
            _mockDb.Setup(m => m.tblCustomerInvoice).Returns(GetQueryableMockDbSet(data).Object);

            // Act
            var result = _controller.RemainingPaymentList() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.InstanceOf<IEnumerable<tblCustomerInvoice>>());
        }

        [Test]
        public void PaidHistory_ReturnsView()
        {
            // Arrange
            int invoiceId = 1;
            var invoices = new List<tblCustomerInvoice>
            {
                new tblCustomerInvoice { CustomerInvoiceID = invoiceId }
            }.AsQueryable();
            _mockDb.Setup(m => m.tblCustomerInvoice).Returns(GetQueryableMockDbSet(invoices).Object);

            // Act
            var result = _controller.PaidHistory(invoiceId) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.InstanceOf<IEnumerable<SalePaymentModel>>());
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}