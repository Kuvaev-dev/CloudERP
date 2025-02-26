using Domain.Models.FinancialModels;

namespace Domain.RepositoryAccess
{
    public interface ILedgerRepository
    {
        Task<List<AccountLedgerModel>> GetLedgerAsync(int companyId, int branchId, int financialYearId);
    }
}
