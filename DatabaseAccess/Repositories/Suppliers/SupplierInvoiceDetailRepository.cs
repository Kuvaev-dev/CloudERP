using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;

namespace DatabaseAccess.Repositories.Suppliers
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
                    ProductName = sid.Stock.ProductName,
                    CompanyName = sid.SupplierInvoice.Company.Name,
                    CompanyLogo = sid.SupplierInvoice.Company.Logo,
                    Branch = new Domain.Models.Branch
                    {
                        BranchName = sid.SupplierInvoice.Branch.BranchName,
                        BranchContact = sid.SupplierInvoice.Branch.BranchContact,
                        BranchAddress = sid.SupplierInvoice.Branch.BranchAddress
                    },
                    Supplier = new Supplier
                    {
                        SupplierName = sid.SupplierInvoice.Supplier.SupplierName,
                        SupplierConatctNo = sid.SupplierInvoice.Supplier.SupplierConatctNo,
                        SupplierAddress = sid.SupplierInvoice.Supplier.SupplierAddress
                    },
                    SupplierInvoice = new SupplierInvoice
                    {
                        InvoiceNo = sid.SupplierInvoice.InvoiceNo,
                        InvoiceDate = sid.SupplierInvoice.InvoiceDate
                    },
                    ReturnedQuantity = sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0,
                    Qty = sid.PurchaseQuantity - (sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0),
                    ItemCost = (sid.PurchaseQuantity - (sid.tblSupplierReturnInvoiceDetail.Sum(q => (int?)q.PurchaseReturnQuantity) ?? 0)) * sid.PurchaseUnitPrice,
                    SupplierReturnInvoiceDetail = sid.tblSupplierReturnInvoiceDetail.Select(srid => new SupplierReturnInvoiceDetail
                    {
                        SupplierReturnInvoiceID = srid.SupplierReturnInvoiceID,
                        PurchaseReturnQuantity = srid.PurchaseReturnQuantity,
                        PurchaseReturnUnitPrice = srid.PurchaseReturnUnitPrice,
                        InvoiceNo = srid.SupplierReturnInvoice.InvoiceNo,
                        InvoiceDate = srid.SupplierReturnInvoice.InvoiceDate,
                        ProductName = srid.Stock.ProductName
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
