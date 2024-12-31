using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Branch>> GetByCompanyAsync(int companyId)
        {
            var entities = await _dbContext.tblBranch
            .Include(b => b.tblBranchType)
            .Where(b => b.CompanyID == companyId)
            .ToListAsync();

            return entities.Select(b => new Branch
            {
                BranchID = b.BranchID,
                BranchName = b.BranchName,
                BranchContact = b.BranchContact,
                BranchAddress = b.BranchAddress,
                CompanyID = b.CompanyID,
                ParentBranchID = b.BrchID,
                BranchTypeID = b.BranchTypeID,
                BranchTypeName = b.tblBranchType.BranchType
            });
        }

        public async Task<IEnumerable<Branch>> GetSubBranchAsync(int companyId, int branchId)
        {
            var entity = await _dbContext.tblBranch
                .Include(b => b.tblBranchType)
                .Where(b => b.CompanyID == companyId && b.BrchID == branchId)
                .ToListAsync();

            return entity.Select(b => new Branch
            {
                BranchID = b.BranchID,
                BranchName = b.BranchName,
                BranchContact = b.BranchContact,
                BranchAddress = b.BranchAddress,
                CompanyID = b.CompanyID,
                ParentBranchID = b.BrchID,
                BranchTypeID = b.BranchTypeID,
                BranchTypeName = b.tblBranchType.BranchType
            });
        }

        public async Task<Branch> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblBranch.
                Include(b => b.tblBranchType).
                FirstOrDefaultAsync(b => b.BranchID == id);

            return entity == null ? null : new Branch
            {
                BranchID = entity.BranchID,
                BranchName = entity.BranchName,
                BranchContact = entity.BranchContact,
                BranchAddress = entity.BranchAddress,
                CompanyID = entity.CompanyID,
                ParentBranchID = entity.BrchID,
                BranchTypeID = entity.BranchTypeID,
                BranchTypeName = entity.tblBranchType.BranchType
            };
        }

        public async Task AddAsync(Branch branch)
        {
            var entity = new tblBranch
            {
                BranchID = branch.BranchID,
                BranchTypeID = branch.BranchTypeID,
                BranchName = branch.BranchName,
                BranchContact = branch.BranchContact,
                BranchAddress = branch.BranchAddress,
                CompanyID = branch.CompanyID,
                BrchID = branch.BrchID
            };

            _dbContext.tblBranch.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Branch branch)
        {
            var entity = await _dbContext.tblBranch.FindAsync(branch.BranchID);

            entity.BranchID = branch.BranchID;
            entity.BranchTypeID = branch.BranchTypeID;
            entity.BranchName = branch.BranchName;
            entity.BranchContact = branch.BranchContact;
            entity.BranchAddress = branch.BranchAddress;
            entity.CompanyID = branch.CompanyID;
            entity.BrchID = branch.BrchID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
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
