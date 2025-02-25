using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISaleService
    {
        Task<SaleItemDetailDto> GetSaleItemDetailAsync(int id);
    }
}
