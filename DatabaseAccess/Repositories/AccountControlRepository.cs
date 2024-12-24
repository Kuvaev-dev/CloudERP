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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account controls.", ex);
            }
        }

        public async Task<AccountControl> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account control with ID {id}.", ex);
            }
        }

        public async Task AddAsync(AccountControl accountControl)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new account control.", ex);
            }
        }

        public async Task UpdateAsync(AccountControl accountControl)
        {
            try
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
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating account control with ID {accountControl.AccountControlID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
