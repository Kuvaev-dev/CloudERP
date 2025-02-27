using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class ProductQualityService : IProductQualityService
    {
        private readonly IStockRepository _repository;

        public ProductQualityService(IStockRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductQuality> GetProductQualityAsync(int productId)
        {
            var stock = await _repository.GetByIdAsync(productId);

            return new ProductQuality
            {
                ProductID = stock.ProductID,
                ProductName = stock.ProductName,
                ExpiryDate = stock.ExpiryDate,
                Manufacture = stock.Manufacture,
                StockTreshHoldQuantity = stock.StockTreshHoldQuantity
            };
        }

        public async Task<IEnumerable<ProductQuality>> GetAllProductsQualityAsync(int branchId, int companyId)
        {
            var stocks = await _repository.GetAllAsync(companyId, branchId);
            var productQualities = new List<ProductQuality>();

            foreach (var stock in stocks)
            {
                productQualities.Add(new ProductQuality
                {
                    ProductID = stock.ProductID,
                    ProductName = stock.ProductName,
                    ExpiryDate = stock.ExpiryDate,
                    Manufacture = stock.Manufacture,
                    StockTreshHoldQuantity = stock.StockTreshHoldQuantity
                });
            }

            return productQualities;
        }
    }
}
