using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IProductQualityService
    {
        Task<ProductQuality> GetProductQualityAsync(int productId);
        Task<IEnumerable<ProductQuality>> GetAllProductsQualityAsync(int branchId, int companyId);
    }
}
