using Domain.Models.FinancialModels;
using System.Threading.Tasks;
using Utils.Helpers;

namespace Domain.ServiceAccess
{
    public interface ISaleCartService
    {
        Task<Result<int>> ConfirmSaleAsync(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId);
    }
}
