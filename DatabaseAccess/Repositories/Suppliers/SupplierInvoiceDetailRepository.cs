using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierInvoiceDetailRepository : ISupplierInvoiceDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierInvoiceDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(SupplierInvoiceDetail supplierInvoiceDetail)
        {
            var entity = new tblSupplierInvoiceDetail
            {
                SupplierInvoiceID = supplierInvoiceDetail.SupplierInvoiceID,
                ProductID = supplierInvoiceDetail.ProductID,
                PurchaseQuantity = supplierInvoiceDetail.PurchaseQuantity,
                PurchaseUnitPrice = supplierInvoiceDetail.PurchaseUnitPrice
            };

            _dbContext.tblSupplierInvoiceDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SupplierInvoiceDetail>> GetListByIdAsync(int id)
        {
            return await _dbContext.tblSupplierInvoiceDetail
            .Where(i => i.SupplierInvoiceID == id)
            .Select(sid => new SupplierInvoiceDetail
            {
                SupplierInvoiceDetailID = sid.SupplierInvoiceDetailID,
                SupplierInvoiceID = sid.SupplierInvoiceID,
                ProductID = sid.ProductID,
                ProductName = sid.tblStock.ProductName,
                PurchaseQuantity = sid.PurchaseQuantity,
                PurchaseUnitPrice = sid.PurchaseUnitPrice,
                UserName = sid.tblSupplierInvoice.tblUser.FullName,
                ReturnedQuantity = sid.tblSupplierReturnInvoiceDetail.Sum(q => q.PurchaseReturnQuantity),
                Qty = sid.PurchaseQuantity - sid.tblSupplierReturnInvoiceDetail.Sum(q => q.PurchaseReturnQuantity),
                ItemCost = (sid.PurchaseQuantity - sid.tblSupplierReturnInvoiceDetail.Sum(q => q.PurchaseReturnQuantity)) * sid.PurchaseUnitPrice,
                Supplier = new Supplier
                {
                    SupplierName = sid.tblSupplierInvoice.tblSupplier.SupplierName,
                    SupplierConatctNo = sid.tblSupplierInvoice.tblSupplier.SupplierConatctNo,
                    SupplierAddress = sid.tblSupplierInvoice.tblSupplier.SupplierAddress
                },
                Company = new Company
                {
                    Name = sid.tblSupplierInvoice.tblCompany.Name
                },
                Branch = new Branch
                {
                    BranchName = sid.tblSupplierInvoice.tblBranch.BranchName,
                    BranchContact = sid.tblSupplierInvoice.tblBranch.BranchContact,
                    BranchAddress = sid.tblSupplierInvoice.tblBranch.BranchAddress
                },
                SupplierInvoice = new SupplierInvoice
                {
                    InvoiceNo = sid.tblSupplierInvoice.InvoiceNo,
                    InvoiceDate = sid.tblSupplierInvoice.InvoiceDate
                },
                SupplierReturnInvoiceDetail = sid.tblSupplierReturnInvoiceDetail.Select(srid => new SupplierReturnInvoiceDetail
                {
                    PurchaseReturnQuantity = srid.PurchaseReturnQuantity,
                    PurchaseReturnUnitPrice = srid.PurchaseReturnUnitPrice,
                    InvoiceNo = srid.tblSupplierReturnInvoice.InvoiceNo,
                    InvoiceDate = srid.tblSupplierReturnInvoice.InvoiceDate,
                    ProductName = srid.tblStock.ProductName
                }).ToList()
            })
            .ToListAsync();
        }
    }
}
