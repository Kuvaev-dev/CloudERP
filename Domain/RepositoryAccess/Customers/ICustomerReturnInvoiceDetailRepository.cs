using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnInvoiceDetailRepository
    {
        Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail);
        List<CustomerInvoiceDetail> GetInvoiceDetails(string invoiceId);
    }
}
