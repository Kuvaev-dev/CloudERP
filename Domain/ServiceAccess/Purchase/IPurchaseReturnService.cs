using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchaseReturnService
    {
        Task<PurchaseReturnConfirmResult> ProcessReturnAsync(PurchaseReturnConfirm returnConfirmDto);
    }
}
