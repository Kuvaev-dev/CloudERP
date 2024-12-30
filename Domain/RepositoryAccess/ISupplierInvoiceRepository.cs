using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierInvoiceRepository
    {
        Task AddAsync(SupplierInvoice supplierInvoice);
        Task<SupplierInvoice> GetByIdAsync(int id);
        Task<SupplierInvoice> GetByInvoiceNoAsync(string invoiceNo);
        Task<double> GetTotalAmountAsync(int id);
        Task<int> GetSupplierIdFromInvoice(int id);
        Task<int> GetLatestIdAsync(int supplierId);
    }
}
