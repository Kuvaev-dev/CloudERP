using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IDashboardService
    {
        Task<DashboardModel> GetDashboardValues(int branchId, int companyId);
    }
}
