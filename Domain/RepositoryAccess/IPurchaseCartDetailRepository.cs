using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IPurchaseCartDetailRepository
    {
        Task<List<PurchaseCartDetail>> GetByDefaultSettingsAsync(int branchId, int companyId, int userId);
        Task<List<PurchaseCartDetail>> GetByBranchAndCompanyAsync(int branchId, int companyId);
        Task<PurchaseCartDetail> GetByProductIdAsync(int branchId, int companyId, int productId);
        Task<PurchaseCartDetail> GetByIdAsync(int PCID);
        Task AddAsync(PurchaseCartDetail tblPurchaseCartDetail);
        Task UpdateAsync(PurchaseCartDetail tblPurchaseCartDetail);
        Task DeleteAsync(PurchaseCartDetail tblPurchaseCartDetail);
        Task<bool> IsCanceled(int branchId, int companyId, int userId);

    }
}
