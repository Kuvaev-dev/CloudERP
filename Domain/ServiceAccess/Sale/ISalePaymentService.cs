using Domain.Models.FinancialModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISalePaymentService
    {
        Task<List<SalePaymentModel>> GetSalePaymentHistoryAsync(int invoiceId);
        Task<double> GetTotalAmountByIdAsync(int invoiceId);
        Task<double> GetTotalPaidAmountByIdAsync(int invoiceId);
        Task<string> ProcessPaymentAsync(SalePayment paymentDto, int branchId, int companyId, int userId);
    }
}
