using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICustomerInvoiceDetailRepository
    {
        Task AddSaleDetailsAsync(IEnumerable<SaleCartDetail> saleDetails, int customerInvoiceId);
        Task<IEnumerable<CustomerInvoiceDetail>> GetListByIdAsync(int id);
    }
}
