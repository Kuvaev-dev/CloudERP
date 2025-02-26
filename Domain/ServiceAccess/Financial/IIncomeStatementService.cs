using Domain.Models.FinancialModels;

namespace Domain.ServiceAccess
{
    public interface IIncomeStatementService
    {
        Task<IncomeStatementModel> GetIncomeStatementAsync(int companyID, int branchID, int financialYearID);
    }
}
