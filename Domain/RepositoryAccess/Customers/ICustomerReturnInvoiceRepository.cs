using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerReturnInvoiceRepository
    {
        Task<IEnumerable<CustomerReturnInvoice>> GetReturnDetails(int id);
        Task<CustomerReturnInvoice> GetByIdAsync(int id);
        Task<double?> GetTotalAmountByIdAsync(int id);
        Task AddAsync(CustomerReturnInvoice customerReturnInvoice);
    }
}
