using Domain.Models.Sale;
using Domain.RepositoryAccess;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISaleService
    {
        Task<SaleItemDetailDto> GetSaleItemDetailAsync(int id);
    }

    public class SaleService : ISaleService
    {
        private readonly ICustomerInvoiceDetailRepository _customerInvoiceDetailRepository;

        public SaleService(ICustomerInvoiceDetailRepository customerInvoiceDetailRepository)
        {
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
        }

        public async Task<SaleItemDetailDto> GetSaleItemDetailAsync(int id)
        {
            var list = await _customerInvoiceDetailRepository.GetListByIdAsync(id);

            if (list == null || !list.Any())
                return null;

            var invoiceNo = list.First().CustomerInvoice.InvoiceNo;
            var total = 0.0;

            var products = list.Select(item =>
            {
                var qty = item.SaleQuantity - item.CustomerReturnInvoiceDetail
                    .Where(q => q.ProductID == item.ProductID)
                    .Sum(q => q.SaleReturnQuantity);

                var itemCost = qty * item.SaleUnitPrice;
                total += itemCost;

                return new SaleProductDetail
                {
                    ProductName = item.ProductName,
                    Quantity = qty,
                    UnitPrice = item.SaleUnitPrice,
                    ItemCost = itemCost
                };
            }).ToList();

            var returns = list
                .SelectMany(item => item.CustomerReturnInvoiceDetail)
                .Where(r => r.SaleReturnQuantity > 0)
                .Select(r => new SaleReturnDetail
                {
                    ProductName = r.ProductName,
                    Quantity = r.SaleReturnQuantity,
                    UnitPrice = r.SaleReturnUnitPrice,
                    ItemCost = r.SaleReturnQuantity * r.SaleReturnUnitPrice
                }).ToList();

            return new SaleItemDetailDto
            {
                InvoiceNo = invoiceNo,
                Products = products,
                Total = total,
                Returns = returns
            };
        }
    }
}
