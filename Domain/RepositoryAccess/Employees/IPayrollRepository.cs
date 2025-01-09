using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IPayrollRepository
    {
        Task<Payroll> GetEmployeePayrollAsync(int employeeID, int branchID, int companyID, string salaryMonth, string salaryYear);
        Task<int> GetLatestPayrollAsync();
        Task<IEnumerable<Payroll>> GetSalaryHistoryAsync(int branchID, int companyID);
        Task<Payroll> GetByIdAsync(int id);
    }
}
