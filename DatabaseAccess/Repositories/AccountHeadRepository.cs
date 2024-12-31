using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountHeadRepository : IAccountHeadRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountHeadRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountHead>> GetAllAsync()
        {
            var entities = await _dbContext.tblAccountHead
            .AsNoTracking()
            .Include(e => e.tblUser)
            .ToListAsync();

            return entities.Select(ah => new AccountHead
            {
                AccountHeadID = ah.AccountHeadID,
                AccountHeadName = ah.AccountHeadName,
                Code = ah.Code,
                UserID = ah.UserID,
                FullName = ah.tblUser.FullName
            });
        }

        public async Task<AccountHead> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountHead
                .Include(ac => ac.tblUser)
                .FirstOrDefaultAsync(ac => ac.AccountHeadID == id);

            return entity == null ? null : new AccountHead
            {
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHeadName,
                Code = entity.Code,
                UserID = entity.UserID,
                FullName = entity.tblUser.FullName
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

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
