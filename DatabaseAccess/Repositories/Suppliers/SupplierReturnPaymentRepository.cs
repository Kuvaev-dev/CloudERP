using DatabaseAccess.Context;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Suppliers
{
    public class SupplierReturnPaymentRepository : ISupplierReturnPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierReturnPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<SupplierReturnPayment>> GetBySupplierReturnInvoiceId(int id)
        {
            var entities = await _dbContext.tblSupplierReturnPayment
                .Where(r => r.SupplierReturnInvoiceID == id)
                .ToListAsync();

            return entities.Select(srp => new SupplierReturnPayment
            {
                SupplierReturnPaymentID = srp.SupplierReturnPaymentID,
                SupplierReturnInvoiceID = srp.SupplierReturnInvoiceID,
                SupplierInvoiceID = srp.SupplierInvoiceID,
                SupplierID = srp.SupplierID,
                CompanyID = srp.CompanyID,
                BranchID = srp.BranchID,
                InvoiceNo = srp.InvoiceNo,
                TotalAmount = srp.TotalAmount,
                PaymentAmount = srp.PaymentAmount,
                RemainingBalance = srp.RemainingBalance,
                UserID = srp.UserID,
                InvoiceDate = (DateTime)srp.InvoiceDate
            });
        }
    }
}
