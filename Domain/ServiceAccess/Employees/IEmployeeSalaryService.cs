using Domain.Models.FinancialModels;

namespace Domain.ServiceAccess
{
    public interface IEmployeeSalaryService
    {
        Task<string> ConfirmSalaryAsync(Salary salary, int userId, int branchId, int companyId);
    }
}
