using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnInvoiceDetailRepository
    {
        Task AddAsync(SupplierReturnInvoiceDetail supplierReturnInvoiceDetail);
    }
}
