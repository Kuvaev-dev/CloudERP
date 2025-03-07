using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Utils.Interfaces;

namespace Services.Implementations
{
    public class ForecastingService : IForecastingService
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly IFinancialForecastAdapter _financialForecastAdapter;

        public ForecastingService(IForecastingRepository forecastingRepository, IFinancialForecastAdapter financialForecastAdapter)
        {
            _forecastingRepository = forecastingRepository ?? throw new ArgumentException(nameof(forecastingRepository));
            _financialForecastAdapter = financialForecastAdapter ?? throw new ArgumentException(nameof(financialForecastAdapter));
        }

        public double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var forecastData = _forecastingRepository.GetForecastData(companyID, branchID, startDate, endDate);

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