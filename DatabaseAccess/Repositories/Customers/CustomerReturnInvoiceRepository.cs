using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerReturnInvoiceRepository : ICustomerReturnInvoiceRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnInvoiceRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<CustomerReturnInvoice>> GetListByIdAsync(int id)
        {
            var entities = await _dbContext.tblCustomerReturnInvoice.Where(r => r.CustomerInvoiceID == id).ToListAsync();

            return entities.Select(cri => new CustomerReturnInvoice
            {
                CustomerReturnInvoiceID = cri.CustomerReturnInvoiceID,
                CustomerInvoiceID = cri.CustomerInvoiceID,
                CustomerID = cri.CustomerID,
                CompanyID = cri.CompanyID,
                BranchID = cri.BranchID,
                InvoiceNo = cri.InvoiceNo,
                TotalAmount = cri.TotalAmount,
                InvoiceDate = cri.InvoiceDate,
                Description = cri.Description,
                UserID = cri.UserID
            });
        }

        public async Task<CustomerReturnInvoice> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblCustomerReturnInvoice.FindAsync(id);

            return entity == null ? null : new CustomerReturnInvoice
            {
                CustomerReturnInvoiceID = entity.CustomerReturnInvoiceID,
                CustomerInvoiceID = entity.CustomerInvoiceID,
                CustomerID = entity.CustomerID,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                InvoiceNo = entity.InvoiceNo,
                TotalAmount = entity.TotalAmount,
                InvoiceDate = entity.InvoiceDate,
                Description = entity.Description,
                UserID = entity.UserID
            };
        }

        public async Task<double> GetTotalAmountByIdAsync(int id)
        {
            var entity = await _dbContext.tblCustomerReturnInvoice.FindAsync(id);
            return entity.TotalAmount;
        }

        public async Task AddAsync(CustomerReturnInvoice customerReturnInvoice)
        {
            var entity = new tblCustomerReturnInvoice
            {
                CustomerReturnInvoiceID = customerReturnInvoice.CustomerReturnInvoiceID,
                CustomerInvoiceID = customerReturnInvoice.CustomerInvoiceID,
                CustomerID = customerReturnInvoice.CustomerID,
                CompanyID = customerReturnInvoice.CompanyID,
                BranchID = customerReturnInvoice.BranchID,
                InvoiceNo = customerReturnInvoice.InvoiceNo,
                TotalAmount = customerReturnInvoice.TotalAmount,
                InvoiceDate = customerReturnInvoice.InvoiceDate,
                Description = customerReturnInvoice.Description,
                UserID = customerReturnInvoice.UserID
            };

            _dbContext.tblCustomerReturnInvoice.Add(entity);
            await _dbContext.SaveChangesAsync();
            customerReturnInvoice.CustomerReturnInvoiceID = entity.CustomerReturnInvoiceID;
        }
    }
}
