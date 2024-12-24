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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account heads.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
