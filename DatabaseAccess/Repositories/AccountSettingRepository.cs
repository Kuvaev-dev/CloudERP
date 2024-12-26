using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountSettingRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountSetting>> GetAllAsync(int companyId, int branchId)
        {
            try
            {
                var entities = await _dbContext.tblAccountSetting
                    .Include(a => a.tblAccountHead)
                    .Include(a => a.tblAccountControl)
                    .Include(a => a.tblAccountSubControl)
                    .Include(a => a.tblAccountActivity)
                    .Include(a => a.tblBranch)
                    .Include(a => a.tblCompany)
                    .Where(x => x.CompanyID == companyId && x.BranchID == branchId)
                    .ToListAsync();

                return entities.Select(s => new AccountSetting
                {
                    AccountSettingID = s.AccountSettingID,
                    AccountHeadID = s.AccountHeadID,
                    AccountHeadName = s.tblAccountHead.AccountHeadName,
                    AccountControlID = s.AccountControlID,
                    AccountControlName = s.tblAccountControl.AccountControlName,
                    AccountSubControlID = s.AccountSubControlID,
                    AccountSubControlName = s.tblAccountSubControl.AccountSubControlName,
                    AccountActivityID = s.AccountActivityID,
                    AccountActivityName = s.tblAccountActivity.Name,
                    CompanyID = s.CompanyID,
                    CompanyName = s.tblCompany.Name,
                    BranchID = s.BranchID,
                    BranchName = s.tblBranch.BranchName
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account settings.", ex);
            }
        }

        public async Task<AccountSetting> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblAccountSetting
                    .Include(a => a.tblAccountHead)
                    .Include(a => a.tblAccountControl)
                    .Include(a => a.tblAccountSubControl)
                    .Include(a => a.tblAccountActivity)
                    .Include(a => a.tblBranch)
                    .Include(a => a.tblCompany)
                    .FirstOrDefaultAsync(x => x.AccountSettingID == id);

                return entity == null ? null : new AccountSetting
                {
                    AccountSettingID = entity.AccountSettingID,
                    AccountHeadID = entity.AccountHeadID,
                    AccountHeadName = entity.tblAccountHead.AccountHeadName,
                    AccountControlID = entity.AccountControlID,
                    AccountControlName = entity.tblAccountControl.AccountControlName,
                    AccountSubControlID = entity.AccountSubControlID,
                    AccountSubControlName = entity.tblAccountSubControl.AccountSubControlName,
                    AccountActivityID = entity.AccountActivityID,
                    AccountActivityName = entity.tblAccountActivity.Name,
                    CompanyID = entity.CompanyID,
                    CompanyName = entity.tblCompany.Name,
                    BranchID = entity.BranchID,
                    BranchName = entity.tblBranch.BranchName
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account setting with ID {id}.", ex);
            }
        }

        public async Task AddAsync(AccountSetting accountSetting)
        {
            try
            {
                if (accountSetting == null) throw new ArgumentNullException(nameof(accountSetting));

                var entity = new tblAccountSetting
                {
                    AccountSettingID = accountSetting.AccountSettingID,
                    AccountHeadID = accountSetting.AccountHeadID,
                    AccountControlID = accountSetting.AccountControlID,
                    AccountSubControlID = accountSetting.AccountSubControlID,
                    AccountActivityID = accountSetting.AccountActivityID,
                    CompanyID = accountSetting.CompanyID,
                    BranchID = accountSetting.BranchID
                };

                _dbContext.tblAccountSetting.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account setting.", ex);
            }
        }

        public async Task UpdateAsync(AccountSetting accountSetting)
        {
            try
            {
                if (accountSetting == null) throw new ArgumentNullException(nameof(accountSetting));

                var entity = await _dbContext.tblAccountSetting.FindAsync(accountSetting.AccountSettingID);
                if (entity == null) throw new KeyNotFoundException("Account setting not found.");

                entity.AccountSettingID = entity.AccountSettingID;
                entity.AccountHeadID = entity.AccountHeadID;
                entity.AccountControlID = entity.AccountControlID;
                entity.AccountSubControlID = entity.AccountSubControlID;
                entity.AccountActivityID = entity.AccountActivityID;
                entity.CompanyID = entity.CompanyID;
                entity.BranchID = entity.BranchID;

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
                throw new InvalidOperationException($"Error updating account setting with ID {accountSetting.AccountSettingID}.", ex);
            }
        }

        public async Task<AccountSetting> GetByActivityAsync(int id, int companyId, int branchId)
        {
            try
            {
                var entity = await _dbContext.tblAccountSetting
                    .Where(a => a.AccountActivityID == id && a.CompanyID == companyId && a.BranchID == branchId)
                    .FirstOrDefaultAsync();

                return entity == null ? null : new AccountSetting
                {
                    AccountSettingID = entity.AccountSettingID,
                    AccountHeadID = entity.AccountHeadID,
                    AccountHeadName = entity.tblAccountHead.AccountHeadName,
                    AccountControlID = entity.AccountControlID,
                    AccountControlName = entity.tblAccountControl.AccountControlName,
                    AccountSubControlID = entity.AccountSubControlID,
                    AccountSubControlName = entity.tblAccountSubControl.AccountSubControlName,
                    AccountActivityID = entity.AccountActivityID,
                    AccountActivityName = entity.tblAccountActivity.Name,
                    CompanyID = entity.CompanyID,
                    CompanyName = entity.tblCompany.Name,
                    BranchID = entity.BranchID,
                    BranchName = entity.tblBranch.BranchName
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account setting with activity {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
