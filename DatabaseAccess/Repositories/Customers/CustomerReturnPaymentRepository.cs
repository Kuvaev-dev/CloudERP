using DatabaseAccess.Context;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerReturnPaymentRepository : ICustomerReturnPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<CustomerReturnPayment>> GetByCustomerReturnInvoiceId(int id)
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
