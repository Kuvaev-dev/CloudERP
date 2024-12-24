using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class AccountActivityRepository : IAccountActivityRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountActivityRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<AccountActivity>> GetAllAsync()
        {
            try
            {
                var entities = await _dbContext.tblAccountActivity
                    .AsNoTracking()
                    .ToListAsync();

                return entities.Select(aa => new AccountActivity
                {
                    AccountActivityID = aa.AccountActivityID,
                    Name = aa.Name
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving account heads.", ex);
            }
        }

        public async Task<AccountActivity> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblAccountActivity
                    .FirstOrDefaultAsync(ac => ac.AccountActivityID == id);

                return entity == null ? null : new AccountActivity
                {
                    AccountActivityID = entity.AccountActivityID,
                    Name = entity.Name
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving account head with ID {id}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
