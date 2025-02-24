using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Inventory
{
    public class StockRepository : IStockRepository
    {
        private readonly CloudDBEntities _dbContext;

        public StockRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID)
        {
            var entities = await _dbContext.tblStock
                .Include(s => s.Category)
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                .ToListAsync();

            return entities.Select(s => new Stock
            {
                ProductID = s.ProductID,
                CategoryID = s.CategoryID,
                CategoryName = s.Category.CategoryName,
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

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblStock
                .Include(p => p.Category)
                .Include(p => p.Company)
                .Include(p => p.Branch)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            return entity == null ? null : new Stock
            {
                ProductID = entity.ProductID,
                CategoryID = entity.CategoryID,
                CategoryName = entity.Category.CategoryName,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                ProductName = entity.ProductName,
                Quantity = entity.Quantity,
                SaleUnitPrice = entity.SaleUnitPrice,
                CurrentPurchaseUnitPrice = entity.CurrentPurchaseUnitPrice,
                ExpiryDate = entity.ExpiryDate,
                Manufacture = entity.Manufacture,
                StockTreshHoldQuantity = entity.StockTreshHoldQuantity,
                Description = entity.Description,
                UserID = entity.UserID,
                UserName = entity.User.FullName,
                IsActive = entity.IsActive
            };
        }

        public async Task<Stock?> GetByProductNameAsync(int companyID, int branchID, string productName)
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

        public async Task AddAsync(Stock stock)
        {
            var entity = new tblStock
            {
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

        public async Task UpdateAsync(Stock stock)
        {
            var entity = await _dbContext.tblStock.FindAsync(stock.ProductID);

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

        public async Task<int> GetTotalStockItemsByCompanyAsync(int companyID)
        {
            return await _dbContext.tblStock
                .Where(s => s.CompanyID == companyID)
                .SumAsync(s => (int?)s.Quantity) ?? 0;
        }

        public async Task<int> GetTotalAvaliableItemsByCompanyAsync(int companyID)
        {
            return await _dbContext.tblStock
                .Where(s => s.CompanyID == companyID && s.IsActive == true)
                .SumAsync(s => (int?)s.Quantity) ?? 0;
        }

        public async Task<int> GetTotalExpiredItemsByCompanyAsync(int companyID)
        {
            return await _dbContext.tblStock
                .Where(s => s.CompanyID == companyID && s.ExpiryDate < DateTime.Now)
                .SumAsync(s => (int?)s.Quantity) ?? 0;
        }
    }
}
