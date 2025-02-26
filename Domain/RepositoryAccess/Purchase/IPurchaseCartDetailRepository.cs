using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IPurchaseCartDetailRepository
    {
        Task<IEnumerable<PurchaseCartDetail>> GetByDefaultSettingsAsync(int branchId, int companyId, int userId);
        Task<IEnumerable<PurchaseCartDetail>> GetByBranchAndCompanyAsync(int branchId, int companyId);
        Task<IEnumerable<PurchaseCartDetail>> GetAllAsync(int branchId, int companyId);
        Task<PurchaseCartDetail> GetByProductIdAsync(int branchId, int companyId, int productId);
        Task<PurchaseCartDetail> GetByIdAsync(int PCID);
        Task AddAsync(PurchaseCartDetail tblPurchaseCartDetail);
        Task UpdateAsync(PurchaseCartDetail tblPurchaseCartDetail);
        Task DeleteAsync(int detailID);
        Task DeleteListAsync(IEnumerable<PurchaseCartDetail> purchaseCartDetails);
        Task<bool> IsCanceled(int branchId, int companyId, int userId);
    }
}
