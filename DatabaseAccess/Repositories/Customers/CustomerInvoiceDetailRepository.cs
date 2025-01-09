using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerInvoiceDetailRepository : ICustomerInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddSaleDetailsAsync(IEnumerable<SaleCartDetail> saleDetails, int customerInvoiceId)
        {
            var newSaleDetails = saleDetails.Select(item => new tblCustomerInvoiceDetail
            {
                ProductID = item.ProductID,
                SaleQuantity = item.SaleQuantity,
                SaleUnitPrice = item.SaleUnitPrice,
                CustomerInvoiceID = customerInvoiceId
            }).ToList();

            _dbContext.tblCustomerInvoiceDetail.AddRange(newSaleDetails);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerInvoiceDetail>> GetListByIdAsync(int id)
        {
            var entities = await _dbContext.tblCustomerInvoiceDetail.Where(i => i.CustomerInvoiceID == id).ToListAsync();
            return entities.Select(ci => new CustomerInvoiceDetail
            {
                CustomerInvoiceDetailID = ci.CustomerInvoiceDetailID,
                CustomerInvoiceID = ci.CustomerInvoiceID,
                ProductID = ci.ProductID,
                SaleQuantity = ci.SaleQuantity,
                SaleUnitPrice = ci.SaleUnitPrice
            });
        }
    }
}
