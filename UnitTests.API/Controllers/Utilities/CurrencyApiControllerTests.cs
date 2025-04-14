using API.Controllers.Utilities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTests.Controllers.API.Utilities
{
    [TestFixture]
    public class CurrencyApiControllerTests
    {
        private Mock<IConfiguration> _configurationMock;
        private CurrencyApiController _controller;
        private const string DefaultCurrency = "UAH";

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["CurrencyApi:DefaultCurrency"]).Returns(DefaultCurrency);
            _controller = new CurrencyApiController(_configurationMock.Object);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CurrencyApiController(null));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenDefaultCurrencyIsMissing()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["CurrencyApi:DefaultCurrency"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CurrencyApiController(configMock.Object));
        }

        [Test]
        public void GetCurrency_ShouldReturnOkWithCurrencyCode_WhenCurrencyCodeIsProvided()
        {
            // Arrange
            string currencyCode = "USD";

            // Act
            var result = _controller.GetCurrency(currencyCode);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(currencyCode);
            _configurationMock.Verify(c => c["CurrencyApi:DefaultCurrency"], Times.Never());
        }

        [Test]
        public void GetCurrency_ShouldReturnOkWithDefaultCurrency_WhenCurrencyCodeIsNull()
        {
            // Arrange
            string currencyCode = null;

            // Act
            var result = _controller.GetCurrency(currencyCode);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(DefaultCurrency);
            _configurationMock.Verify(c => c["CurrencyApi:DefaultCurrency"], Times.Once());
        }

        [Test]
        public void GetCurrency_ShouldReturnOkWithDefaultCurrency_WhenCurrencyCodeIsEmpty()
        {
            // Arrange
            string currencyCode = "";

            // Act
            var result = _controller.GetCurrency(currencyCode);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(DefaultCurrency);
            _configurationMock.Verify(c => c["CurrencyApi:DefaultCurrency"], Times.Once());
        }
    }
}