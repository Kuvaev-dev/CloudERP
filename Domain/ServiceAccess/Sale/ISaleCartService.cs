using Domain.Models;
using Utils.Helpers;

namespace Domain.ServiceAccess
{
    public interface ISaleCartService
    {
        Task<Result<int>> ConfirmSaleAsync(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId);
    }
}
