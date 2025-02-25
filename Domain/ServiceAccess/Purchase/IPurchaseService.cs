using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchaseService
    {
        Task<PurchaseItemDetailDto> GetPurchaseItemDetailAsync(int id);
    }
}
