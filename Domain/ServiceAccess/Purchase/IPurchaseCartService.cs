using Domain.Models.FinancialModels;
using System.Threading.Tasks;
using Utils.Helpers;

namespace Domain.ServiceAccess
{
    public interface IPurchaseCartService
    {
        Task<Result<int>> ConfirmPurchaseAsync(PurchaseConfirm dto, int companyId, int branchId, int userId);
    }
}
