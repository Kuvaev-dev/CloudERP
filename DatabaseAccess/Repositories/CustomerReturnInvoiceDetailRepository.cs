using Domain.Models;
using Domain.RepositoryAccess;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerReturnInvoiceDetailRepository : ICustomerReturnInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail)
        {
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
    }
}
