using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnInvoiceDetailRepository
    {
        Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail);
    }
}
