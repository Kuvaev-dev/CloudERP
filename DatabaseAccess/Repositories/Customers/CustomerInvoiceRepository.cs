using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerInvoiceRepository : ICustomerInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(CustomerInvoice customerInvoice)
        {
            var entity = new tblCustomerInvoice
            {
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
            customerInvoice.CustomerInvoiceID = entity.CustomerInvoiceID;
        }

        public async Task<CustomerInvoice?> GetByIdAsync(int id)
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

        public async Task<CustomerInvoice?> GetByInvoiceNoAsync(string invoiceNo)
        {
            var entity = await _dbContext.tblCustomerInvoice
                .Where(p => p.InvoiceNo == invoiceNo.Trim())
                .FirstOrDefaultAsync();

            return entity == null ? null : new CustomerInvoice
            {
                CustomerInvoiceID = entity.CustomerInvoiceID,
                CustomerID = entity.CustomerID,
                Customername = entity.Customer.Customername,
                CustomerAddress = entity.Customer.CustomerAddress,
                CustomerContact = entity.Customer.CustomerContact,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                CompanyLogo = entity.Company.Logo,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                BranchAddress = entity.Branch.BranchAddress,
                BranchContact = entity.Branch.BranchContact,
                InvoiceNo = entity.InvoiceNo,
                Title = entity.Title,
                TotalAmount = entity.TotalAmount,
                InvoiceDate = entity.InvoiceDate,
                Description = entity.Description,
                UserID = entity.UserID
            };
        }

        public async Task<double?> GetTotalAmountByIdAsync(int id)
        {
            var entity = await _dbContext.tblCustomerInvoice.FindAsync(id);
            return entity?.TotalAmount;
        }
    }
}
