using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetByBranchAsync(int companyId, int branchId);
        Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId);
        Task<IEnumerable<Employee>> GetEmployeesForTaskAssignmentAsync(int branchId, int companyId);
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByTINAsync(string TIN);
        Task<Employee> GetByUserIdAsync(int id);
        Task<Employee?> GetByContactAsync(string contact);
        Task<IEnumerable<Employee>> GetEmployeesByDateRangeAsync(DateTime startDate, DateTime endDate, List<int> branchIDs, int companyID);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task<bool> IsFirstLoginAsync(Employee employee);
        Task<bool> IsExists(Employee employee);
        Task<int> GetCountByCompanyAsync(int companyId);
        Task<int> GetMonthNewEmployeesCountByCompanyAsync(int companyId);
        Task<int> GetYearNewEmployeesCountByCompanyAsync(int companyId);
    }
}
