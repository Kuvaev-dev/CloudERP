using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IForecastingRepository
    {
        IEnumerable<ForecastData> GetForecastData(int companyID, int branchID, DateTime startDate, DateTime endDate);
    }
}
