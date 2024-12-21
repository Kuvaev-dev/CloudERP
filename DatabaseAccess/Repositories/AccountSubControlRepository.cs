using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IAccountSubControlRepository
    {
        Task<IEnumerable<tblAccountSubControl>> GetAllAsync(int companyId, int branchId);
        Task<tblAccountSubControl> GetByIdAsync(int id);
        Task<tblAccountSubControl> GetBySettingAsync(int id, int companyId, int branchId);
        Task AddAsync(tblAccountSubControl entity);
        Task UpdateAsync(tblAccountSubControl entity);
    }

    public class AccountSubControlRepository : IAccountSubControlRepository
    {
        private readonly CloudDBEntities _db;

        public AccountSubControlRepository(CloudDBEntities db)
        {
            _db = db;
        }

        public async Task<IEnumerable<tblAccountSubControl>> GetAllAsync(int companyId, int branchId)
        {
            return await _db.tblAccountSubControl
                .Include(ac => ac.tblAccountControl)
                .Include(ah => ah.tblAccountHead)
                .Include(u => u.tblUser)
                .Where(x => x.CompanyID == companyId && x.BranchID == branchId)
                .ToListAsync();
        }

        public async Task<tblAccountSubControl> GetByIdAsync(int id)
        {
            return await _db.tblAccountSubControl.FindAsync(id);
        }

        public async Task AddAsync(tblAccountSubControl entity)
        {
            _db.tblAccountSubControl.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblAccountSubControl entity)
        {
            _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<tblAccountSubControl> GetBySettingAsync(int id, int companyId, int branchId)
        {
            var asc = await _db.tblAccountSubControl.FirstOrDefaultAsync(a =>
                        a.AccountSubControlID == id &&
                        a.CompanyID == companyId &&
                        a.BranchID == branchId);
            return asc;
        }
    }
}
