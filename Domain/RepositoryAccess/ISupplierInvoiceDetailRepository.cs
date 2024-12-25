using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierInvoiceDetailRepository
    {
        Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail);
    }
}
