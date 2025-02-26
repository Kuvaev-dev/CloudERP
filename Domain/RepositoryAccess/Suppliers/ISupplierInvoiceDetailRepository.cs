using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ISupplierInvoiceDetailRepository
    {
        Task<IEnumerable<SupplierInvoiceDetail>> GetListByIdAsync(int id);
        Task AddPurchaseDetailsAsync(IEnumerable<PurchaseCartDetail> purchaseDetails, int supplierInvoiceID);
        Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail);
    }
}
