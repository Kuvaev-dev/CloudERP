using Domain.RepositoryAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierPaymentRepository : ISupplierPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<double> GetTotalPaidAmount(int id)
        {
            return await _dbContext.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).SumAsync(p => p.PaymentAmount);
        }

        public async Task<bool> GetByInvoiceIdAsync(int id)
        {
            return await _dbContext.tblSupplierPayment.AnyAsync(p => p.SupplierInvoiceID == id);
        }
    }
}
