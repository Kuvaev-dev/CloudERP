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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving supplier invoice with ID {id}.", ex);
            }
        }

        public async Task<SupplierInvoice> GetByInvoiceNoAsync(string invoiceNo)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving supplier invoice with NO {invoiceNo}.", ex);
            }
        }

        public async Task AddAsync(SupplierInvoice supplierInvoice)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new supplier invoice.", ex);
            }
        }

        public async Task<int> GetSupplierIdFromInvoice(int id)
        {
            try
            {
                var entity = await _dbContext.tblSupplierInvoice.FindAsync(id);
                return entity.SupplierID;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalAmountAsync), ex);
                throw new InvalidOperationException($"Error retrieving total amount.", ex);
            }
        }

        public async Task<double> GetTotalAmountAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblSupplierInvoice.FindAsync(id);
                return entity.TotalAmount;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalAmountAsync), ex);
                throw new InvalidOperationException($"Error retrieving total amount.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
