using API.Controllers.Analytics;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.API.Controllers.Analytics
{
    [TestFixture]
    public class AnalyticsApiControllerTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<IStockRepository> _stockRepositoryMock;
        private Mock<ISupportTicketRepository> _supportTicketRepositoryMock;
        private AnalyticsApiController _controller;
        private AnalyticsModel _expectedModel;

        [SetUp]
        public void SetUp()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _stockRepositoryMock = new Mock<IStockRepository>();
            _supportTicketRepositoryMock = new Mock<ISupportTicketRepository>();
            _controller = new AnalyticsApiController(
                _employeeRepositoryMock.Object,
                _stockRepositoryMock.Object,
                _supportTicketRepositoryMock.Object);
            _expectedModel = new AnalyticsModel
            {
                TotalEmployees = 100,
                NewEmployeesThisMonth = 10,
                NewEmployeesThisYear = 50,
                TotalStockItems = 1000,
                StockAvailable = 800,
                StockExpired = 50,
                TotalSupportTickets = 200,
                ResolvedSupportTickets = 150,
                PendingSupportTickets = 50
            };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenEmployeeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsApiController(null, _stockRepositoryMock.Object, _supportTicketRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenStockRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsApiController(_employeeRepositoryMock.Object, null, _supportTicketRepositoryMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenSupportTicketRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsApiController(_employeeRepositoryMock.Object, _stockRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetAnalytics_ShouldReturnOkWithAnalyticsModel_WhenRepositoriesSucceed()
        {
            int companyId = 1;
            _employeeRepositoryMock.Setup(r => r.GetCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.TotalEmployees);
            _employeeRepositoryMock.Setup(r => r.GetMonthNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisMonth);
            _employeeRepositoryMock.Setup(r => r.GetYearNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisYear);
            _stockRepositoryMock.Setup(r => r.GetTotalStockItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.TotalStockItems);
            _stockRepositoryMock.Setup(r => r.GetTotalAvaliableItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.StockAvailable);
            _stockRepositoryMock.Setup(r => r.GetTotalExpiredItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.StockExpired);
            _supportTicketRepositoryMock.Setup(r => r.GetTotalSupportTicketsByCompany(companyId))
                                        .ReturnsAsync(_expectedModel.TotalSupportTickets);
            _supportTicketRepositoryMock.Setup(r => r.GetTotalResolvedSupportTicketsByCompany(companyId))
                                        .ReturnsAsync(_expectedModel.ResolvedSupportTickets);
            _supportTicketRepositoryMock.Setup(r => r.GetTotalPendingSupportTicketsByCompany(companyId))
                                        .ReturnsAsync(_expectedModel.PendingSupportTickets);

            var result = await _controller.GetAnalytics(companyId);
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(_expectedModel);
            _employeeRepositoryMock.Verify(r => r.GetCountByCompanyAsync(companyId), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetMonthNewEmployeesCountByCompanyAsync(companyId), Times.Once());
            _employeeRepositoryMock.Verify(r => r.GetYearNewEmployeesCountByCompanyAsync(companyId), Times.Once());
            _stockRepositoryMock.Verify(r => r.GetTotalStockItemsByCompanyAsync(companyId), Times.Once());
            _stockRepositoryMock.Verify(r => r.GetTotalAvaliableItemsByCompanyAsync(companyId), Times.Once());
            _stockRepositoryMock.Verify(r => r.GetTotalExpiredItemsByCompanyAsync(companyId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.GetTotalSupportTicketsByCompany(companyId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.GetTotalResolvedSupportTicketsByCompany(companyId), Times.Once());
            _supportTicketRepositoryMock.Verify(r => r.GetTotalPendingSupportTicketsByCompany(companyId), Times.Once());
        }

        [Test]
        public async Task GetAnalytics_ShouldReturnProblem_WhenEmployeeRepositoryThrowsException()
        {
            int companyId = 1;
            var exceptionMessage = "Employee database error";
            _employeeRepositoryMock.Setup(r => r.GetCountByCompanyAsync(companyId))
                                   .ThrowsAsync(new Exception(exceptionMessage));

            var result = await _controller.GetAnalytics(companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _employeeRepositoryMock.Verify(r => r.GetCountByCompanyAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetAnalytics_ShouldReturnProblem_WhenStockRepositoryThrowsException()
        {
            int companyId = 1;
            _employeeRepositoryMock.Setup(r => r.GetCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.TotalEmployees);
            _employeeRepositoryMock.Setup(r => r.GetMonthNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisMonth);
            _employeeRepositoryMock.Setup(r => r.GetYearNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisYear);
            var exceptionMessage = "Stock database error";
            _stockRepositoryMock.Setup(r => r.GetTotalStockItemsByCompanyAsync(companyId))
                                .ThrowsAsync(new Exception(exceptionMessage));

            var result = await _controller.GetAnalytics(companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _stockRepositoryMock.Verify(r => r.GetTotalStockItemsByCompanyAsync(companyId), Times.Once());
        }

        [Test]
        public async Task GetAnalytics_ShouldReturnProblem_WhenSupportTicketRepositoryThrowsException()
        {
            int companyId = 1;
            _employeeRepositoryMock.Setup(r => r.GetCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.TotalEmployees);
            _employeeRepositoryMock.Setup(r => r.GetMonthNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisMonth);
            _employeeRepositoryMock.Setup(r => r.GetYearNewEmployeesCountByCompanyAsync(companyId))
                                   .ReturnsAsync(_expectedModel.NewEmployeesThisYear);
            _stockRepositoryMock.Setup(r => r.GetTotalStockItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.TotalStockItems);
            _stockRepositoryMock.Setup(r => r.GetTotalAvaliableItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.StockAvailable);
            _stockRepositoryMock.Setup(r => r.GetTotalExpiredItemsByCompanyAsync(companyId))
                                .ReturnsAsync(_expectedModel.StockExpired);
            var exceptionMessage = "Support ticket database error";
            _supportTicketRepositoryMock.Setup(r => r.GetTotalSupportTicketsByCompany(companyId))
                                        .ThrowsAsync(new Exception(exceptionMessage));

            var result = await _controller.GetAnalytics(companyId);
            var problemResult = result.Result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _supportTicketRepositoryMock.Verify(r => r.GetTotalSupportTicketsByCompany(companyId), Times.Once());
        }
    }
}