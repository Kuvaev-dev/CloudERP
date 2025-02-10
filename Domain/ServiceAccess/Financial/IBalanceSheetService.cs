using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IBalanceSheetService
    {
        Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds);
    }
}
