using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentReturnService
    {
        Task<double> GetRemainingAmountAsync(int? invoiceId);
        Task<string> ProcessReturnPaymentAsync(PurchaseReturn returnAmountDto, int branchId, int companyId, int userId);
    }
}
