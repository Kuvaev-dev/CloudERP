using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ILedgerRepository
    {
        Task<List<AccountLedgerModel>> GetLedgerAsync(int companyId, int branchId, int financialYearId);
    }
}
