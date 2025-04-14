using API.Controllers.Analytics;
using Domain.Models;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Analytics
{
    [TestFixture]
    public class EmployeeStatisticsApiControllerTests
    {
        private Mock<IEmployeeStatisticsService> _employeeStatsServiceMock;
        private EmployeeStatisticsApiController _controller;
        private List<EmployeeStatistics> _testStatistics;

        [SetUp]
        public void SetUp()
        {
            _employeeStatsServiceMock = new Mock<IEmployeeStatisticsService>();
            _controller = new EmployeeStatisticsApiController(_employeeStatsServiceMock.Object);
            _testStatistics = new List<EmployeeStatistics>
            {
                new EmployeeStatistics { Date = new DateTime(2025, 4, 1), NumberOfRegistrations = 10 },
                new EmployeeStatistics { Date = new DateTime(2025, 4, 2), NumberOfRegistrations = 15 }
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmployeeStatisticsApiController(null));
        }

        [Test]
        public async Task GetEmployeeStatistics_ShouldReturnOkWithStatisticsAndChartData_WhenServiceSucceeds()
        {
            DateTime startDate = new DateTime(2025, 4, 1);
            DateTime endDate = new DateTime(2025, 4, 2);
            int companyId = 1, branchId = 1;
            _employeeStatsServiceMock.Setup(s => s.GetStatisticsAsync(startDate, endDate, branchId, companyId))
                                    .ReturnsAsync(_testStatistics);

            var result = await _controller.GetEmployeeStatistics(startDate, endDate, companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var value = okResult.Value as dynamic;
            value.statistics.Should().BeEquivalentTo(_testStatistics);
            value.chartData.Labels.Should().BeEquivalentTo(new[] { "2025-04-01", "2025-04-02" });
            value.chartData.Data.Should().BeEquivalentTo(new[] { 10, 15 });
            _employeeStatsServiceMock.Verify(s => s.GetStatisticsAsync(startDate, endDate, branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetEmployeeStatistics_ShouldUseDefaultDates_WhenDatesNotProvided()
        {
            int companyId = 0, branchId = 0;
            DateTime defaultStart = DateTime.Now.AddMonths(-1);
            DateTime defaultEnd = DateTime.Now;
            _employeeStatsServiceMock.Setup(s => s.GetStatisticsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), branchId, companyId))
                                    .ReturnsAsync(_testStatistics);

            var result = await _controller.GetEmployeeStatistics(null, null, companyId, branchId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var value = okResult.Value as dynamic;
            value.statistics.Should().BeEquivalentTo(_testStatistics);
            _employeeStatsServiceMock.Verify(s => s.GetStatisticsAsync(It.Is<DateTime>(d => Math.Abs((d - defaultStart).TotalDays) < 1),
                                                                     It.Is<DateTime>(d => Math.Abs((d - defaultEnd).TotalDays) < 1),
                                                                     branchId, companyId), Times.Once());
        }

        [Test]
        public async Task GetEmployeeStatistics_ShouldReturnProblem_WhenServiceThrowsException()
        {
            DateTime startDate = new DateTime(2025, 4, 1);
            DateTime endDate = new DateTime(2025, 4, 2);
            int companyId = 1, branchId = 1;
            var exceptionMessage = "Service error";
            _employeeStatsServiceMock.Setup(s => s.GetStatisticsAsync(startDate, endDate, branchId, companyId))
                                    .ThrowsAsync(new Exception(exceptionMessage));

            var result = await _controller.GetEmployeeStatistics(startDate, endDate, companyId, branchId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeStatsServiceMock.Verify(s => s.GetStatisticsAsync(startDate, endDate, branchId, companyId), Times.Once());
        }
    }
}