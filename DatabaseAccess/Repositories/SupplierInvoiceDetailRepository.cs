using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    internal class SupplierInvoiceDetailRepository : ISupplierInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail)
        {
            try
            {
                if (supplierInvoiceDetail == null) throw new ArgumentNullException(nameof(supplierInvoiceDetail));

                var entity = new tblSupplierInvoiceDetail
                {
                    SupplierInvoiceDetailID = supplierInvoiceDetail.SupplierInvoiceDetailID,
                    SupplierInvoiceID = supplierInvoiceDetail.SupplierInvoiceDetailID,
                    ProductID = supplierInvoiceDetail.ProductID,
                    PurchaseQuantity = supplierInvoiceDetail.PurchaseQuantity,
                    PurchaseUnitPrice = supplierInvoiceDetail.PurchaseUnitPrice
                };

                _dbContext.tblSupplierInvoiceDetail.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new supplier invoice detail.", ex);
            }
        }

        public async Task<List<SupplierInvoiceDetail>> GetListByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetListByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
