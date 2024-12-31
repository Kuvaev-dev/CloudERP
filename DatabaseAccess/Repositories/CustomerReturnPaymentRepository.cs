using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerReturnPaymentRepository : ICustomerReturnPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CustomerReturnPayment>> GetListByReturnInvoiceIdAsync(int id)
        {
            var entities = await _dbContext.tblCustomerReturnPayment
                .Where(r => r.CustomerReturnInvoiceID == id)
                .ToListAsync();

            return entities.Select(crp => new CustomerReturnPayment
            {
                CustomerReturnPaymentID = crp.CustomerReturnPaymentID,
                CustomerReturnInvoiceID = crp.CustomerReturnInvoiceID,
                CustomerID = crp.CustomerID,
                CustomerInvoiceID = crp.CustomerInvoiceID,
                CompanyID = crp.CompanyID,
                BranchID = crp.BranchID,
                InvoiceNo = crp.InvoiceNo,
                TotalAmount = crp.TotalAmount,
                PaidAmount = crp.PaidAmount,
                RemainingBalance = crp.RemainingBalance,
                UserID = crp.UserID,
                InvoiceDate = (DateTime)crp.InvoiceDate
            });
        }
    }
}
