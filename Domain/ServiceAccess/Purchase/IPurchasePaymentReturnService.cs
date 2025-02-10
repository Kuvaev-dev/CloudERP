using Domain.Models.FinancialModels;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IPurchasePaymentReturnService
    {
        Task<IEnumerable<SupplierReturnPayment>> GetSupplierReturnPaymentsAsync(int invoiceId);
        Task<double> GetRemainingAmountAsync(int invoiceId);
        Task<string> ProcessReturnPaymentAsync(PurchaseReturnAmount returnAmountDto, int branchId, int companyId, int userId);
    }
}
