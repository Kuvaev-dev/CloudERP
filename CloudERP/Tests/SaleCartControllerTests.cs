using CloudERP.Controllers;
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
    public class SaleCartControllerTests
    {
        private Mock<CloudDBEntities> _mockDb;
        private Mock<HttpSessionStateBase> _mockSession;
        private SaleCartController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDb = new Mock<CloudDBEntities>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _controller = new SaleCartController(_mockDb.Object);

            // Set up controller context with mock session
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            var mockResponse = new Mock<HttpResponseBase>();

            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);

            var session = new MockHttpSession();
            mockHttpContext.Setup(x => x.Session).Returns(session);

            _controller.ControllerContext = new ControllerContext(mockHttpContext.Object, new System.Web.Routing.RouteData(), _controller);

            // Set mock session values
            SetMockSessionValue("CompanyID", "1");
            SetMockSessionValue("BranchID", "1");
            SetMockSessionValue("UserID", "1");
        }

        // Helper method to set up mock session
        private void SetMockSessionValue(string key, object value)
        {
            _mockSession.Setup(s => s[key]).Returns(value);
        }

        // Helper class for mock HttpSessionStateBase
        public class MockHttpSession : HttpSessionStateBase
        {
            readonly Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

            public override object this[string name]
            {
                get { return _sessionDictionary[name]; }
                set { _sessionDictionary[name] = value; }
            }
        }

        // Helper method to create a mock DbSet
        private static Mock<System.Data.Entity.DbSet<T>> GetQueryableMockDbSet<T>(IQueryable<T> sourceList) where T : class
        {
            var mockSet = new Mock<System.Data.Entity.DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.GetEnumerator());
            return mockSet;
        }

        [Test]
        public async void NewSale_ReturnsViewWithModel()
        {
            // Arrange
            var data = new List<tblSaleCartDetail>
            {
                new tblSaleCartDetail { BranchID = 1, CompanyID = 1, UserID = 1, SaleQuantity = 1, SaleUnitPrice = 10 },
                new tblSaleCartDetail { BranchID = 1, CompanyID = 1, UserID = 1, SaleQuantity = 2, SaleUnitPrice = 20 }
            }.AsQueryable();

            var mockSet = GetQueryableMockDbSet(data);
            _mockDb.Setup(m => m.tblSaleCartDetail).Returns(mockSet.Object);

            // Act
            var result = await _controller.NewSale() as ViewResult;
            var model = result.Model as List<tblSaleCartDetail>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Empty.Or.EqualTo("NewSale")); // Check view name if specified
            Assert.That(model.Count, Is.EqualTo(2));
            Assert.That(model[0].SaleQuantity, Is.EqualTo(1));
            Assert.That(model[1].SaleQuantity, Is.EqualTo(2));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}