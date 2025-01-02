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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(SupplierReturnInvoiceDetail supplierReturnInvoiceDetail)
        {
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
    }
}
