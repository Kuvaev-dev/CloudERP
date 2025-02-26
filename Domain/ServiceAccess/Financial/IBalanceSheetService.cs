using Domain.Models.FinancialModels;

namespace Domain.ServiceAccess
{
    public interface IBalanceSheetService
    {
        Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds);
    }
}
