using Domain.Models.Forecasting;
using Domain.RepositoryAccess;
using Domain.Services;
using Domain.Services.Interfaces;
using System;
using System.Linq;

namespace DatabaseAccess.Services
{
    public class ForecastingService : IForecastingService
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly IFinancialForecastAdapter _financialForecastAdapter;

        public ForecastingService(IForecastingRepository forecastingRepository, IFinancialForecastAdapter financialForecastAdapter)
        {
            _forecastingRepository = forecastingRepository;
            _financialForecastAdapter = financialForecastAdapter;
        }

        public double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var forecastData = _forecastingRepository.GetForecastData(companyID, branchID, startDate, endDate);
            if (forecastData == null || !forecastData.Any())
            {
                throw new InvalidOperationException("Insufficient data to train the model.");
            }

            _financialForecastAdapter.Train(forecastData);

            var futureDate = DateTime.Now.AddMonths(1);
            var futureData = new ForecastData
            {
                Date = futureDate,
                DateAsNumber = (float)(futureDate - new DateTime(2000, 1, 1)).TotalDays
            };

            return _financialForecastAdapter.Predict(futureData);
        }
    }
}