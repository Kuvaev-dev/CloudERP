using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierInvoiceRepository : ISupplierInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
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
                SupplierName = entity.tblSupplier.SupplierName,
                SupplierAddress = entity.tblSupplier.SupplierAddress,
                SupplierConatctNo = entity.tblSupplier.SupplierConatctNo,
                CompanyID = entity.CompanyID,
                CompanyName = entity.tblCompany.Name,
                CompanyLogo = entity.tblCompany.Logo,
                BranchID = entity.BranchID,
                BranchName = entity.tblBranch.BranchName,
                BranchAddress = entity.tblBranch.BranchAddress,
                BranchContact = entity.tblBranch.BranchContact,
                InvoiceNo = entity.InvoiceNo,
                TotalAmount = entity.TotalAmount,
                InvoiceDate = entity.InvoiceDate,
                Description = entity.Description,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(SupplierInvoice supplierInvoice)
        {
            var entity = new tblSupplierInvoice
            {
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
            supplierInvoice.SupplierInvoiceID = entity.SupplierInvoiceID;
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

        public async Task<int> GetLatestIdAsync(int supplierId)
        {
            var latestInvoice = await _dbContext.tblSupplierInvoice
                .Where(invoice => invoice.SupplierID == supplierId)
                .OrderByDescending(invoice => invoice.InvoiceDate)
                .FirstOrDefaultAsync();

            return latestInvoice.SupplierInvoiceID;
        }
    }
}
