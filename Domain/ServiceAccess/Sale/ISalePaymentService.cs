using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface ISalePaymentService
    {
        Task<string> ProcessPaymentAsync(int companyId, int branchId, int userId, SaleAmount paymentDto);
    }
}
