using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnInvoiceRepository
    {
        Task<IEnumerable<SupplierReturnInvoice>> GetReturnDetails(int id);
        Task<SupplierReturnInvoice> GetById(int id);
        Task<double?> GetTotalAmount(int id);
        Task<int?> GetSupplierIdByInvoice(int id);
        Task AddAsync(SupplierReturnInvoice supplierReturnInvoice);
    }
}
