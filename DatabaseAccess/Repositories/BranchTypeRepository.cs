using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class BranchTypeRepository : IBranchTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
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
    }
}
