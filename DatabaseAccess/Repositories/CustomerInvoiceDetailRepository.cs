using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerInvoiceDetailRepository : ICustomerInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddSaleDetailsAsync(IEnumerable<SaleCartDetail> saleDetails, int customerInvoiceId)
        {
            if (saleDetails == null || !saleDetails.Any())
                throw new ArgumentNullException(nameof(saleDetails));

            try
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
            catch (Exception ex)
            {
                LogException(nameof(AddSaleDetailsAsync), ex);
                throw new InvalidOperationException("Error occurred while adding SaleCartDetails.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
