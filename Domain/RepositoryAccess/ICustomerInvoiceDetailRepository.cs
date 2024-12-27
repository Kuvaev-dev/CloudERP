using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerInvoiceDetailRepository
    {
        Task AddSaleDetailsAsync(IEnumerable<SaleCartDetail> saleDetails, int customerInvoiceId);
    }
}
