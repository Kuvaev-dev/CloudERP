using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
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
                    ProductName = ci.tblStock.ProductName,
                    CompanyName = ci.tblCustomerInvoice.tblCompany.Name,
                    CompanyLogo = ci.tblCustomerInvoice.tblCompany.Logo,
                    Branch = new Branch()
                    {
                        BranchName = ci.tblCustomerInvoice.tblBranch.BranchName,
                        BranchContact = ci.tblCustomerInvoice.tblBranch.BranchContact,
                        BranchAddress = ci.tblCustomerInvoice.tblBranch.BranchAddress,
                    },
                    Customer = new Customer()
                    {
                        Customername = ci.tblCustomerInvoice.tblCustomer.Customername,
                        CustomerContact = ci.tblCustomerInvoice.tblCustomer.CustomerContact,
                        CustomerAddress = ci.tblCustomerInvoice.tblCustomer.CustomerAddress,
                    },
                    CustomerInvoiceNo = ci.tblCustomerInvoice.InvoiceNo,
                    CustomerInvoiceDate = ci.tblCustomerInvoice.InvoiceDate,
                    ReturnedQuantity = ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0,
                    Qty = ci.SaleQuantity - (ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0),
                    ItemCost = (ci.SaleQuantity - (ci.tblCustomerReturnInvoiceDetail.Sum(q => (int?)q.SaleReturnQuantity) ?? 0)) * ci.SaleUnitPrice,
                    CustomerReturnInvoiceDetail = ci.tblCustomerReturnInvoiceDetail.Select(cr => new CustomerReturnInvoiceDetail
                    {
                        CustomerReturnInvoiceID = cr.CustomerReturnInvoiceID,
                        SaleReturnQuantity = cr.SaleReturnQuantity,
                        SaleReturnUnitPrice = cr.SaleReturnUnitPrice,
                        InvoiceNo = cr.tblCustomerReturnInvoice.InvoiceNo,
                        InvoiceDate = cr.tblCustomerReturnInvoice.InvoiceDate,
                        ProductName = cr.tblStock.ProductName
                    }).ToList()
                }).ToListAsync();
        }
    }
}
