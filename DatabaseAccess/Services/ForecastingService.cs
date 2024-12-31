using Domain.Models.Forecasting;
using Domain.RepositoryAccess;
using Domain.Services;
using Microsoft.ML;
using System;
using System.Linq;

namespace DatabaseAccess.Services
{
    public class ForecastingService : IForecastingService
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly IFinancialForecaster _financialForecaster;
        private readonly MLContext _mlContext;

        public ForecastingService(IForecastingRepository forecastingRepository)
        {
            _forecastingRepository = forecastingRepository;
            _mlContext = new MLContext();
            _financialForecaster = new FinancialForecaster(_mlContext);
        }

        public double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var forecastData = _forecastingRepository.GetForecastData(companyID, branchID, startDate, endDate);

            var model = _financialForecaster.TrainModel(forecastData);

            var futureDate = DateTime.Now.AddMonths(1);
            var futureData = new ForecastData
            {
                Date = futureDate,
                DateAsNumber = (float)(futureDate - new DateTime(2000, 1, 1)).TotalDays
            };

            return _financialForecaster.Predict(model, futureData);
        }
    }
}