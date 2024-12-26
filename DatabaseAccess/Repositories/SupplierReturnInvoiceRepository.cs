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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<SupplierReturnInvoice>> GetReturnDetails(int id)
        {
            try
            {
                var entities = await _dbContext.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToListAsync();

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
            catch (Exception ex)
            {
                LogException(nameof(GetReturnDetails), ex);
                throw new InvalidOperationException("Error retrieving supplier return invoices.", ex);
            }
        }

        public async Task<double> GetTotalAmount(int id)
        {
            try
            {
                var entity = await _dbContext.tblSupplierReturnInvoice.FindAsync(id);
                return entity.TotalAmount;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalAmount), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        public async Task<int> GetSupplierIdByInvoice(int id)
        {
            try
            {
                var entity = await _dbContext.tblSupplierReturnInvoice.FindAsync(id);
                return entity.SupplierID;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalAmount), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }

        public async Task<SupplierReturnInvoice> GetById(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetById), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        public async Task AddAsync(SupplierReturnInvoice supplierReturnInvoice)
        {
            try
            {
                if (supplierReturnInvoice == null) throw new ArgumentNullException(nameof(supplierReturnInvoice));

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
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account head.", ex);
            }
        }
    }
}
