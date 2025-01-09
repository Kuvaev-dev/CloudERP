using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnPaymentRepository
    {
        Task<IEnumerable<CustomerReturnPayment>> GetListByReturnInvoiceIdAsync(int id);
    }
}
