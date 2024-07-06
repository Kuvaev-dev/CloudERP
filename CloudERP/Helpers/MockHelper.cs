using Moq;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudERP.Helpers
{
    public class MockHelper
    {
        private Mock<HttpSessionStateBase> _mockSession;

        public void SetMockSessionValue(string key, object value)
        {
            _mockSession.Setup(s => s[key]).Returns(value);
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(IQueryable<T> sourceList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.GetEnumerator());
            
            return mockSet;
        }
    }
}