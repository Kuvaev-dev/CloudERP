using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SaleCartDetailRepository : ISaleCartDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SaleCartDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(SaleCartDetail saleCartDetail)
        {
            if (saleCartDetail == null) throw new ArgumentNullException(nameof(saleCartDetail));

            var entity = new tblSaleCartDetail
            {
                SaleCartDetailID = saleCartDetail.SaleCartDetailID,
                ProductID = saleCartDetail.ProductID,
                SaleQuantity = saleCartDetail.SaleQuantity,
                SaleUnitPrice = saleCartDetail.SaleUnitPrice,
                CompanyID = saleCartDetail.CompanyID,
                BranchID = saleCartDetail.BranchID,
                UserID = saleCartDetail.UserID
            };

            _dbContext.tblSaleCartDetail.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<SaleCartDetail> saleCartDetails)
        {
            foreach (var item in saleCartDetails)
            {
                _dbContext.Entry(item).State = EntityState.Deleted;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<SaleCartDetail> GetByCompanyAndBranchAsync(int branchId, int companyId)
        {
            var entity = await _dbContext.tblSaleCartDetail.FirstOrDefaultAsync(pd => pd.CompanyID == companyId && pd.BranchID == branchId);

            return entity == null ? null : new SaleCartDetail
            {
                SaleCartDetailID = entity.SaleCartDetailID,
                ProductID = entity.ProductID,
                SaleQuantity = entity.SaleQuantity,
                SaleUnitPrice = entity.SaleUnitPrice,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<IEnumerable<SaleCartDetail>> GetByDefaultSettingAsync(int branchId, int companyId, int userId)
        {
            var entities = await _dbContext.tblSaleCartDetail
                .Where(i => i.BranchID == branchId && i.CompanyID == companyId && i.UserID == userId)
                .ToListAsync();

            return entities.Select(scd => new SaleCartDetail
            {
                SaleCartDetailID = scd.SaleCartDetailID,
                ProductID = scd.ProductID,
                SaleQuantity = scd.SaleQuantity,
                SaleUnitPrice = scd.SaleUnitPrice,
                CompanyID = scd.CompanyID,
                BranchID = scd.BranchID,
                UserID = scd.UserID
            });
        }

        public async Task<SaleCartDetail> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblSaleCartDetail.FindAsync(id);

            return entity == null ? null : new SaleCartDetail
            {
                SaleCartDetailID = entity.SaleCartDetailID,
                ProductID = entity.ProductID,
                SaleQuantity = entity.SaleQuantity,
                SaleUnitPrice = entity.SaleUnitPrice,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<SaleCartDetail> GetByProductIdAsync(int productId, int branchId, int companyId)
        {
            var entity = await _dbContext.tblSaleCartDetail
            .FirstOrDefaultAsync(i => i.ProductID == productId && i.BranchID == branchId && i.CompanyID == companyId);

            return entity == null ? null : new SaleCartDetail
            {
                SaleCartDetailID = entity.SaleCartDetailID,
                ProductID = entity.ProductID,
                SaleQuantity = entity.SaleQuantity,
                SaleUnitPrice = entity.SaleUnitPrice,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<IEnumerable<SaleCartDetail>> GetAllAsync(int branchId, int companyId)
        {
            var entities = await _dbContext.tblSaleCartDetail.
                Where(pd => pd.CompanyID == companyId && pd.BranchID == branchId)
                .ToListAsync();

            return entities.Select(scd => new SaleCartDetail
            {
                SaleCartDetailID = scd.SaleCartDetailID,
                ProductID = scd.ProductID,
                SaleQuantity = scd.SaleQuantity,
                SaleUnitPrice = scd.SaleUnitPrice,
                CompanyID = scd.CompanyID,
                BranchID = scd.BranchID,
                UserID = scd.UserID
            });
        }

        public async Task UpdateAsync(SaleCartDetail saleCartDetail)
        {
            if (saleCartDetail == null) throw new ArgumentNullException(nameof(saleCartDetail));

            var entity = await _dbContext.tblSaleCartDetail.FindAsync(saleCartDetail.SaleCartDetailID);
            if (entity == null) throw new KeyNotFoundException("Sale cart detail not found.");

            entity.SaleCartDetailID = saleCartDetail.SaleCartDetailID;
            entity.ProductID = saleCartDetail.ProductID;
            entity.SaleQuantity = saleCartDetail.SaleQuantity;
            entity.SaleUnitPrice = saleCartDetail.SaleUnitPrice;
            entity.CompanyID = saleCartDetail.CompanyID;
            entity.BranchID = saleCartDetail.BranchID;
            entity.UserID = saleCartDetail.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
