using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISalePaymentService
    {
        Task<List<SaleInfo>> GetSalePaymentHistoryAsync(int invoiceId);
        Task<double?> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, SaleAmount paymentDto);
    }
}
