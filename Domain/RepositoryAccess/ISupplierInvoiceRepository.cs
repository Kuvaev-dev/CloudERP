using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierInvoiceRepository
    {
        Task AddAsync(SupplierInvoice supplierInvoice);
    }
}
