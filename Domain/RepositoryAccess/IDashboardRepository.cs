using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IDashboardRepository
    {
        Task<DashboardModel> GetDashboardValuesAsync(string fromDate, string toDate, int branchID, int companyID);
    }
}
