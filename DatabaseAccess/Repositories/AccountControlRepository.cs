using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IAccountControlRepository
    {
        Task<IEnumerable<tblAccountControl>> GetAllAsync(int companyId, int branchId);
        Task<tblAccountControl> GetByIdAsync(int id);
        Task AddAsync(tblAccountControl accountControl);
        Task UpdateAsync(tblAccountControl accountControl);
    }

    public class AccountControlRepository : IAccountControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblAccountControl>> GetAllAsync(int companyId, int branchId)
        {
            return await _dbContext.tblAccountControl
                .AsNoTracking()
                .Include(ac => ac.tblUser)
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId)
                .ToListAsync();
        }

        public async Task<tblAccountControl> GetByIdAsync(int id)
        {
            return await _dbContext.tblAccountControl.FirstOrDefaultAsync(ac => ac.AccountControlID == id);
        }

        public async Task AddAsync(tblAccountControl accountControl)
        {
            _dbContext.tblAccountControl.Add(accountControl);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblAccountControl accountControl)
        {
            var entity = await _dbContext.tblAccountControl.FindAsync(accountControl.AccountControlID);
            if (entity == null) throw new KeyNotFoundException("AccountControl not found.");

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;
            entity.BranchID = accountControl.BranchID;
            entity.CompanyID = accountControl.CompanyID;
            entity.UserID = accountControl.UserID;

            _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
