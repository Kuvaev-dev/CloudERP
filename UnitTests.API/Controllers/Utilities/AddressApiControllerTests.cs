using System.Net;
using API.Controllers.Utilities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace UnitTests.Controllers.API.Utilities
{
    [TestFixture]
    public class AddressApiControllerTests : IDisposable
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private AddressApiController _controller;
        private const string ApiKey = "test-api-key";
        private const string BaseUrl = "https://api.geoapify.com/v1/geocode";

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["GeoapifyApi:Key"]).Returns(ApiKey);
            _configurationMock.Setup(c => c["GeoapifyApi:BaseUrl"]).Returns(BaseUrl);

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _controller = new AddressApiController(_configurationMock.Object);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AddressApiController(null));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenApiKeyIsMissing()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["GeoapifyApi:Key"]).Returns((string)null);
            configMock.Setup(c => c["GeoapifyApi:BaseUrl"]).Returns(BaseUrl);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AddressApiController(configMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenApiUrlIsMissing()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["GeoapifyApi:Key"]).Returns(ApiKey);
            configMock.Setup(c => c["GeoapifyApi:BaseUrl"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AddressApiController(configMock.Object));
        }

        [Test]
        public async Task Autocomplete_ShouldReturnBadRequest_WhenQueryIsNull()
        {
            // Act
            var result = await _controller.Autocomplete(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Query parameter is required.");
        }

        [Test]
        public async Task Autocomplete_ShouldReturnBadRequest_WhenQueryIsEmpty()
        {
            // Act
            var result = await _controller.Autocomplete("   ");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Query parameter is required.");
        }

        [Test]
        public async Task Autocomplete_ShouldReturnContent_WhenApiCallIsSuccessful()
        {
            // Arrange
            var query = "Paris";
            var expectedResponse = "{\"results\": [{\"name\": \"Paris, France\"}]}";
            var expectedUrl = $"{BaseUrl}/autocomplete?text={query}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.Autocomplete(query);

            // Assert
            var contentResult = result as ContentResult;
            contentResult.Should().NotBeNull();
            contentResult.StatusCode.Should().Be(200);
            contentResult.ContentType.Should().Be("application/json");
            contentResult.Content.Should().Be(expectedResponse);
        }

        [Test]
        public async Task Autocomplete_ShouldReturnErrorStatusCode_WhenApiCallFails()
        {
            // Arrange
            var query = "Paris";
            var errorResponse = "{\"error\": \"Invalid query\"}";
            var expectedUrl = $"{BaseUrl}/autocomplete?text={query}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorResponse)
                });

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.Autocomplete(query);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(400);
            statusCodeResult.Value.Should().Be($"Error fetching data from Geoapify API: {errorResponse}");
        }

        [Test]
        public async Task Autocomplete_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            var query = "Paris";
            var exceptionMessage = "Network error";
            var expectedUrl = $"{BaseUrl}/autocomplete?text={query}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception(exceptionMessage));

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.Autocomplete(query);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
        }

        [Test]
        public async Task GetAddressByCoordinates_ShouldReturnContent_WhenApiCallIsSuccessful()
        {
            // Arrange
            double latitude = 48.8566;
            double longitude = 2.3522;
            var expectedResponse = "{\"results\": [{\"formatted\": \"Paris, France\"}]}";
            var expectedUrl = $"{BaseUrl}/reverse?lat={latitude}&lon={longitude}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.GetAddressByCoordinates(latitude, longitude);

            // Assert
            var contentResult = result as ContentResult;
            contentResult.Should().NotBeNull();
            contentResult.StatusCode.Should().Be(200);
            contentResult.ContentType.Should().Be("application/json");
            contentResult.Content.Should().Be(expectedResponse);
        }

        [Test]
        public async Task GetAddressByCoordinates_ShouldReturnErrorStatusCode_WhenApiCallFails()
        {
            // Arrange
            double latitude = 48.8566;
            double longitude = 2.3522;
            var errorResponse = "{\"error\": \"Invalid coordinates\"}";
            var expectedUrl = $"{BaseUrl}/reverse?lat={latitude}&lon={longitude}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorResponse)
                });

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.GetAddressByCoordinates(latitude, longitude);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(400);
            statusCodeResult.Value.Should().Be($"Error fetching data from Geoapify API: {errorResponse}");
        }

        [Test]
        public async Task GetAddressByCoordinates_ShouldReturnProblem_WhenExceptionIsThrown()
        {
            // Arrange
            double latitude = 48.8566;
            double longitude = 2.3522;
            var exceptionMessage = "Network error";
            var expectedUrl = $"{BaseUrl}/reverse?lat={latitude}&lon={longitude}&apiKey={ApiKey}";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString() == expectedUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception(exceptionMessage));

            _controller = new AddressApiController(_configurationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await _controller.GetAddressByCoordinates(latitude, longitude);

            // Assert
            var problemResult = result as ObjectResult;
            problemResult.Should().NotBeNull();
            problemResult.StatusCode.Should().Be(500);
            problemResult.Value.Should().BeOfType<ProblemDetails>()
                         .Which.Detail.Should().Be(exceptionMessage);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}