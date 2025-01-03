using System;

namespace Domain.Interfaces
{
    public interface IForecastingService
    {
        double GenerateForecast(int companyID, int branchID, DateTime startDate, DateTime endDate);
    }
}
