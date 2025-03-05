using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentService
    {
        Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchaseAmount paymentDto);
        Task<List<PurchaseInfo>> GetPurchasePaymentHistoryAsync(int invoiceId);
        Task<double> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
    }
}
