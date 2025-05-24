using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentService
    {
        Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchaseAmount paymentDto);
    }
}
