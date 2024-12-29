using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IBalanceSheetRepository
    {
        Task<double> GetAccountTotalAmountAsync(int companyId, int branchId, int financialYearId, int headId);
        Task<AccountHeadTotal> GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID);
    }
}
