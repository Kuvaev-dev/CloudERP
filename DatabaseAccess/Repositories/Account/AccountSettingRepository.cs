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

        private const int DEFAULT_ACCOUNT_HEAD_ID = 7;
        private const int DEFAULT_ACCOUNT_CONTROL_ID = 2;
        private const int DEFAULT_ACCOUNT_SUB_CONTROL_ID = 1;
        private const int ACCOUNT_SETTINGS_COUNT = 20;

        public AccountSettingRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<AccountSetting>> GetAllAsync(int companyId, int branchId)
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

        public async Task<AccountSetting> GetByIdAsync(int id)
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

        public async Task SetDefault(int companyId, int branchId)
        {
            for (int id = 1; id <= ACCOUNT_SETTINGS_COUNT; id++)
            {
                var entity = new AccountSetting
                {
                    AccountHeadID = DEFAULT_ACCOUNT_HEAD_ID,
                    AccountControlID = DEFAULT_ACCOUNT_CONTROL_ID,
                    AccountSubControlID = DEFAULT_ACCOUNT_SUB_CONTROL_ID,
                    AccountActivityID = id,
                    CompanyID = companyId,
                    BranchID = branchId
                };
                await AddAsync(entity);
            }
        }

        public async Task AddAsync(AccountSetting accountSetting)
        {
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

        public async Task UpdateAsync(AccountSetting accountSetting)
        {
            var entity = await _dbContext.tblAccountSetting.FindAsync(accountSetting.AccountSettingID);

            entity.AccountHeadID = accountSetting.AccountHeadID;
            entity.AccountControlID = accountSetting.AccountControlID;
            entity.AccountSubControlID = accountSetting.AccountSubControlID;
            entity.AccountActivityID = accountSetting.AccountActivityID;
            entity.CompanyID = accountSetting.CompanyID;
            entity.BranchID = accountSetting.BranchID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountSetting> GetByActivityAsync(int id, int companyId, int branchId)
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
    }
}
