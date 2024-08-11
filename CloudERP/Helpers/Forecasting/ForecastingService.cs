using CloudERP.Models.Forecasting;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Helpers.Forecasting
{
    public class ForecastingService
    {
        private readonly CloudDBEntities _context;

        public ForecastingService(CloudDBEntities context)
        {
            _context = context;
        }

        public IEnumerable<ForecastData> GetForecastData(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            // Retrieve the data from the database
            var transactionData = _context.tblTransaction
                .Where(t => t.CompanyID == companyID &&
                            t.BranchID == branchID &&
                            t.TransectionDate >= startDate &&
                            t.TransectionDate <= endDate)
                .AsEnumerable() // Convert to Enumerable to perform LINQ to Objects
                .GroupBy(t => new { Year = t.TransectionDate.Year, Month = t.TransectionDate.Month })
                .Select(g => new
                {
                    NetAmount = g.Sum(t => t.Credit) - g.Sum(t => t.Debit),
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Check if the transaction data is empty
            if (!transactionData.Any())
            {
                return Enumerable.Empty<ForecastData>();
            }

            // Transform the data into the required format
            return transactionData.Select(t => new ForecastData
            {
                Value = (float)t.NetAmount,
                Date = t.Date,
                DateAsNumber = (float)(t.Date - new DateTime(2000, 1, 1)).TotalDays
            });
        }

        public double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var forecastData = GetForecastData(companyID, branchID, startDate, endDate);
            if (!forecastData.Any())
            {
                throw new InvalidOperationException("No data available for forecasting.");
            }

            var forecaster = new FinancialForecaster();
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