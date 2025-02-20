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

        public async Task AddPurchaseDetailsAsync(IEnumerable<PurchaseCartDetail> purchaseDetails, int supplierInvoiceID)
        {
            var newSaleDetails = purchaseDetails.Select(item => new tblSupplierInvoiceDetail
            {
                ProductID = item.ProductID,
                PurchaseQuantity = item.PurchaseQuantity,
                PurchaseUnitPrice = item.PurchaseUnitPrice,
                SupplierInvoiceID = supplierInvoiceID
            }).ToList();

            _dbContext.tblSupplierInvoiceDetail.AddRange(newSaleDetails);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SupplierInvoiceDetail>> GetListByIdAsync(int id)
        {
            return await _dbContext.tblSupplierInvoiceDetail
                .Where(i => i.SupplierInvoiceID == id)
                .Select(sid => new SupplierInvoiceDetail
                {
                    SupplierInvoiceDetailID = sid.SupplierInvoiceDetailID,
                    SupplierInvoiceID = sid.SupplierInvoiceID,
                    ProductID = sid.ProductID,
                    PurchaseQuantity = sid.PurchaseQuantity,
                    PurchaseUnitPrice = sid.PurchaseUnitPrice,
                    ProductName = sid.tblStock.ProductName,
                    CompanyName = sid.tblSupplierInvoice.tblCompany.Name,
                    CompanyLogo = sid.tblSupplierInvoice.tblCompany.Logo,
                    Branch = new Branch
                    {
                        BranchName = sid.tblSupplierInvoice.tblBranch.BranchName,
                        BranchContact = sid.tblSupplierInvoice.tblBranch.BranchContact,
                        BranchAddress = sid.tblSupplierInvoice.tblBranch.BranchAddress
                    },
                    Supplier = new Supplier
                    {
                        SupplierName = sid.tblSupplierInvoice.tblSupplier.SupplierName,
                        SupplierConatctNo = sid.tblSupplierInvoice.tblSupplier.SupplierConatctNo,
                        SupplierAddress = sid.tblSupplierInvoice.tblSupplier.SupplierAddress
                    },
                    SupplierInvoice = new SupplierInvoice
                    {
                        InvoiceNo = sid.tblSupplierInvoice.InvoiceNo,
                        InvoiceDate = sid.tblSupplierInvoice.InvoiceDate
                    },
                    ReturnedQuantity = sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0,
                    Qty = sid.PurchaseQuantity - (sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0),
                    ItemCost = (sid.PurchaseQuantity - (sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0)) * sid.PurchaseUnitPrice,
                    SupplierReturnInvoiceDetail = sid.tblSupplierReturnInvoiceDetail.Select(srid => new SupplierReturnInvoiceDetail
                    {
                        SupplierReturnInvoiceID = srid.SupplierReturnInvoiceID,
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
