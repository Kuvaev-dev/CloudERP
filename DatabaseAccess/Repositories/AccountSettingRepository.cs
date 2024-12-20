using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IAccountSettingRepository
    {
        Task<IEnumerable<tblAccountSetting>> GetAllAsync(int companyId, int branchId);
        Task<tblAccountSetting> GetByIdAsync(int id);
        Task<tblAccountSetting> GetByActivityAsync(int id, int companyId, int branchId);
        Task AddAsync(tblAccountSetting accountSetting);
        Task UpdateAsync(tblAccountSetting accountSetting);
    }

    public class AccountSettingRepository : IAccountSettingRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountSettingRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblAccountSetting>> GetAllAsync(int companyId, int branchId)
        {
            return await _dbContext.tblAccountSetting
                .Include(a => a.tblAccountHead)
                .Include(a => a.tblAccountControl)
                .Include(a => a.tblAccountSubControl)
                .Include(a => a.tblAccountActivity)
                .Include(a => a.tblBranch)
                .Include(a => a.tblCompany)
                .Where(x => x.CompanyID == companyId && x.BranchID == branchId)
                .ToListAsync();
        }

        public async Task<tblAccountSetting> GetByIdAsync(int id)
        {
            return await _dbContext.tblAccountSetting
                .Include(a => a.tblAccountHead)
                .Include(a => a.tblAccountControl)
                .Include(a => a.tblAccountSubControl)
                .Include(a => a.tblAccountActivity)
                .Include(a => a.tblBranch)
                .Include(a => a.tblCompany)
                .FirstOrDefaultAsync(x => x.AccountSettingID == id);
        }

        public async Task AddAsync(tblAccountSetting accountSetting)
        {
            _dbContext.tblAccountSetting.Add(accountSetting);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblAccountSetting accountSetting)
        {
            var existing = await _dbContext.tblAccountSetting.FindAsync(accountSetting.AccountSettingID);
            if (existing == null)
                throw new KeyNotFoundException("Account setting not found.");

            _dbContext.Entry(existing).CurrentValues.SetValues(accountSetting);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<tblAccountSetting> GetByActivityAsync(int id, int companyId, int branchId)
        {
            return await _dbContext.tblAccountSetting
                        .Where(a => a.AccountActivityID == id && a.CompanyID == companyId && a.BranchID == branchId)
                        .FirstOrDefaultAsync();
        }
    }
}
