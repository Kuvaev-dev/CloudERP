using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
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
                SupplierInvoiceID = supplierReturnInvoiceDetail.SupplierInvoiceID,
                SupplierInvoiceDetailID = supplierReturnInvoiceDetail.SupplierInvoiceDetailID,
                SupplierReturnInvoiceID = supplierReturnInvoiceDetail.SupplierReturnInvoiceID,
                ProductID = supplierReturnInvoiceDetail.ProductID,
                PurchaseReturnQuantity = supplierReturnInvoiceDetail.PurchaseReturnQuantity,
                PurchaseReturnUnitPrice = supplierReturnInvoiceDetail.PurchaseReturnUnitPrice,
                SupplierReturnInvoiceDetailID = supplierReturnInvoiceDetail.SupplierReturnInvoiceDetailID
            };

            _dbContext.tblSupplierReturnInvoiceDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public List<SupplierInvoiceDetail> GetInvoiceDetails(string invoiceId)
        {
            var invoiceDetails = _dbContext.tblSupplierInvoiceDetail
                .Where(d => d.tblSupplierInvoice.InvoiceNo == invoiceId)
                .Select(d => new SupplierInvoiceDetail
                {
                    ProductID = d.ProductID,
                    ProductName = d.tblStock != null ? d.tblStock.ProductName : "Unknown Product",
                    PurchaseQuantity = d.PurchaseQuantity,
                    PurchaseUnitPrice = d.PurchaseUnitPrice,
                    ReturnedQuantity = d.tblSupplierReturnInvoiceDetail
                        .Where(r => r.ProductID == d.ProductID)
                        .Sum(r => (int?)r.PurchaseReturnQuantity) ?? 0
                })
                .ToList();

            foreach (var item in invoiceDetails)
            {
                item.Qty = item.PurchaseQuantity - item.ReturnedQuantity;
                item.ItemCost = item.Qty * item.PurchaseUnitPrice;
            }

            return invoiceDetails;
        }
    }
}
