using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Account
{
    public class AccountHeadRepository : IAccountHeadRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountHeadRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<AccountHead>> GetAllAsync()
        {
            var entities = await _dbContext.tblAccountHead
                .AsNoTracking()
                .Include(e => e.User)
                .ToListAsync();

            return entities.Select(ah => new AccountHead
            {
                AccountHeadID = ah.AccountHeadID,
                AccountHeadName = ah.AccountHeadName,
                Code = ah.Code,
                UserID = ah.UserID,
                FullName = ah.User.FullName
            });
        }

        public async Task<IEnumerable<int>> GetAllIdsAsync()
        {
            var ids = await _dbContext.tblAccountHead
                .AsNoTracking()
                .Select(ah => ah.AccountHeadID)
                .ToListAsync();

            return ids;
        }

        public async Task<AccountHead?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountHead
                .Include(ac => ac.User)
                .FirstOrDefaultAsync(ac => ac.AccountHeadID == id);

            return entity == null ? null : new AccountHead
            {
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHeadName,
                Code = entity.Code,
                UserID = entity.UserID,
                FullName = entity.User.FullName
            };
        }

        public async Task AddAsync(AccountHead accountHead)
        {
            var entity = new tblAccountHead
            {
                AccountHeadID = accountHead.AccountHeadID,
                AccountHeadName = accountHead.AccountHeadName,
                Code = accountHead.Code,
                UserID = accountHead.UserID
            };

            _dbContext.tblAccountHead.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountHead accountHead)
        {
            var entity = await _dbContext.tblAccountHead.FindAsync(accountHead.AccountHeadID);

            entity.AccountHeadID = accountHead.AccountHeadID;
            entity.AccountHeadName = accountHead.AccountHeadName;
            entity.Code = accountHead.Code;
            entity.UserID = accountHead.UserID;

            _dbContext.tblAccountHead.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
