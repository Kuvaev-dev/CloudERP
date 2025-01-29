using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnInvoiceDetailRepository
    {
        Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail);
        List<CustomerInvoiceDetail> GetInvoiceDetails(string invoiceId);
    }
}
