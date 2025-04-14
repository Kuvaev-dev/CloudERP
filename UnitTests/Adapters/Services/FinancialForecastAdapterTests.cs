using Domain.Models;
using Domain.ServiceAccess;
using FluentAssertions;
using Microsoft.ML;
using Moq;
using Services.Adapters;

namespace UnitTests.Adapters.Services
{
    [TestFixture]
    public class FinancialForecastAdapterTests
    {
        private Mock<IFinancialForecaster> _financialForecasterMock;
        private Mock<ITransformer> _transformerMock;
        private FinancialForecastAdapter _adapter;
        private List<ForecastData> _testData;
        private ForecastData _singleTestData;

        [SetUp]
        public void SetUp()
        {
            _financialForecasterMock = new Mock<IFinancialForecaster>();
            _transformerMock = new Mock<ITransformer>();
            _adapter = new FinancialForecastAdapter(_financialForecasterMock.Object);
            _testData = new List<ForecastData>
            {
                new ForecastData { Value = 100, Date = DateTime.Now },
                new ForecastData { Value = 200, Date = DateTime.Now.AddDays(1) }
            };
            _singleTestData = new ForecastData { Value = 150, Date = DateTime.Now };
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFinancialForecasterIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FinancialForecastAdapter(null));
        }

        [Test]
        public void Constructor_ShouldInitializeFinancialForecaster_WhenValid()
        {
            // Act
            var adapter = new FinancialForecastAdapter(_financialForecasterMock.Object);

            // Assert
            adapter.Should().NotBeNull();
        }

        [Test]
        public void Train_ShouldCallTrainModel_WhenDataIsValid()
        {
            // Arrange
            _financialForecasterMock.Setup(f => f.TrainModel(It.IsAny<IEnumerable<ForecastData>>()))
                                   .Returns(_transformerMock.Object);

            // Act
            _adapter.Train(_testData);

            // Assert
            _financialForecasterMock.Verify(f => f.TrainModel(It.Is<IEnumerable<ForecastData>>(d => d == _testData)), Times.Once());
        }

        [Test]
        public void Train_ShouldSetTrainedModel_WhenTrainModelSucceeds()
        {
            // Arrange
            _financialForecasterMock.Setup(f => f.TrainModel(It.IsAny<IEnumerable<ForecastData>>()))
                                   .Returns(_transformerMock.Object);

            // Act
            _adapter.Train(_testData);

            // Assert
            _financialForecasterMock.Verify(f => f.TrainModel(It.IsAny<IEnumerable<ForecastData>>()), Times.Once());
        }

        [Test]
        public void Train_ShouldThrowArgumentNullException_WhenDataIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _adapter.Train(null));
        }

        [Test]
        public void Predict_ShouldReturnPrediction_WhenModelIsTrained()
        {
            // Arrange
            double expectedPrediction = 300.0;
            _financialForecasterMock.Setup(f => f.TrainModel(It.IsAny<IEnumerable<ForecastData>>()))
                                   .Returns(_transformerMock.Object);
            _financialForecasterMock.Setup(f => f.Predict(_transformerMock.Object, _singleTestData))
                                   .Returns(expectedPrediction);
            _adapter.Train(_testData); // Train model to set _trainedModel

            // Act
            var result = _adapter.Predict(_singleTestData);

            // Assert
            result.Should().Be(expectedPrediction);
            _financialForecasterMock.Verify(f => f.Predict(_transformerMock.Object, _singleTestData), Times.Once());
        }

        [Test]
        public void Predict_ShouldThrowInvalidOperationException_WhenModelIsNotTrained()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _adapter.Predict(_singleTestData));
        }

        [Test]
        public void Predict_ShouldThrowArgumentNullException_WhenDataIsNull()
        {
            // Arrange
            _financialForecasterMock.Setup(f => f.TrainModel(It.IsAny<IEnumerable<ForecastData>>()))
                                   .Returns(_transformerMock.Object);
            _adapter.Train(_testData); // Train model to set _trainedModel

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _adapter.Predict(null));
        }
    }
}