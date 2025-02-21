using DatabaseAccess.Context;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerPaymentRepository : ICustomerPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<double> GetTotalPaidAmountById(int id)
        {
            return await _dbContext.tblCustomerPayment
                .Where(p => p.CustomerInvoiceID == id)
                .SumAsync(p => p.PaidAmount);
        }
    }
}
