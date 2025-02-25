using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnPaymentRepository
    {
        Task<IEnumerable<CustomerReturnPayment>> GetByCustomerReturnInvoiceId(int id);
    }
}
