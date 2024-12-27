using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerReturnInvoiceDetailRepository : ICustomerReturnInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail)
        {
            try
            {
                if (customerReturnInvoiceDetail == null) throw new ArgumentNullException(nameof(customerReturnInvoiceDetail));

                var entity = new tblCustomerReturnInvoiceDetail
                {
                    CustomerReturnInvoiceDetailID = customerReturnInvoiceDetail.CustomerReturnInvoiceDetailID,
                    CustomerInvoiceDetailID = customerReturnInvoiceDetail.CustomerInvoiceDetailID,
                    CustomerInvoiceID = customerReturnInvoiceDetail.CustomerInvoiceID,
                    CustomerReturnInvoiceID = customerReturnInvoiceDetail.CustomerReturnInvoiceID,
                    ProductID = customerReturnInvoiceDetail.ProductID,
                    SaleReturnQuantity = customerReturnInvoiceDetail.SaleReturnQuantity,
                    SaleReturnUnitPrice = customerReturnInvoiceDetail.SaleReturnUnitPrice
                };

                _dbContext.tblCustomerReturnInvoiceDetail.Add(entity);
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
