using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnPaymentRepository
    {
        Task<IEnumerable<SupplierReturnPayment>> GetBySupplierReturnInvoiceId(int id);
    }
}
