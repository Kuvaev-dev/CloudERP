using Domain.Models.Purchase;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IPurchaseService
    {
        Task<PurchaseItemDetailDto> GetPurchaseItemDetailAsync(int id);
    }
}
