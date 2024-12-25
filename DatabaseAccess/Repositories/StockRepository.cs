using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly CloudDBEntities _dbContext;

        public StockRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID)
        {
            try
            {
                var entities = await _dbContext.tblStock
                    .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                    .ToListAsync();

                return entities.Select(s => new Stock
                {
                    ProductID = s.ProductID,
                    CategoryID = s.CategoryID,
                    CompanyID = s.CompanyID,
                    BranchID = s.BranchID,
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    SaleUnitPrice = s.SaleUnitPrice,
                    CurrentPurchaseUnitPrice = s.CurrentPurchaseUnitPrice,
                    ExpiryDate = s.ExpiryDate,
                    Manufacture = s.Manufacture,
                    StockTreshHoldQuantity = s.StockTreshHoldQuantity,
                    Description = s.Description,
                    UserID = s.UserID,
                    IsActive = s.IsActive
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving products.", ex);
            }
        }

        public async Task<Stock> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblStock.FindAsync(id);

                return entity == null ? null : new Stock
                {
                    ProductID = entity.ProductID,
                    CategoryID = entity.CategoryID,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    ProductName = entity.ProductName,
                    Quantity = entity.Quantity,
                    SaleUnitPrice = entity.SaleUnitPrice,
                    CurrentPurchaseUnitPrice = entity.CurrentPurchaseUnitPrice,
                    ExpiryDate = entity.ExpiryDate,
                    Manufacture = entity.Manufacture,
                    StockTreshHoldQuantity = entity.StockTreshHoldQuantity,
                    Description = entity.Description,
                    UserID = entity.UserID,
                    IsActive = entity.IsActive
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving product with ID {id}.", ex);
            }
        }

        public async Task<Stock> GetByProductNameAsync(int companyID, int branchID, string productName)
        {
            try
            {
                var entity = await _dbContext.tblStock
                    .Where(s => s.CompanyID == companyID && s.BranchID == branchID && s.ProductName == productName)
                    .FirstOrDefaultAsync();

                return entity == null ? null : new Stock
                {
                    ProductID = entity.ProductID,
                    CategoryID = entity.CategoryID,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    ProductName = entity.ProductName,
                    Quantity = entity.Quantity,
                    SaleUnitPrice = entity.SaleUnitPrice,
                    CurrentPurchaseUnitPrice = entity.CurrentPurchaseUnitPrice,
                    ExpiryDate = entity.ExpiryDate,
                    Manufacture = entity.Manufacture,
                    StockTreshHoldQuantity = entity.StockTreshHoldQuantity,
                    Description = entity.Description,
                    UserID = entity.UserID,
                    IsActive = entity.IsActive
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving products.", ex);
            }
        }

        public async Task AddAsync(Stock stock)
        {
            try
            {
                if (stock == null) throw new ArgumentNullException(nameof(stock));

                var entity = new tblStock
                {
                    ProductID = stock.ProductID,
                    CategoryID = stock.CategoryID,
                    CompanyID = stock.CompanyID,
                    BranchID = stock.BranchID,
                    ProductName = stock.ProductName,
                    Quantity = stock.Quantity,
                    SaleUnitPrice = stock.SaleUnitPrice,
                    CurrentPurchaseUnitPrice = stock.CurrentPurchaseUnitPrice,
                    ExpiryDate = stock.ExpiryDate,
                    Manufacture = stock.Manufacture,
                    StockTreshHoldQuantity = stock.StockTreshHoldQuantity,
                    Description = stock.Description,
                    UserID = stock.UserID,
                    IsActive = stock.IsActive
                };

                _dbContext.tblStock.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new product.", ex);
            }
        }

        public async Task UpdateAsync(Stock stock)
        {
            try
            {
                if (stock == null) throw new ArgumentNullException(nameof(stock));

                var entity = await _dbContext.tblStock.FindAsync(stock.ProductID);
                if (entity == null) throw new KeyNotFoundException("Product not found.");

                entity.ProductID = stock.ProductID;
                entity.CategoryID = stock.CategoryID;
                entity.CompanyID = stock.CompanyID;
                entity.BranchID = stock.BranchID;
                entity.ProductName = stock.ProductName;
                entity.Quantity = stock.Quantity;
                entity.SaleUnitPrice = stock.SaleUnitPrice;
                entity.CurrentPurchaseUnitPrice = stock.CurrentPurchaseUnitPrice;
                entity.ExpiryDate = stock.ExpiryDate;
                entity.Manufacture = stock.Manufacture;
                entity.StockTreshHoldQuantity = stock.StockTreshHoldQuantity;
                entity.Description = stock.Description;
                entity.UserID = stock.UserID;
                entity.IsActive = stock.IsActive;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating product with ID {stock.ProductID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
