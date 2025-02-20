using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierReturnInvoiceRepository : ISupplierReturnInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierReturnInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<SupplierReturnInvoice>> GetReturnDetails(int id)
        {
            var entities = await _dbContext.tblSupplierReturnInvoice
                .Where(r => r.SupplierInvoiceID == id)
                .ToListAsync();

            return entities.Select(sri => new SupplierReturnInvoice
            {
                SupplierReturnInvoiceID = sri.SupplierReturnInvoiceID,
                SupplierInvoiceID = sri.SupplierInvoiceID,
                SupplierID = sri.SupplierID,
                CompanyID = sri.CompanyID,
                BranchID = sri.BranchID,
                InvoiceNo = sri.InvoiceNo,
                TotalAmount = sri.TotalAmount,
                InvoiceDate = sri.InvoiceDate,
                Description = sri.Description,
                UserID = sri.UserID
            });
        }

        public async Task<double> GetTotalAmount(int id)
        {
            var entity = await _dbContext.tblSupplierReturnInvoice.FindAsync(id);
            return entity.TotalAmount;
        }

        public async Task<int> GetSupplierIdByInvoice(int id)
        {
            var entity = await _dbContext.tblSupplierReturnInvoice.FindAsync(id);
            return entity.SupplierID;
        }

        public async Task<SupplierReturnInvoice> GetById(int id)
        {
            var entity = await _dbContext.tblSupplierReturnInvoice.FindAsync(id);

            return entity == null ? null : new SupplierReturnInvoice
            {
                SupplierReturnInvoiceID = entity.SupplierReturnInvoiceID,
                SupplierInvoiceID = entity.SupplierInvoiceID,
                SupplierID = entity.SupplierID,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                InvoiceNo = entity.InvoiceNo,
                TotalAmount = entity.TotalAmount,
                InvoiceDate = entity.InvoiceDate,
                Description = entity.Description,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(SupplierReturnInvoice supplierReturnInvoice)
        {
            var entity = new tblSupplierReturnInvoice
            {
                BranchID = supplierReturnInvoice.BranchID,
                CompanyID = supplierReturnInvoice.CompanyID,
                Description = supplierReturnInvoice.Description,
                InvoiceDate = supplierReturnInvoice.InvoiceDate,
                InvoiceNo = supplierReturnInvoice.InvoiceNo,
                SupplierID = supplierReturnInvoice.SupplierID,
                UserID = supplierReturnInvoice.UserID,
                TotalAmount = supplierReturnInvoice.TotalAmount,
                SupplierInvoiceID = supplierReturnInvoice.SupplierInvoiceID
            };

            _dbContext.tblSupplierReturnInvoice.Add(entity);
            await _dbContext.SaveChangesAsync();
            supplierReturnInvoice.SupplierReturnInvoiceID = entity.SupplierReturnInvoiceID;
        }
    }
}
