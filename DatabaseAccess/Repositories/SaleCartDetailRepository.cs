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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account head.", ex);
            }
        }

        public async Task DeleteAsync(IEnumerable<SaleCartDetail> saleCartDetails)
        {
            if (saleCartDetails == null || !saleCartDetails.Any())
                throw new ArgumentNullException(nameof(saleCartDetails));

            try
            {
                foreach (var item in saleCartDetails)
                {
                    _dbContext.Entry(item).State = EntityState.Deleted;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteAsync), ex);
                throw new InvalidOperationException("Error occurred while deleting SaleCartDetails.", ex);
            }
        }

        public async Task<SaleCartDetail> GetByCompanyAndBranchAsync(int branchId, int companyId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID.", ex);
            }
        }

        public async Task<IEnumerable<SaleCartDetail>> GetByDefaultSettingAsync(int branchId, int companyId, int userId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByDefaultSettingAsync), ex);
                throw new InvalidOperationException($"Error retrieving sale cart detail.", ex);
            }
        }

        public async Task<SaleCartDetail> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        public async Task<SaleCartDetail> GetByProductIdAsync(int productId, int branchId, int companyId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByDefaultSettingAsync), ex);
                throw new InvalidOperationException($"Error retrieving sale cart detail.", ex);
            }
        }

        public async Task<IEnumerable<SaleCartDetail>> GetAllAsync(int branchId, int companyId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account heads.", ex);
            }
        }

        public async Task UpdateAsync(SaleCartDetail saleCartDetail)
        {
            try
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
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating sale cart detail with ID {saleCartDetail.SaleCartDetailID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
