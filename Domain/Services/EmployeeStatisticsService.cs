using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmployeeStatisticsService
    {
        Task<List<EmployeeStatistics>> GetStatisticsAsync(DateTime startDate, DateTime endDate, int branchID, int companyID);
    }

    public class EmployeeStatisticsService : IEmployeeStatisticsService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBranchRepository _branchRepository;

        public EmployeeStatisticsService(IEmployeeRepository employeeRepository, IBranchRepository branchRepository)
        {
            _employeeRepository = employeeRepository;
            _branchRepository = branchRepository;
        }

        public async Task<List<EmployeeStatistics>> GetStatisticsAsync(DateTime startDate, DateTime endDate, int branchID, int companyID)
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
