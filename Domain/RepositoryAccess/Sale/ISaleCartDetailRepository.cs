using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISaleCartDetailRepository
    {
        Task<IEnumerable<SaleCartDetail>> GetAllAsync(int branchId, int companyId);
        Task<IEnumerable<SaleCartDetail>> GetByDefaultSettingAsync(int branchId, int companyId, int userId);
        Task<SaleCartDetail> GetByCompanyAndBranchAsync(int branchId, int companyId);
        Task<SaleCartDetail> GetByProductIdAsync(int productId, int branchId, int companyId);
        Task<SaleCartDetail> GetByIdAsync(int id);
        Task AddAsync(SaleCartDetail saleCartDetail);
        Task UpdateAsync(SaleCartDetail saleCartDetail);
        Task DeleteAsync(int detailID);
        Task DeleteListAsync(IEnumerable<SaleCartDetail> saleCartDetails);
    }
}
