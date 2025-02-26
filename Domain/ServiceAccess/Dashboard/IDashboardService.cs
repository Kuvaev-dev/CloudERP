using Domain.Models.FinancialModels;

namespace Domain.ServiceAccess
{
    public interface IDashboardService
    {
        Task<DashboardModel> GetDashboardValues(int branchId, int companyId);
    }
}
