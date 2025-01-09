using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetByBranchAsync(int companyId, int branchId);
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByTINAsync(string TIN);
        Task<Employee> GetByUserIdAsync(int id);
        Task<Employee> GetByCompanyIdAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByDateRangeAsync(DateTime startDate, DateTime endDate, List<int?> branchIDs, int companyID);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task<bool> IsFirstLoginAsync(Employee employee);
    }
}
