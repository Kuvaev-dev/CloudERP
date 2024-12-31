using Domain.Models.Forecasting;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public class ForecastingRepository : IForecastingRepository
    {
        private readonly CloudDBEntities _dbContext;

        public ForecastingRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ForecastData> GetForecastData(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            // Retrieve the data from the database
            var transactionData = _dbContext.tblTransaction.Where(t => t.CompanyID == companyID &&
                            t.BranchID == branchID &&
                            t.TransectionDate >= startDate &&
                            t.TransectionDate <= endDate)
                .AsEnumerable() // Convert to Enumerable to perform LINQ to Objects
                .GroupBy(t => new { t.TransectionDate.Year, t.TransectionDate.Month })
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
    }
}
