using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerInvoiceDetailRepository : ICustomerInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddSaleDetailsAsync(IEnumerable<SaleCartDetail> saleDetails, int customerInvoiceId)
        {
            var newSaleDetails = saleDetails.Select(item => new tblCustomerInvoiceDetail
            {
                ProductID = item.ProductID,
                SaleQuantity = item.SaleQuantity,
                SaleUnitPrice = item.SaleUnitPrice,
                CustomerInvoiceID = customerInvoiceId
            }).ToList();

            _dbContext.tblCustomerInvoiceDetail.AddRange(newSaleDetails);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerInvoiceDetail>> GetListByIdAsync(int id)
        {
            return await _dbContext.tblCustomerInvoiceDetail
                .Where(i => i.CustomerInvoiceID == id)
                .Select(ci => new CustomerInvoiceDetail
                {
                    CustomerInvoiceDetailID = ci.CustomerInvoiceDetailID,
                    CustomerInvoiceID = ci.CustomerInvoiceID,
                    ProductID = ci.ProductID,
                    SaleQuantity = ci.SaleQuantity,
                    SaleUnitPrice = ci.SaleUnitPrice,
                    ProductName = ci.Stock.ProductName,
                    CompanyName = ci.CustomerInvoice.Company.Name,
                    CompanyLogo = ci.CustomerInvoice.Company.Logo,
                    Branch = new Domain.Models.Branch()
                    {
                        BranchName = ci.CustomerInvoice.Branch.BranchName,
                        BranchContact = ci.CustomerInvoice.Branch.BranchContact,
                        BranchAddress = ci.CustomerInvoice.Branch.BranchAddress,
                    },
                    Customer = new Customer()
                    {
                        Customername = ci.CustomerInvoice.Customer.Customername,
                        CustomerContact = ci.CustomerInvoice.Customer.CustomerContact,
                        CustomerAddress = ci.CustomerInvoice.Customer.CustomerAddress,
                    },
                    CustomerInvoice = new CustomerInvoice()
                    {
                        InvoiceNo = ci.CustomerInvoice.InvoiceNo,
                        InvoiceDate = ci.CustomerInvoice.InvoiceDate,
                    },
                    ReturnedQuantity = ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0,
                    Qty = ci.SaleQuantity - (ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0),
                    ItemCost = (ci.SaleQuantity - (ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0)) * ci.SaleUnitPrice,
                    CustomerReturnInvoiceDetail = ci.tblCustomerReturnInvoiceDetail.Select(cr => new CustomerReturnInvoiceDetail
                    {
                        CustomerReturnInvoiceID = cr.CustomerReturnInvoiceID,
                        SaleReturnQuantity = cr.SaleReturnQuantity,
                        SaleReturnUnitPrice = cr.SaleReturnUnitPrice,
                        InvoiceNo = cr.CustomerReturnInvoice.InvoiceNo,
                        InvoiceDate = cr.CustomerReturnInvoice.InvoiceDate,
                        ProductName = cr.Stock.ProductName
                    }).ToList()
                }).ToListAsync();
        }
    }
}
