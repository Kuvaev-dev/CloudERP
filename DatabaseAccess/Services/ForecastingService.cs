using Domain.Models.Forecasting;
using Domain.RepositoryAccess;
using Microsoft.ML;
using System;
using System.Linq;

namespace CloudERP.Helpers
{
    public class ForecastingService
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly MLContext _mlContext;

        public ForecastingService(IForecastingRepository forecastingRepository)
        {
            _forecastingRepository = forecastingRepository;
            _mlContext = new MLContext();
        }

        public double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var forecastData = _forecastingRepository.GetForecastData(companyID, branchID, startDate, endDate);
            if (!forecastData.Any())
            {
                throw new InvalidOperationException("No data avaliable for forecasting");
            }

            var forecaster = new FinancialForecaster(_mlContext);
            var model = forecaster.TrainModel(forecastData);

            var futureDate = DateTime.Now.AddMonths(1);
            var futureData = new ForecastData
            {
                Date = futureDate,
                DateAsNumber = (float)(futureDate - new DateTime(2000, 1, 1)).TotalDays
            };

            return forecaster.Predict(model, futureData);
        }
    }
}