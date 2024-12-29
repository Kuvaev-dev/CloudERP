using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountControlRepository : IAccountControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<AccountControl>> GetAllAsync(int companyId, int branchId)
        {
            var entities = await _dbContext.tblAccountControl
                .AsNoTracking()
                .Include(ac => ac.tblUser)
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId)
                .ToListAsync();

            return entities.Select(ac => new AccountControl
            {
                AccountControlID = ac.AccountControlID,
                AccountControlName = ac.AccountControlName,
                AccountHeadID = ac.AccountHeadID,
                BranchID = ac.BranchID,
                CompanyID = ac.CompanyID,
                UserID = ac.UserID,
                FullName = ac.tblUser?.UserName
            });
        }

        public async Task<AccountControl> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblAccountControl
                .Include(ac => ac.tblUser)
                .FirstOrDefaultAsync(ac => ac.AccountControlID == id);

            return entity == null ? null : new AccountControl
            {
                AccountControlID = entity.AccountControlID,
                AccountControlName = entity.AccountControlName,
                AccountHeadID = entity.AccountHeadID,
                BranchID = entity.BranchID,
                CompanyID = entity.CompanyID,
                UserID = entity.UserID,
                FullName = entity.tblUser?.UserName
            };
        }

        public async Task AddAsync(AccountControl accountControl)
        {
            if (accountControl == null) throw new ArgumentNullException(nameof(accountControl));

            var entity = new tblAccountControl
            {
                AccountControlID = accountControl.AccountControlID,
                AccountControlName = accountControl.AccountControlName,
                AccountHeadID = accountControl.AccountHeadID,
                BranchID = accountControl.BranchID,
                CompanyID = accountControl.CompanyID,
                UserID = accountControl.UserID
            };

            _dbContext.tblAccountControl.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountControl accountControl)
        {
            if (accountControl == null) throw new ArgumentNullException(nameof(accountControl));

            var entity = await _dbContext.tblAccountControl.FindAsync(accountControl.AccountControlID);
            if (entity == null) throw new KeyNotFoundException("AccountControl not found.");

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;
            entity.BranchID = accountControl.BranchID;
            entity.CompanyID = accountControl.CompanyID;
            entity.UserID = accountControl.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
