using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnInvoiceRepository
    {
        Task<IEnumerable<CustomerReturnInvoice>> GetListByIdAsync(int id);
        Task<CustomerReturnInvoice> GetByIdAsync(int id);
        Task<double> GetTotalAmountByIdAsync(int id);
        Task AddAsync(CustomerReturnInvoice customerReturnInvoice);
    }
}
