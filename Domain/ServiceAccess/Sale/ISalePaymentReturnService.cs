using Domain.Models.FinancialModels;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface ISalePaymentReturnService
    {
        Task<(bool IsSuccess, string Message, IEnumerable<CustomerReturnPayment> Items, double RemainingAmount)> 
            ProcessReturnAmountAsync(SalePaymentReturn paymentReturnDto, int branchId, int companyId, int userId);
    }
}
