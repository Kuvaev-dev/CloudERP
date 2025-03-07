using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class EmployeeStatisticsService : IEmployeeStatisticsService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBranchRepository _branchRepository;

        public EmployeeStatisticsService(
            IEmployeeRepository employeeRepository,
            IBranchRepository branchRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
        }

        public async Task<IEnumerable<EmployeeStatistics>> GetStatisticsAsync(DateTime startDate, DateTime endDate, int branchID, int companyID)
        {
            var branchIDs = await _branchRepository.GetBranchIDsAsync(branchID);
            var employees = await _employeeRepository.GetEmployeesByDateRangeAsync(startDate, endDate, branchIDs, companyID);

            return employees
                .GroupBy(e => e.RegistrationDate.Value.Date)
                .Select(g => new EmployeeStatistics
                {
                    Date = g.Key,
                    NumberOfRegistrations = g.Count()
                })
                .ToList();
        }
    }
}
