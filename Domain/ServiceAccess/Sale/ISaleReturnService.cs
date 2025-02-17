using Domain.Models;
using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISaleReturnService
    {
        Task<ReturnConfirmResult> ProcessReturnConfirmAsync(SaleReturnConfirm returnConfirmDto, int branchId, int companyId, int userId);
    }
}
