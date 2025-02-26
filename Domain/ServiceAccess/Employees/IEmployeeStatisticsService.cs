using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IEmployeeStatisticsService
    {
        Task<IEnumerable<EmployeeStatistics>> GetStatisticsAsync(DateTime startDate, DateTime endDate, int branchID, int companyID);
    }
}
