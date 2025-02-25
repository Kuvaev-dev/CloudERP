using Domain.Models;
using Utils.Helpers;

namespace Domain.ServiceAccess
{
    public interface IPurchaseCartService
    {
        Task<Result<int>> ConfirmPurchaseAsync(PurchaseConfirm dto, int companyId, int branchId, int userId);
    }
}
