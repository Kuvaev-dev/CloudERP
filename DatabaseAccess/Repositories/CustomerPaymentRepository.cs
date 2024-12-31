using Domain.RepositoryAccess;
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
            _dbContext = dbContext;
        }

        public async Task<double> GetTotalPaidAmountById(int id)
        {
            return await _dbContext.tblCustomerPayment
                .Where(p => p.CustomerInvoiceID == id)
                .SumAsync(p => p.PaidAmount);
        }
    }
}
