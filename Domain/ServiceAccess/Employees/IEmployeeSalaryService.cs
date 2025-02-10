using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IEmployeeSalaryService
    {
        Task<string> ConfirmSalaryAsync(Salary salary, int userId, int branchId, int companyId);
    }
}
