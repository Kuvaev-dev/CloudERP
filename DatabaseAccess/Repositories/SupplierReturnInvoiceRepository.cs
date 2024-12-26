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

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
