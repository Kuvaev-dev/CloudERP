using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ITrialBalanceRepository
    {
        Task<List<TrialBalanceModel>> GetTrialBalanceAsync(int branchId, int companyId, int financialYearId);
    }
}
