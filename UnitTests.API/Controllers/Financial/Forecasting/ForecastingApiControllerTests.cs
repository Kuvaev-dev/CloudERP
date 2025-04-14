using API.Controllers.Financial.Forecasting;
using Domain.Models;
using Domain.RepositoryAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utils.Interfaces;

namespace UnitTests.API.Controllers.Financial.Forecasting
{
    [TestFixture]
    public class ForecastingApiControllerTests
    {
        private Mock<IForecastingRepository> _forecastingRepositoryMock;
        private Mock<IForecastingService> _forecastingServiceMock;
        private ForecastingApiController _controller;
        private ForecastInputModel _testInputModel;
        private double _testForecastValue;

        [SetUp]
        public void SetUp()
        {
            _forecastingRepositoryMock = new Mock<IForecastingRepository>();
            _forecastingServiceMock = new Mock<IForecastingService>();
            _controller = new ForecastingApiController(_forecastingRepositoryMock.Object, _forecastingServiceMock.Object);

            _testInputModel = new ForecastInputModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                ForecastData = new List<ForecastData>
                {
                    new ForecastData { Value = 100, Date = DateTime.Now },
                    new ForecastData { Value = 100, Date = DateTime.Now }
                },
                CompanyID = 1,
                BranchID = 1
            };

            _testForecastValue = 10000.0;
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenForecastingRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ForecastingApiController(null, _forecastingServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenForecastingServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ForecastingApiController(_forecastingRepositoryMock.Object, null));
        }

        [Test]
        public void GenerateForecast_ShouldReturnOkWithForecast_WhenInputIsValid()
        {
            // Arrange
            _forecastingServiceMock.Setup(s => s.GenerateForecast(
                _testInputModel.CompanyID,
                _testInputModel.BranchID,
                It.IsAny<DateTime>(),
                _testInputModel.EndDate))
                .Returns(_testForecastValue);

            // Act
            var result = _controller.GenerateForecast(_testInputModel);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as dynamic;
            response.Success.Should().BeTrue();
            response.Forecast.Should().Be(_testForecastValue);
            _forecastingServiceMock.Verify(s => s.GenerateForecast(
                _testInputModel.CompanyID,
                _testInputModel.BranchID,
                It.Is<DateTime>(d => d.Date == DateTime.Now.Date),
                _testInputModel.EndDate), Times.Once());
        }

        [Test]
        public void GenerateForecast_ShouldReturnBadRequest_WhenInputModelIsNull()
        {
            // Act
            var result = _controller.GenerateForecast(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Invalid forecast input data.");
            _forecastingServiceMock.Verify(s => s.GenerateForecast(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Test]
        public void GenerateForecast_ShouldReturnProblem_WhenServiceThrowsException()
        {
            // Arrange
            var exceptionMessage = "Forecasting error";
            _forecastingServiceMock.Setup(s => s.GenerateForecast(
                _testInputModel.CompanyID,
                _testInputModel.BranchID,
                It.IsAny<DateTime>(),
                _testInputModel.EndDate))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.GenerateForecast(_testInputModel);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
            _forecastingServiceMock.Verify(s => s.GenerateForecast(
                _testInputModel.CompanyID,
                _testInputModel.BranchID,
                It.IsAny<DateTime>(),
                _testInputModel.EndDate), Times.Once());
        }
    }
}