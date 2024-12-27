using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerInvoiceRepository : ICustomerInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(CustomerInvoice customerInvoice)
        {
            try
            {
                if (customerInvoice == null) throw new ArgumentNullException(nameof(customerInvoice));

                var entity = new tblCustomerInvoice
                {
                    
                };

                _dbContext.tblCustomerInvoice.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account head.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
