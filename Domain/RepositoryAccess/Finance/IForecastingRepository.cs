using Domain.Models.Forecasting;
using System;
using System.Collections.Generic;

namespace Domain.RepositoryAccess
{
    public interface IForecastingRepository
    {
        IEnumerable<ForecastData> GetForecastData(int companyID, int branchID, DateTime startDate, DateTime endDate);
    }
}
