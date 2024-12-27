using Domain.RepositoryAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerPaymentRepository : ICustomerPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<double> GetTotalPaidAmountById(int id)
        {
            try
            {
                return await _dbContext.tblCustomerPayment
                    .Where(p => p.CustomerInvoiceID == id)
                    .SumAsync(p => p.PaidAmount);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalPaidAmountById), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
