using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Sale
{
    public class SaleCartDetailRepository : ISaleCartDetailRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SaleCartDetailRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(SaleCartDetail saleCartDetail)
        {
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

        public async Task DeleteAsync(int detailID)
        {
            var item = await _dbContext.tblSaleCartDetail.FirstOrDefaultAsync(d => d.SaleCartDetailID == detailID);
            _dbContext.tblSaleCartDetail.Update(item);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteListAsync(IEnumerable<SaleCartDetail> saleCartDetails)
        {
            foreach (var item in saleCartDetails)
            {
                var entity = new tblSaleCartDetail
                {
                    SaleCartDetailID = item.SaleCartDetailID,
                    ProductID = item.ProductID,
                    SaleQuantity = item.SaleQuantity,
                    SaleUnitPrice = item.SaleUnitPrice,
                    CompanyID = item.CompanyID,
                    BranchID = item.BranchID,
                    UserID = item.UserID
                };

                var trackedEntity = _dbContext.ChangeTracker.Entries<tblSaleCartDetail>()
                    .FirstOrDefault(e => e.Entity.SaleCartDetailID == entity.SaleCartDetailID);

                if (trackedEntity != null)
                {
                    trackedEntity.State = EntityState.Deleted;
                }
                else
                {
                    _dbContext.tblSaleCartDetail.Attach(entity);
                    _dbContext.Entry(entity).State = EntityState.Deleted;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<SaleCartDetail?> GetByCompanyAndBranchAsync(int branchId, int companyId)
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
                ProductName = scd.Product.ProductName,
                SaleQuantity = scd.SaleQuantity,
                SaleUnitPrice = scd.SaleUnitPrice,
                CompanyID = scd.CompanyID,
                BranchID = scd.BranchID,
                UserID = scd.UserID,
                UserName = scd.User.FullName
            });
        }

        public async Task<SaleCartDetail?> GetByIdAsync(int id)
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

        public async Task<SaleCartDetail?> GetByProductIdAsync(int productId, int branchId, int companyId)
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
            var entity = await _dbContext.tblSaleCartDetail.FindAsync(saleCartDetail.SaleCartDetailID);

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
