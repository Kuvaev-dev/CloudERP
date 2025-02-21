using DatabaseAccess.Context;
using Domain.RepositoryAccess;
using Utils.Models;

namespace DatabaseAccess.Repositories.Finance
{
    public class ForecastingRepository : IForecastingRepository
    {
        private readonly CloudDBEntities _dbContext;

        public ForecastingRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public IEnumerable<ForecastData> GetForecastData(int companyID, int branchID, DateTime startDate, DateTime endDate)
        {
            var transactions = _dbContext.tblTransaction
                .Where(t => t.CompanyID == companyID &&
                            t.BranchID == branchID &&
                            t.TransectionDate >= startDate &&
                            t.TransectionDate <= endDate)
                .Select(t => new
                {
                    Date = new DateTime(t.TransectionDate.Year, t.TransectionDate.Month, 1),
                    NetAmount = t.Credit - t.Debit
                });

            var customerPayments = _dbContext.tblCustomerPayment
                .Where(p => p.CompanyID == companyID &&
                            p.BranchID == branchID &&
                            p.InvoiceDate >= startDate &&
                            p.InvoiceDate <= endDate)
                .Select(p => new
                {
                    Date = new DateTime(p.InvoiceDate.Value.Year, p.InvoiceDate.Value.Month, 1),
                    NetAmount = p.PaidAmount - p.TotalAmount
                });

            var customerReturns = _dbContext.tblCustomerReturnPayment
                .Where(cr => cr.CompanyID == companyID &&
                             cr.BranchID == branchID &&
                             cr.InvoiceDate >= startDate &&
                             cr.InvoiceDate <= endDate)
                .Select(cr => new
                {
                    Date = new DateTime(cr.InvoiceDate.Value.Year, cr.InvoiceDate.Value.Month, 1),
                    NetAmount = -cr.PaidAmount
                });

            var supplierPayments = _dbContext.tblSupplierPayment
                .Where(sp => sp.CompanyID == companyID &&
                             sp.BranchID == branchID &&
                             sp.InvoiceDate >= startDate &&
                             sp.InvoiceDate <= endDate)
                .Select(sp => new
                {
                    Date = new DateTime(sp.InvoiceDate.Value.Year, sp.InvoiceDate.Value.Month, 1),
                    NetAmount = -sp.PaymentAmount
                });

            var supplierReturns = _dbContext.tblSupplierReturnPayment
                .Where(sr => sr.CompanyID == companyID &&
                             sr.BranchID == branchID &&
                             sr.InvoiceDate >= startDate &&
                             sr.InvoiceDate <= endDate)
                .Select(sr => new
                {
                    Date = new DateTime(sr.InvoiceDate.Value.Year, sr.InvoiceDate.Value.Month, 1),
                    NetAmount = sr.PaymentAmount
                });

            // Объединяем все данные
            var allData = transactions
                .Concat(customerPayments)
                .Concat(customerReturns)
                .Concat(supplierPayments)
                .Concat(supplierReturns)
                .GroupBy(d => d.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    NetAmount = g.Sum(d => d.NetAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            if (!allData.Any())
            {
                return Enumerable.Empty<ForecastData>();
            }

            return allData.Select(d => new ForecastData
            {
                Value = (float)d.NetAmount,
                Date = d.Date,
                DateAsNumber = (float)(d.Date - new DateTime(2000, 1, 1)).TotalDays
            });
        }
    }
}
