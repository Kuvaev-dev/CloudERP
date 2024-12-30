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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId)
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

        public async Task<AccountSubControl> GetByIdAsync(int id)
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

        public async Task AddAsync(AccountSubControl accountSubControl)
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

        public async Task UpdateAsync(AccountSubControl accountSubControl)
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

        public async Task<AccountSubControl> GetBySettingAsync(int id, int companyId, int branchId)
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
    }
}
