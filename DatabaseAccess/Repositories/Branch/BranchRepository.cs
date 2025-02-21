using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Branch
{
    public class BranchRepository : IBranchRepository
    {
        private readonly CloudDBEntities _dbContext;

        public BranchRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Domain.Models.Branch>> GetByCompanyAsync(int companyId)
        {
            var entities = await _dbContext.tblBranch
                .Include(b => b.BranchType)
                .Where(b => b.CompanyID == companyId)
                .ToListAsync();

            return entities.Select(b => new Domain.Models.Branch
            {
                BranchID = b.BranchID,
                BranchName = b.BranchName,
                BranchContact = b.BranchContact,
                BranchAddress = b.BranchAddress,
                CompanyID = b.CompanyID,
                ParentBranchID = b.BrchID,
                BranchTypeID = b.BranchTypeID,
                BranchTypeName = b.BranchType.BranchType
            });
        }

        public async Task<IEnumerable<Domain.Models.Branch>> GetSubBranchAsync(int companyId, int branchId)
        {
            var entity = await _dbContext.tblBranch
                .Include(b => b.BranchType)
                .Where(b => b.CompanyID == companyId && b.BrchID == branchId)
                .ToListAsync();

            return entity.Select(b => new Domain.Models.Branch
            {
                BranchID = b.BranchID,
                BranchName = b.BranchName,
                BranchContact = b.BranchContact,
                BranchAddress = b.BranchAddress,
                CompanyID = b.CompanyID,
                ParentBranchID = b.BrchID,
                BranchTypeID = b.BranchTypeID,
                BranchTypeName = b.BranchType.BranchType
            });
        }

        public async Task<Domain.Models.Branch?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblBranch.
                Include(b => b.BranchType).
                FirstOrDefaultAsync(b => b.BranchID == id);

            return entity == null ? null : new Domain.Models.Branch
            {
                BranchID = entity.BranchID,
                BranchName = entity.BranchName,
                BranchContact = entity.BranchContact,
                BranchAddress = entity.BranchAddress,
                CompanyID = entity.CompanyID,
                ParentBranchID = entity.BrchID,
                BranchTypeID = entity.BranchTypeID,
                BranchTypeName = entity.BranchType.BranchType
            };
        }

        public async Task AddAsync(Domain.Models.Branch branch)
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

            branch.CompanyID = entity.CompanyID;
            branch.BranchID = entity.BranchID;
        }

        public async Task UpdateAsync(Domain.Models.Branch branch)
        {
            var entity = await _dbContext.tblBranch.FindAsync(branch.BranchID);

            entity.BranchID = branch.BranchID;
            entity.BranchTypeID = branch.BranchTypeID;
            entity.BranchName = branch.BranchName;
            entity.BranchContact = branch.BranchContact;
            entity.BranchAddress = branch.BranchAddress;

            _dbContext.tblBranch.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<int>> GetBranchIDsAsync(int branchID)
        {
            var branchIDs = await _dbContext.tblBranch
                .Where(b => b.BrchID == branchID || b.BranchID == branchID)
                .Select(b => b.BranchID)
                .ToListAsync();

            branchIDs.Add(branchID);
            return branchIDs;
        }
    }
}
