using Domain.Models;
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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<double> GetTotalPaidAmount(int id)
        {
            try
            {
                return await _dbContext.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).SumAsync(p => p.PaymentAmount);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalPaidAmount), ex);
                throw new InvalidOperationException($"Error retrieving total paid amount.", ex);
            }
        }

        public async Task<bool> GetByInvoiceIdAsync(int id)
        {
            try
            {
                return await _dbContext.tblSupplierPayment.AnyAsync(p => p.SupplierInvoiceID == id);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByInvoiceIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account control with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
