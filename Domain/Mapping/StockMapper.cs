using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class StockMapper : BaseMapper<Stock, tblStock>
    {
        public override Stock MapToDomain(tblStock dbModel)
        {
            return new Stock
            {
                ProductID = dbModel.ProductID,
                CategoryID = dbModel.CategoryID,
                ProductName = dbModel.ProductName,
                Quantity = dbModel.Quantity,
                SaleUnitPrice = dbModel.SaleUnitPrice,
                CurrentPurchaseUnitPrice = dbModel.CurrentPurchaseUnitPrice,
                ExpiryDate = dbModel.ExpiryDate,
                Manufacture = dbModel.Manufacture,
                StockTreshHoldQuantity = dbModel.StockTreshHoldQuantity,
                Description = dbModel.Description,
                UserID = dbModel.UserID,
                IsActive = dbModel.IsActive
            };
        }

        public override tblStock MapToDatabase(Stock domainModel)
        {
            return new tblStock
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
