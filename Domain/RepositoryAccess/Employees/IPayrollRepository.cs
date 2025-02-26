using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IPayrollRepository
    {
        Task<Payroll> GetEmployeePayrollAsync(int employeeID, int branchID, int companyID, string salaryMonth, string salaryYear);
        Task<Payroll> GetLatestPayrollAsync();
        Task<int> GetLatestPayrollIdAsync();
        Task<IEnumerable<Payroll>> GetSalaryHistoryAsync(int branchID, int companyID);
        Task<Payroll> GetByIdAsync(int id);
    }
}
