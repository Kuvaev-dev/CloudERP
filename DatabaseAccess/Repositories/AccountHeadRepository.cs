using Domain.Models;
using Domain.RepositoryAccess;
using System;
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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<AccountHead>> GetAllAsync()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account heads.", ex);
            }
        }

        public async Task<AccountHead> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        public async Task AddAsync(AccountHead accountHead)
        {
            try
            {
                if (accountHead == null) throw new ArgumentNullException(nameof(accountHead));

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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account head.", ex);
            }
        }

        public async Task UpdateAsync(AccountHead accountHead)
        {
            try
            {
                if (accountHead == null) throw new ArgumentNullException(nameof(accountHead));

                var entity = await _dbContext.tblAccountHead.FindAsync(accountHead.AccountHeadID);
                if (entity == null) throw new KeyNotFoundException("AccountHead not found.");

                entity.AccountHeadID = accountHead.AccountHeadID;
                entity.AccountHeadName = accountHead.AccountHeadName;
                entity.Code = accountHead.Code;
                entity.UserID = accountHead.UserID;

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
                throw new InvalidOperationException($"Error updating account head with ID {accountHead.AccountHeadID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
