using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierReturnInvoiceDetailRepository : ISupplierReturnInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierReturnInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(SupplierReturnInvoiceDetail supplierReturnInvoiceDetail)
        {
            try
            {
                if (supplierReturnInvoiceDetail == null) throw new ArgumentNullException(nameof(supplierReturnInvoiceDetail));

                var entity = new tblSupplierReturnInvoiceDetail
                {
                    SupplierReturnInvoiceDetailID = supplierReturnInvoiceDetail.SupplierReturnInvoiceDetailID,
                    SupplierInvoiceID = supplierReturnInvoiceDetail.SupplierInvoiceID,
                    SupplierInvoiceDetailID = supplierReturnInvoiceDetail.SupplierInvoiceDetailID,
                    SupplierReturnInvoiceID = supplierReturnInvoiceDetail.SupplierReturnInvoiceID,
                    ProductID = supplierReturnInvoiceDetail.ProductID,
                    PurchaseReturnQuantity = supplierReturnInvoiceDetail.PurchaseReturnQuantity,
                    PurchaseReturnUnitPrice = supplierReturnInvoiceDetail.PurchaseReturnUnitPrice
                };

                _dbContext.tblSupplierReturnInvoiceDetail.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new supplier return invoice detail.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
