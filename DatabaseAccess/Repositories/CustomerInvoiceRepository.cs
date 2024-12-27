using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerInvoiceRepository : ICustomerInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(CustomerInvoice customerInvoice)
        {
            try
            {
                if (customerInvoice == null) throw new ArgumentNullException(nameof(customerInvoice));

                var entity = new tblCustomerInvoice
                {
                    CustomerInvoiceID = customerInvoice.CustomerInvoiceID,
                    CustomerID = customerInvoice.CustomerID,
                    CompanyID = customerInvoice.CompanyID,
                    BranchID = customerInvoice.BranchID,
                    InvoiceNo = customerInvoice.InvoiceNo,
                    Title = customerInvoice.Title,
                    TotalAmount = customerInvoice.TotalAmount,
                    InvoiceDate = customerInvoice.InvoiceDate,
                    Description = customerInvoice.Description,
                    UserID = customerInvoice.UserID
                };

                _dbContext.tblCustomerInvoice.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account head.", ex);
            }
        }

        public async Task<CustomerInvoice> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblCustomerInvoice.FindAsync(id);
                return entity == null ? null : new CustomerInvoice
                {
                    CustomerInvoiceID = entity.CustomerInvoiceID,
                    CustomerID = entity.CustomerID,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    InvoiceNo = entity.InvoiceNo,
                    Title = entity.Title,
                    TotalAmount = entity.TotalAmount,
                    InvoiceDate = entity.InvoiceDate,
                    Description = entity.Description,
                    UserID = entity.UserID
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        public async Task<CustomerInvoice> GetByInvoiceNoAsync(string invoiceNo)
        {
            try
            {
                var entity = await _dbContext.tblCustomerInvoice
                    .Where(p => p.InvoiceNo == invoiceNo.Trim())
                    .FirstOrDefaultAsync();
                return entity == null ? null : new CustomerInvoice
                {
                    CustomerInvoiceID = entity.CustomerInvoiceID,
                    CustomerID = entity.CustomerID,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    InvoiceNo = entity.InvoiceNo,
                    Title = entity.Title,
                    TotalAmount = entity.TotalAmount,
                    InvoiceDate = entity.InvoiceDate,
                    Description = entity.Description,
                    UserID = entity.UserID
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head.", ex);
            }
        }

        public async Task<double> GetTotalAmountByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblCustomerInvoice.FindAsync(id);
                return entity.TotalAmount;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetTotalAmountByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
