using Domain.Models;
using Domain.RepositoryAccess;
using System;
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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task AddAsync(AccountActivity accountActivity)
        {
            var entity = new tblAccountActivity
            {
                AccountActivityID = accountActivity.AccountActivityID,
                Name = accountActivity.Name
            };

            _dbContext.tblAccountActivity.Add(entity);
            await _dbContext.SaveChangesAsync();
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

        public async Task UpdateAsync(AccountActivity accountActivity)
        {
            var entity = await _dbContext.tblAccountActivity.FindAsync(accountActivity.AccountActivityID);

            entity.AccountActivityID = accountActivity.AccountActivityID;
            entity.Name = accountActivity.Name;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
