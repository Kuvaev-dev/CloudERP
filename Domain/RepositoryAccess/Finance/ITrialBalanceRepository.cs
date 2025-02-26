using Domain.Models.FinancialModels;

namespace Domain.RepositoryAccess
{
    public interface ITrialBalanceRepository
    {
        Task<List<TrialBalanceModel>> GetTrialBalanceAsync(int branchId, int companyId, int financialYearId);
    }
}
