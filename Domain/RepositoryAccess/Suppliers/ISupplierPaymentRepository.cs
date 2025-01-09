using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupplierPaymentRepository
    {
        Task<double> GetTotalPaidAmount(int id);
        Task<bool> GetByInvoiceIdAsync(int id);
    }
}
