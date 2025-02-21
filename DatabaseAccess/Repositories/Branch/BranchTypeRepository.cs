using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories
{
    public class BranchTypeRepository : IBranchTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(BranchType branchType)
        {
            var entity = new tblBranchType
            {
                BranchType = branchType.BranchTypeName
            };

            _dbContext.tblBranchType.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<BranchType>> GetAllAsync()
        {
            var entities = await _dbContext.tblBranchType
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(bt => new BranchType
            {
                BranchTypeID = bt.BranchTypeID,
                BranchTypeName = bt.BranchType
            });
        }

        public async Task<BranchType?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblBranchType
                .FirstOrDefaultAsync(ac => ac.BranchTypeID == id);

            return entity == null ? null : new BranchType
            {
                BranchTypeID = entity.BranchTypeID,
                BranchTypeName = entity.BranchType
            };
        }

        public async Task UpdateAsync(BranchType branchType)
        {
            var entity = await _dbContext.tblBranchType.FindAsync(branchType.BranchTypeID);

            entity.BranchTypeID = branchType.BranchTypeID;
            entity.BranchType = branchType.BranchTypeName;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
