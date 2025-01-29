using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace DatabaseAccess.Repositories
{
    public class PurchaseCartDetailRepository : IPurchaseCartDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public PurchaseCartDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(PurchaseCartDetail purchaseCartDetail)
        {
            var entity = new tblPurchaseCartDetail
            {
                PurchaseCartDetailID = purchaseCartDetail.PurchaseCartDetailID,
                ProductID = purchaseCartDetail.ProductID,
                PurchaseQuantity = purchaseCartDetail.PurchaseQuantity,
                PurchaseUnitPrice = purchaseCartDetail.PurchaseUnitPrice,
                CompanyID = purchaseCartDetail.CompanyID,
                BranchID = purchaseCartDetail.BranchID,
                UserID = purchaseCartDetail.UserID
            };

            _dbContext.tblPurchaseCartDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseCartDetail purchaseCartDetail)
        {
            var entity = await _dbContext.tblPurchaseCartDetail.FindAsync(purchaseCartDetail.PurchaseCartDetailID);

            entity.PurchaseCartDetailID = purchaseCartDetail.PurchaseCartDetailID;
            entity.ProductID = purchaseCartDetail.ProductID;
            entity.PurchaseQuantity = purchaseCartDetail.PurchaseQuantity;
            entity.PurchaseUnitPrice = purchaseCartDetail.PurchaseUnitPrice;
            entity.CompanyID = purchaseCartDetail.CompanyID;
            entity.BranchID = purchaseCartDetail.BranchID;
            entity.UserID = purchaseCartDetail.UserID;

            _dbContext.Entry(entity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseCartDetail>> GetByBranchAndCompanyAsync(int branchId, int companyId)
        {
            var entities = await _dbContext.tblPurchaseCartDetail
                .Where(pd => pd.BranchID == branchId && pd.CompanyID == companyId)
                .ToListAsync();

            return entities.Select(pcd => new PurchaseCartDetail
            {
                PurchaseCartDetailID = pcd.PurchaseCartDetailID,
                ProductID = pcd.ProductID,
                PurchaseQuantity = pcd.PurchaseQuantity,
                PurchaseUnitPrice = pcd.PurchaseUnitPrice,
                CompanyID = pcd.CompanyID,
                BranchID = pcd.BranchID,
                UserID = pcd.UserID
            });
        }

        public async Task<IEnumerable<PurchaseCartDetail>> GetByDefaultSettingsAsync(int branchId, int companyId, int userId)
        {
            var entities = await _dbContext.tblPurchaseCartDetail
                .Where(i => i.BranchID == branchId && i.CompanyID == companyId && i.UserID == userId)
                .ToListAsync();

            return entities.Select(pcd => new PurchaseCartDetail
            {
                PurchaseCartDetailID = pcd.PurchaseCartDetailID,
                ProductID = pcd.ProductID,
                ProductName = pcd.tblStock.ProductName,
                PurchaseQuantity = pcd.PurchaseQuantity,
                PurchaseUnitPrice = pcd.PurchaseUnitPrice,
                CompanyID = pcd.CompanyID,
                BranchID = pcd.BranchID,
                UserID = pcd.UserID,
                UserName = pcd.tblUser.FullName,
            });
        }

        public async Task<PurchaseCartDetail> GetByIdAsync(int PCID)
        {
            var entity = await _dbContext.tblPurchaseCartDetail.FindAsync(PCID);

            return entity == null ? null : new PurchaseCartDetail
            {
                PurchaseCartDetailID = entity.PurchaseCartDetailID,
                ProductID = entity.ProductID,
                PurchaseQuantity = entity.PurchaseQuantity,
                PurchaseUnitPrice = entity.PurchaseUnitPrice,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<PurchaseCartDetail> GetByProductIdAsync(int branchId, int companyId, int productId)
        {
            var entity = await _dbContext.tblPurchaseCartDetail
                .FirstOrDefaultAsync(i => i.ProductID == productId && i.BranchID == branchId && i.CompanyID == companyId);

            return entity == null ? null : new PurchaseCartDetail
            {
                PurchaseCartDetailID = entity.PurchaseCartDetailID,
                ProductID = entity.ProductID,
                PurchaseQuantity = entity.PurchaseQuantity,
                PurchaseUnitPrice = entity.PurchaseUnitPrice,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<bool> IsCanceled(int branchId, int companyId, int userId)
        {
            bool cancelstatus = false;

            foreach (var item in await GetByDefaultSettingsAsync(branchId, companyId, userId))
            {
                _dbContext.Entry(item).State = EntityState.Deleted;
                int noofrecords = await _dbContext.SaveChangesAsync();
                if (cancelstatus == false && noofrecords > 0)
                {
                    cancelstatus = true;
                }
            }

            return cancelstatus;
        }

        public async Task UpdateAsync(PurchaseCartDetail purchaseCartDetail)
        {
            var entity = await _dbContext.tblPurchaseCartDetail.FindAsync(purchaseCartDetail.PurchaseCartDetailID);

            entity.PurchaseCartDetailID = purchaseCartDetail.PurchaseCartDetailID;
            entity.ProductID = purchaseCartDetail.ProductID;
            entity.PurchaseQuantity = purchaseCartDetail.PurchaseQuantity;
            entity.PurchaseUnitPrice = purchaseCartDetail.PurchaseUnitPrice;
            entity.CompanyID = purchaseCartDetail.CompanyID;
            entity.BranchID = purchaseCartDetail.BranchID;
            entity.UserID = purchaseCartDetail.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
