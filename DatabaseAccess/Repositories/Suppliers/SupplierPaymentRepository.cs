using DatabaseAccess.Context;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Suppliers
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
            return await _dbContext.tblSupplierPayment
                .Where(p => p.SupplierInvoiceID == id)
                .SumAsync(p => (double?)p.PaymentAmount) ?? 0;
        }

        public async Task<bool> GetByInvoiceIdAsync(int id)
        {
            return await _dbContext.tblSupplierPayment.AnyAsync(p => p.SupplierInvoiceID == id);
        }
    }
}
