using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Account
{
    public class AccountActivityRepository : IAccountActivityRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountActivityRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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

        public async Task<AccountActivity?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountActivity
                .AsNoTracking()
                .FirstOrDefaultAsync(ac => ac.AccountActivityID == id);

            return entity == null ? null : new AccountActivity
            {
                AccountActivityID = entity.AccountActivityID,
                Name = entity.Name
            };
        }

        public async Task<bool> IsExists(AccountActivity accountActivity)
        {
            return await _dbContext.tblAccountActivity
                .AnyAsync(a => a.Name == accountActivity.Name);
        }

        public async Task UpdateAsync(AccountActivity accountActivity)
        {
            var entity = await _dbContext.tblAccountActivity.FindAsync(accountActivity.AccountActivityID);
            if (entity == null) return;

            entity.Name = accountActivity.Name;

            _dbContext.tblAccountActivity.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}