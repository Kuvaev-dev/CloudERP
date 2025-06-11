using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Account
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
                .Include(a => a.AccountHead)
                .Include(a => a.AccountControl)
                .Include(a => a.AccountSubControl)
                .Include(a => a.AccountActivity)
                .Include(a => a.Branch)
                .Include(a => a.Company)
                .Include(a => a.User)
                .Where(x => x.CompanyID == companyId && x.BranchID == branchId || x.IsGlobal == true)
                .ToListAsync();

            return entities.Select(s => new AccountSetting
            {
                AccountSettingID = s.AccountSettingID,
                AccountHeadID = s.AccountHeadID,
                AccountHeadName = s.AccountHead.AccountHeadName,
                AccountControlID = s.AccountControlID,
                AccountControlName = s.AccountControl.AccountControlName,
                AccountSubControlID = s.AccountSubControlID,
                AccountSubControlName = s.AccountSubControl.AccountSubControlName,
                AccountActivityID = s.AccountActivityID,
                AccountActivityName = s.AccountActivity.Name,
                CompanyID = s.CompanyID,
                CompanyName = s.Company.Name,
                BranchID = s.BranchID,
                BranchName = s.Branch.BranchName,
                UserID = s.UserID ?? 0,
                FullName = s.User.FullName,
                IsGlobal = s.IsGlobal ?? false
            });
        }

        public async Task<AccountSetting?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountSetting
                .Include(a => a.AccountHead)
                .Include(a => a.AccountControl)
                .Include(a => a.AccountSubControl)
                .Include(a => a.AccountActivity)
                .Include(a => a.Branch)
                .Include(a => a.Company)
                .Include(a => a.User)
                .FirstOrDefaultAsync(x => x.AccountSettingID == id);

            return entity == null ? null : new AccountSetting
            {
                AccountSettingID = entity.AccountSettingID,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHead.AccountHeadName,
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControl.AccountControlName,
                AccountSubControlID = entity.AccountSubControlID,
                AccountSubControlName = entity.AccountSubControl.AccountSubControlName,
                AccountActivityID = entity.AccountActivityID,
                AccountActivityName = entity.AccountActivity.Name,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                IsGlobal = entity.IsGlobal ?? false,
                UserID = entity.UserID ?? 0,
            };
        }

        public async Task AddAsync(AccountSetting accountSetting)
        {
            var entity = new tblAccountSetting
            {
                AccountHeadID = accountSetting.AccountHeadID,
                AccountControlID = accountSetting.AccountControlID,
                AccountSubControlID = accountSetting.AccountSubControlID,
                AccountActivityID = accountSetting.AccountActivityID,
                CompanyID = accountSetting.CompanyID,
                BranchID = accountSetting.BranchID,
                UserID = accountSetting.UserID,
                IsGlobal = accountSetting.IsGlobal
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
            entity.IsGlobal = accountSetting.IsGlobal;

            _dbContext.tblAccountSetting.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountSetting?> GetByActivityAsync(int id, int companyId, int branchId)
        {
            var entity = await _dbContext.tblAccountSetting
                .Include(a => a.AccountHead)
                .Include(a => a.AccountControl)
                .Include(a => a.AccountSubControl)
                .Include(a => a.AccountActivity)
                .Include(a => a.Company)
                .Include(a => a.Branch)
                .Include(a => a.User)
                .Where(a => a.AccountActivityID == id &&
                           ((a.CompanyID == companyId && a.BranchID == branchId) || a.IsGlobal == true))
                .FirstOrDefaultAsync();

            return entity == null ? null : new AccountSetting
            {
                AccountSettingID = entity.AccountSettingID,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHead.AccountHeadName,
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControl.AccountControlName,
                AccountSubControlID = entity.AccountSubControlID,
                AccountSubControlName = entity.AccountSubControl.AccountSubControlName,
                AccountActivityID = entity.AccountActivityID,
                AccountActivityName = entity.AccountActivity.Name,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                UserID = entity.UserID ?? 0,
                FullName = entity.User?.FullName,
                IsGlobal = entity.IsGlobal ?? false
            };
        }

        public async Task<bool> IsExists(AccountSetting accountSetting)
        {
            return await _dbContext.tblAccountSetting
                .AnyAsync(a => a.AccountHeadID == accountSetting.AccountHeadID &&
                               a.AccountControlID == accountSetting.AccountControlID &&
                               a.AccountSubControlID == accountSetting.AccountSubControlID &&
                               a.AccountActivityID == accountSetting.AccountActivityID &&
                               a.CompanyID == accountSetting.CompanyID &&
                               a.BranchID == accountSetting.BranchID &&
                               (a.IsGlobal ?? false) == accountSetting.IsGlobal);
        }
    }
}
