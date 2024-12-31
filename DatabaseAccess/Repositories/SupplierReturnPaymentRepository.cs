using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierReturnPaymentRepository : ISupplierReturnPaymentRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierReturnPaymentRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SupplierReturnPayment>> GetBySupplierReturnInvoiceId(int id)
        {
            var entities = await _dbContext.tblSupplierReturnPayment.Where(r => r.SupplierReturnInvoiceID == id).ToListAsync();

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
                InvoiceDate = (System.DateTime)srp.InvoiceDate
            });
        }
    }
}
