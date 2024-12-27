using Domain.Models;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ICustomerInvoiceRepository
    {
        Task AddAsync(CustomerInvoice customerInvoice);
    }
}
