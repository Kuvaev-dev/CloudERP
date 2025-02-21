using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Account
{
    public class AccountSubControlRepository : IAccountSubControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountSubControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId)
        {
            var entities = await _dbContext.tblAccountSubControl
                .AsNoTracking()
                .Include(ac => ac.User)
                .Include(ac => ac.AccountControl)
                .Include(ah => ah.AccountHead)
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId || ac.IsGlobal == true)
                .ToListAsync();

            return entities.Select(asc => new AccountSubControl
            {
                AccountSubControlID = asc.AccountSubControlID,
                AccountSubControlName = asc.AccountSubControlName,
                AccountControlID = asc.AccountControlID,
                AccountControlName = asc.AccountControl.AccountControlName,
                AccountHeadID = asc.AccountHeadID,
                AccountHeadName = asc.AccountHead.AccountHeadName,
                CompanyID = asc.CompanyID,
                BranchID = asc.BranchID,
                UserID = asc.UserID,
                FullName = asc.User.FullName,
                IsGlobal = asc.IsGlobal ?? false
            });
        }

        public async Task<AccountSubControl?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountSubControl
                .Include(ac => ac.User)
                .Include(ac => ac.AccountControl)
                .Include(ah => ah.AccountHead)
                .FirstOrDefaultAsync(ac => ac.AccountSubControlID == id);

            return entity == null ? null : new AccountSubControl
            {
                AccountSubControlID = entity.AccountSubControlID,
                AccountSubControlName = entity.AccountSubControlName,
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControl.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHead.AccountHeadName,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID,
                FullName = entity.User.FullName
            };
        }

        public async Task AddAsync(AccountSubControl accountSubControl)
        {
            var entity = new tblAccountSubControl
            {
                AccountSubControlID = accountSubControl.AccountSubControlID,
                AccountSubControlName = accountSubControl.AccountSubControlName,
                AccountControlID = accountSubControl.AccountControlID,
                AccountHeadID = accountSubControl.AccountHeadID,
                CompanyID = accountSubControl.CompanyID,
                BranchID = accountSubControl.BranchID,
                UserID = accountSubControl.UserID,
                IsGlobal = accountSubControl.IsGlobal
            };

            _dbContext.tblAccountSubControl.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountSubControl accountSubControl)
        {
            var entity = await _dbContext.tblAccountSubControl.FindAsync(accountSubControl.AccountSubControlID);

            entity.AccountSubControlName = accountSubControl.AccountSubControlName;
            entity.AccountControlID = accountSubControl.AccountControlID;
            entity.AccountHeadID = accountSubControl.AccountHeadID;

            _dbContext.tblAccountSubControl.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountSubControl?> GetBySettingAsync(int id, int companyId, int branchId)
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
                AccountControlName = entity.AccountControl.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHead.AccountHeadName,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID,
                FullName = entity.User.FullName
            };
        }
    }
}
