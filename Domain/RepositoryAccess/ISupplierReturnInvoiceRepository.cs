using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierReturnInvoiceRepository
    {
        Task<IEnumerable<SupplierReturnInvoice>> GetReturnDetails(int id);
    }
}
