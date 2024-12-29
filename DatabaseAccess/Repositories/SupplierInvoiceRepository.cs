using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    internal class SupplierInvoiceRepository : ISupplierInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SupplierInvoice> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblSupplierInvoice.FindAsync(id);

            return entity == null ? null : new SupplierInvoice
            {
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

        public async Task<SupplierInvoice> GetByInvoiceNoAsync(string invoiceNo)
        {
            var entity = await _dbContext.tblSupplierInvoice
                .Where(p => p.InvoiceNo == invoiceNo.Trim())
                .FirstOrDefaultAsync();

            return entity == null ? null : new SupplierInvoice
            {
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

        public async Task AddAsync(SupplierInvoice supplierInvoice)
        {
            if (supplierInvoice == null) throw new ArgumentNullException(nameof(supplierInvoice));

            var entity = new tblSupplierInvoice
            {
                SupplierInvoiceID = supplierInvoice.SupplierInvoiceID,
                SupplierID = supplierInvoice.SupplierID,
                CompanyID = supplierInvoice.CompanyID,
                BranchID = supplierInvoice.BranchID,
                InvoiceNo = supplierInvoice.InvoiceNo,
                TotalAmount = supplierInvoice.TotalAmount,
                InvoiceDate = supplierInvoice.InvoiceDate,
                Description = supplierInvoice.Description,
                UserID = supplierInvoice.UserID
            };

            _dbContext.tblSupplierInvoice.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetSupplierIdFromInvoice(int id)
        {
            var entity = await _dbContext.tblSupplierInvoice.FindAsync(id);
            return entity.SupplierID;
        }

        public async Task<double> GetTotalAmountAsync(int id)
        {
            var entity = await _dbContext.tblSupplierInvoice.FindAsync(id);
            return entity.TotalAmount;
        }
    }
}
