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

        public IEnumerable<ForecastData> GetForecastData(int companyID, int branchID)
        {
            var financialData = _context.tblPayroll
                .Where(p => p.CompanyID == companyID && p.BranchID == branchID)
                .Select(p => new ForecastData
                {
                    Value = p.TransferAmount,
                    Date = p.PaymentDate
                })
                .ToList();

            return financialData;
        }

        public void GenerateForecast(int companyID, int branchID)
        {
            var financialData = _context.tblPayroll
                .Where(p => p.CompanyID == companyID && p.BranchID == branchID)
                .Select(p => new ForecastData
                {
                    Value = p.TransferAmount,
                    Date = p.PaymentDate
                }).ToList();

            var forecaster = new FinancialForecaster();
            var model = forecaster.TrainModel(financialData);

            var forecastData = new ForecastData
            {
                Date = DateTime.Now.AddMonths(1)
            };

            var prediction = forecaster.Predict(model, forecastData);
        }
    }
}