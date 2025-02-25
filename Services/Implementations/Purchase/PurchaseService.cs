using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;

        public PurchaseService(ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository)
        {
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
        }

        public async Task<PurchaseItemDetailDto> GetPurchaseItemDetailAsync(int id)
        {
            var list = await _supplierInvoiceDetailRepository.GetListByIdAsync(id);

            if (list == null || !list.Any())
                return null;

            var invoiceNo = list.First().SupplierInvoice.InvoiceNo;
            var total = 0.0;

            var products = list.Select(item =>
            {
                var qty = item.PurchaseQuantity - item.SupplierReturnInvoiceDetail
                    .Where(q => q.ProductID == item.ProductID)
                    .Sum(q => q.PurchaseReturnQuantity);

                var itemCost = qty * item.PurchaseUnitPrice;
                total += itemCost;

                return new PurchaseProductDetail
                {
                    ProductName = item.ProductName,
                    Quantity = qty,
                    UnitPrice = item.PurchaseUnitPrice,
                    ItemCost = itemCost
                };
            }).ToList();

            var returns = list
                .SelectMany(item => item.SupplierReturnInvoiceDetail)
                .Where(r => r.PurchaseReturnQuantity > 0)
                .Select(r => new PurchaseProductDetail
                {
                    ProductName = r.ProductName,
                    Quantity = r.PurchaseReturnQuantity,
                    UnitPrice = r.PurchaseReturnUnitPrice,
                    ItemCost = r.PurchaseReturnQuantity * r.PurchaseReturnUnitPrice
                }).ToList();

            return new PurchaseItemDetailDto
            {
                InvoiceNo = invoiceNo,
                Products = products,
                Total = total,
                Returns = returns
            };
        }
    }
}
