using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierInvoiceDetailRepository
    {
        Task<List<SupplierInvoiceDetail>> GetListByIdAsync(int id);
        Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail);
    }
}
