using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountSubControlRepository : IAccountSubControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountSubControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId)
        {
            try
            {
                var entities = await _dbContext.tblAccountSubControl
                    .AsNoTracking()
                    .Include(ac => ac.tblUser)
                    .Include(ac => ac.tblAccountControl)
                    .Include(ah => ah.tblAccountHead)
                    .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId)
                    .ToListAsync();

                return entities.Select(asc => new AccountSubControl
                {
                    AccountSubControlID = asc.AccountSubControlID,
                    AccountSubControlName = asc.AccountSubControlName,
                    AccountControlID = asc.AccountControlID,
                    AccountControlName = asc.tblAccountControl.AccountControlName,
                    AccountHeadID = asc.AccountHeadID,
                    AccountHeadName = asc.tblAccountHead.AccountHeadName,
                    CompanyID = asc.CompanyID,
                    BranchID = asc.BranchID,
                    UserID = asc.UserID,
                    FullName = asc.tblUser.FullName
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account sub controls.", ex);
            }
        }

        public async Task<AccountSubControl> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblAccountSubControl
                    .Include(ac => ac.tblUser)
                    .Include(ac => ac.tblAccountControl)
                    .Include(ah => ah.tblAccountHead)
                    .FirstOrDefaultAsync(ac => ac.AccountControlID == id);

                return entity == null ? null : new AccountSubControl
                {
                    AccountSubControlID = entity.AccountSubControlID,
                    AccountSubControlName = entity.AccountSubControlName,
                    AccountControlID = entity.AccountControlID,
                    AccountControlName = entity.tblAccountControl.AccountControlName,
                    AccountHeadID = entity.AccountHeadID,
                    AccountHeadName = entity.tblAccountHead.AccountHeadName,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    UserID = entity.UserID,
                    FullName = entity.tblUser.FullName
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account sub control with ID {id}.", ex);
            }
        }

        public async Task AddAsync(AccountSubControl accountSubControl)
        {
            try
            {
                if (accountSubControl == null) throw new ArgumentNullException(nameof(accountSubControl));

                var entity = new tblAccountSubControl
                {
                    AccountSubControlID = accountSubControl.AccountSubControlID,
                    AccountSubControlName = accountSubControl.AccountSubControlName,
                    AccountControlID = accountSubControl.AccountControlID,
                    AccountHeadID = accountSubControl.AccountHeadID,
                    CompanyID = accountSubControl.CompanyID,
                    BranchID = accountSubControl.BranchID,
                    UserID = accountSubControl.UserID
                };

                _dbContext.tblAccountSubControl.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account sub control.", ex);
            }
        }

        public async Task UpdateAsync(AccountSubControl accountSubControl)
        {
            try
            {
                if (accountSubControl == null) throw new ArgumentNullException(nameof(accountSubControl));

                var entity = await _dbContext.tblAccountSubControl.FindAsync(accountSubControl.AccountSubControlID);
                if (entity == null) throw new KeyNotFoundException("AccountSubControl not found.");

                entity.AccountSubControlID = accountSubControl.AccountSubControlID;
                entity.AccountSubControlName = accountSubControl.AccountSubControlName;
                entity.AccountControlID = accountSubControl.AccountControlID;
                entity.AccountHeadID = accountSubControl.AccountHeadID;
                entity.CompanyID = accountSubControl.CompanyID;
                entity.BranchID = accountSubControl.BranchID;
                entity.UserID = accountSubControl.UserID;

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
                throw new InvalidOperationException($"Error updating account sub control with ID {accountSubControl.AccountSubControlID}.", ex);
            }
        }

        public async Task<AccountSubControl> GetBySettingAsync(int id, int companyId, int branchId)
        {
            try
            {
                var entity = await _dbContext.tblAccountSubControl.FirstOrDefaultAsync(a =>
                        a.AccountSubControlID == id &&
                        a.CompanyID == companyId &&
                        a.BranchID == branchId);

                return entity == null ? null : new AccountSubControl
                {
                    AccountSubControlID = entity.AccountSubControlID,
                    AccountSubControlName = entity.AccountSubControlName,
                    AccountControlID = entity.AccountControlID,
                    AccountControlName = entity.tblAccountControl.AccountControlName,
                    AccountHeadID = entity.AccountHeadID,
                    AccountHeadName = entity.tblAccountHead.AccountHeadName,
                    CompanyID = entity.CompanyID,
                    BranchID = entity.BranchID,
                    UserID = entity.UserID,
                    FullName = entity.tblUser.FullName
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account sub control with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
