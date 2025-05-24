using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchaseCartService
    {
        Task<Result<int>> ConfirmPurchaseAsync(PurchaseConfirm dto);
    }
}
