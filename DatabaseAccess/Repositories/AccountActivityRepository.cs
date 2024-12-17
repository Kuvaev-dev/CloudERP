using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IAccountActivityRepository
    {
        Task<IEnumerable<tblAccountActivity>> GetAllAsync();
        Task<tblAccountActivity> GetByIdAsync(int id);
    }

    public class AccountActivityRepository : IAccountActivityRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountActivityRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblAccountActivity>> GetAllAsync()
        {
            return await _dbContext.tblAccountActivity.ToListAsync();
        }

        public async Task<tblAccountActivity> GetByIdAsync(int id)
        {
            return await _dbContext.tblAccountActivity.FindAsync(id);
        }
    }
}
