using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IIncomeStatementService
    {
        Task<IncomeStatementModel> GetIncomeStatementAsync(int companyID, int branchID, int financialYearID);
    }
}
