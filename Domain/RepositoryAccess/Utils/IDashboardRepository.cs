using Domain.Models.FinancialModels;

namespace Domain.RepositoryAccess
{
    public interface IDashboardRepository
    {
        Task<DashboardModel> GetDashboardValuesAsync(string fromDate, string toDate, int branchID, int companyID);
    }
}
