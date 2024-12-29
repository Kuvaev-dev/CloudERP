using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    internal class BranchTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
