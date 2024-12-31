using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountActivityRepository : IAccountActivityRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountActivityRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountActivity>> GetAllAsync()
        {
            var entities = await _dbContext.tblAccountActivity
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(aa => new AccountActivity
            {
                AccountActivityID = aa.AccountActivityID,
                Name = aa.Name
            });
        }

        public async Task<AccountActivity> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountActivity
                .FirstOrDefaultAsync(ac => ac.AccountActivityID == id);

            return entity == null ? null : new AccountActivity
            {
                AccountActivityID = entity.AccountActivityID,
                Name = entity.Name
            };
        }
    }
}
