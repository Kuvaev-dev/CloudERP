using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class StockMapper : BaseMapper<Stock, StockMV>
    {
        public override Stock MapToDomain(StockMV viewModel)
        {
            return new Stock
            {
                ProductID = viewModel.ProductID,
                CategoryID = viewModel.CategoryID,
                ProductName = viewModel.ProductName,
                Quantity = viewModel.Quantity,
                SaleUnitPrice = viewModel.SaleUnitPrice,
                CurrentPurchaseUnitPrice = viewModel.CurrentPurchaseUnitPrice,
                ExpiryDate = viewModel.ExpiryDate,
                Manufacture = viewModel.Manufacture,
                StockTreshHoldQuantity = viewModel.StockTreshHoldQuantity,
                Description = viewModel.Description,
                UserID = viewModel.UserID,
                IsActive = viewModel.IsActive
            };
        }

        public override StockMV MapToViewModel(Stock domainModel)
        {
            return new StockMV
            {
                ProductID = domainModel.ProductID,
                CategoryID = domainModel.CategoryID,
                ProductName = domainModel.ProductName,
                Quantity = domainModel.Quantity,
                SaleUnitPrice = domainModel.SaleUnitPrice,
                CurrentPurchaseUnitPrice = domainModel.CurrentPurchaseUnitPrice,
                ExpiryDate = domainModel.ExpiryDate,
                Manufacture = domainModel.Manufacture,
                StockTreshHoldQuantity = domainModel.StockTreshHoldQuantity,
                Description = domainModel.Description,
                UserID = domainModel.UserID,
                IsActive = domainModel.IsActive
            };
        }
    }
}