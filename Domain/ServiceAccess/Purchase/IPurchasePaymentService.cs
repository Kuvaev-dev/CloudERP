using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentService
    {
        Task<(IEnumerable<object> PaymentHistory, IEnumerable<object> ReturnDetails, double RemainingAmount)> GetPaymentDetailsAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, PurchasePayment paymentDto);
        Task<List<PurchasePaymentModel>> GetPurchasePaymentHistoryAsync(int invoiceId);
        Task<double> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
    }
}
