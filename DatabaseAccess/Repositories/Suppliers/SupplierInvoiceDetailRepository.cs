using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierInvoiceDetailRepository : ISupplierInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail)
        {
            var entity = new tblSupplierInvoiceDetail
            {
                SupplierInvoiceID = supplierInvoiceDetail.SupplierInvoiceID,
                ProductID = supplierInvoiceDetail.ProductID,
                PurchaseQuantity = supplierInvoiceDetail.PurchaseQuantity,
                PurchaseUnitPrice = supplierInvoiceDetail.PurchaseUnitPrice
            };

            _dbContext.tblSupplierInvoiceDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SupplierInvoiceDetail>> GetListByIdAsync(int id)
        {
            var entities = await _dbContext.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id).ToListAsync();

            return entities.Select(sid => new SupplierInvoiceDetail
            {
                SupplierInvoiceDetailID = sid.SupplierInvoiceDetailID,
                SupplierInvoiceID = sid.SupplierInvoiceID,
                ProductID = sid.ProductID,
                PurchaseQuantity = sid.PurchaseQuantity,
                PurchaseUnitPrice = sid.PurchaseUnitPrice
            }).ToList();
        }
    }
}
