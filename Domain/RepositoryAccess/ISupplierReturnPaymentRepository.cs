using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnPaymentRepository
    {
        Task<IEnumerable<SupplierReturnPayment>> GetBySupplierReturnInvoiceId(int id);
    }
}
