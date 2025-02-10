using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using System.Threading.Tasks;

namespace Domain.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(IDashboardRepository));
        }

        public async Task<DashboardModel> GetDashboardValues(int branchId, int companyId)
        {
            DateTime currentDate = DateTime.Today;
            string fromDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToString("yyyy-MM-dd");
            string toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)).ToString("yyyy-MM-dd");

            return await _dashboardRepository.GetDashboardValuesAsync(fromDate, toDate, branchId, companyId);
        }
    }
}
