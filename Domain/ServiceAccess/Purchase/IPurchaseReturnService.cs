using Domain.Models.FinancialModels;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IPurchaseReturnService
    {
        Task<(bool IsSuccess, string Message, string InvoiceNo)> ProcessReturnAsync(PurchaseReturnConfirm returnConfirmDto, int branchId, int companyId, int userId);
    }
}
