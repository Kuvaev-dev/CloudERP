using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnInvoiceDetailRepository
    {
        Task AddAsync(SupplierReturnInvoiceDetail supplierReturnInvoiceDetail);
        List<SupplierInvoiceDetail> GetInvoiceDetails(string invoiceId);
    }
}
