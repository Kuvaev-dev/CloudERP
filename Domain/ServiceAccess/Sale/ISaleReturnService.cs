using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISaleReturnService
    {
        Task<(bool IsSuccess, string Message, string InvoiceNo)> ProcessReturnConfirmAsync(SaleReturnConfirm returnConfirmDto, int branchId, int companyId, int userId);
    }
}
