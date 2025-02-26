using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnInvoiceDetailRepository
    {
        Task AddAsync(SupplierReturnInvoiceDetail supplierReturnInvoiceDetail);
        List<SupplierInvoiceDetail> GetInvoiceDetails(string invoiceId);
    }
}
