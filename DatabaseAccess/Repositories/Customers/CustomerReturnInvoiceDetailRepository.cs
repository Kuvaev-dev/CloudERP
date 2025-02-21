using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerReturnInvoiceDetailRepository : ICustomerReturnInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerReturnInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(CustomerReturnInvoiceDetail customerReturnInvoiceDetail)
        {
            var entity = new tblCustomerReturnInvoiceDetail
            {
                CustomerInvoiceDetailID = customerReturnInvoiceDetail.CustomerInvoiceDetailID,
                CustomerInvoiceID = customerReturnInvoiceDetail.CustomerInvoiceID,
                CustomerReturnInvoiceID = customerReturnInvoiceDetail.CustomerReturnInvoiceID,
                ProductID = customerReturnInvoiceDetail.ProductID,
                SaleReturnQuantity = customerReturnInvoiceDetail.SaleReturnQuantity,
                SaleReturnUnitPrice = customerReturnInvoiceDetail.SaleReturnUnitPrice
            };

            _dbContext.tblCustomerReturnInvoiceDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public List<CustomerInvoiceDetail> GetInvoiceDetails(string invoiceId)
        {
            var invoiceDetails = _dbContext.tblCustomerInvoiceDetail
                .Where(d => d.CustomerInvoice.InvoiceNo == invoiceId)
                .Select(d => new CustomerInvoiceDetail
                {
                    ProductID = d.ProductID,
                    ProductName = d.Stock != null ? d.Stock.ProductName : "Unknown Product",
                    SaleQuantity = d.SaleQuantity,
                    SaleUnitPrice = d.SaleUnitPrice,
                    ReturnedQuantity = d.tblCustomerReturnInvoiceDetail
                        .Where(r => r.ProductID == d.ProductID)
                        .Sum(r => (int?)r.SaleReturnQuantity) ?? 0
                })
                .ToList();

            foreach (var item in invoiceDetails)
            {
                item.Qty = item.SaleQuantity - item.ReturnedQuantity;
                item.ItemCost = item.Qty * item.SaleUnitPrice;
            }

            return invoiceDetails ?? new List<CustomerInvoiceDetail>();
        }
    }
}
