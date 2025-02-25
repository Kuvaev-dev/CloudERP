using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISalePaymentReturnService
    {
        Task<string> ProcessReturnAmountAsync(SaleReturn paymentReturnDto, int branchId, int companyId, int userId);
    }
}
