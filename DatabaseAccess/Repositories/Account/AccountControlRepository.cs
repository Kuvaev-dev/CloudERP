using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Account
{
    public class AccountControlRepository : IAccountControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<AccountControl>> GetAllAsync(int companyId, int branchId)
        {
            var entities = await _dbContext.tblAccountControl
                .AsNoTracking()
                .Include(ac => ac.User)
                .Include(ac => ac.AccountHead)
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId || ac.IsGlobal == true)
                .ToListAsync();

            return entities.Select(ac => new AccountControl
            {
                AccountControlID = ac.AccountControlID,
                AccountControlName = ac.AccountControlName,
                AccountHeadID = ac.AccountHeadID,
                AccountHeadName = ac.AccountHead.AccountHeadName,
                BranchID = ac.BranchID,
                CompanyID = ac.CompanyID,
                UserID = ac.UserID,
                FullName = ac.User?.UserName,
                IsGlobal = ac.IsGlobal
            });
        }

        public async Task<AccountControl?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountControl
                .Include(ac => ac.User)
                .Include(ac => ac.AccountHead)
                .FirstOrDefaultAsync(ac => ac.AccountControlID == id);

            return entity == null ? null : new AccountControl
            {
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                AccountHeadName = entity.AccountHead.AccountHeadName,
                BranchID = entity.BranchID,
                CompanyID = entity.CompanyID,
                UserID = entity.UserID,
                FullName = entity.User.UserName,
                IsGlobal = entity.IsGlobal
            };
        }

        public async Task AddAsync(AccountControl accountControl)
        {
            var entity = new tblAccountControl
            {
                AccountControlName = accountControl.AccountControlName,
                AccountHeadID = accountControl.AccountHeadID,
                BranchID = accountControl.BranchID,
                CompanyID = accountControl.CompanyID,
                UserID = accountControl.UserID,
                IsGlobal = accountControl.IsGlobal
            };

            _dbContext.tblAccountControl.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountControl accountControl)
        {
            var entity = await _dbContext.tblAccountControl.FindAsync(accountControl.AccountControlID);

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;

            _dbContext.tblAccountControl.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
