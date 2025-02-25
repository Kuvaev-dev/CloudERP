using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentReturnService
    {
        Task<IEnumerable<SupplierReturnPayment>> GetSupplierReturnPaymentsAsync(int invoiceId);
        Task<double> GetRemainingAmountAsync(int? invoiceId);
        Task<string> ProcessReturnPaymentAsync(PurchaseReturn returnAmountDto, int branchId, int companyId, int userId);
    }
}
