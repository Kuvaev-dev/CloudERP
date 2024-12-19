using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IBranchRepository
    {
        Task<IEnumerable<tblBranch>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<tblBranch>> GetSubBranchAsync(int companyId, int branchId);
        Task<List<int?>> GetBranchIDsAsync(int branchID);
        Task<tblBranch> GetByIdAsync(int id);
        Task AddAsync(tblBranch branch);
        Task UpdateAsync(tblBranch branch);
        Task DeleteAsync(int id);
    }

    public class BranchRepository : IBranchRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblBranch>> GetByCompanyAsync(int companyId)
        {
            return await _dbContext.tblBranch
                .Include(b => b.tblBranchType)
                .Where(b => b.CompanyID == companyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<tblBranch>> GetSubBranchAsync(int companyId, int branchId)
        {
            return await _dbContext.tblBranch
                .Include(b => b.tblBranchType)
                .Where(b => b.CompanyID == companyId && b.BrchID == branchId)
                .ToListAsync();
        }

        public async Task<tblBranch> GetByIdAsync(int id)
        {
            return await _dbContext.tblBranch.Include(b => b.tblBranchType).FirstOrDefaultAsync(b => b.BranchID == id);
        }

        public async Task AddAsync(tblBranch branch)
        {
            _dbContext.tblBranch.Add(branch);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblBranch branch)
        {
            _dbContext.Entry(branch).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var branch = await _dbContext.tblBranch.FindAsync(id);
            if (branch != null)
            {
                _dbContext.tblBranch.Remove(branch);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<int?>> GetBranchIDsAsync(int branchID)
        {
            var branchIDs = await _dbContext.tblBranch
                .Where(b => b.BrchID == branchID || b.BranchID == branchID)
                .Select(b => b.BrchID)
                .ToListAsync();

            branchIDs.Add(branchID);
            return branchIDs;
        }
    }
}
