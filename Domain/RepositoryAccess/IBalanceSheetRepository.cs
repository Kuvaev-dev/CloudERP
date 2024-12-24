using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IBalanceSheetRepository
    {
        Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds);
        Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId);
        Task<AccountHeadTotal> GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID);
    }
}
