using Domain.Models;
using Domain.RepositoryAccess;
using System;
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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Branch>> GetByCompanyAsync(int companyId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByCompanyAsync), ex);
                throw new InvalidOperationException("Error retrieving branches.", ex);
            }
        }

        public async Task<IEnumerable<Branch>> GetSubBranchAsync(int companyId, int branchId)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving sub branch.", ex);
            }
        }

        public async Task<Branch> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving branch with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Branch branch)
        {
            try
            {
                if (branch == null) throw new ArgumentNullException(nameof(branch));

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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new branch.", ex);
            }
        }

        public async Task UpdateAsync(Branch branch)
        {
            try
            {
                if (branch == null) throw new ArgumentNullException(nameof(branch));

                var entity = await _dbContext.tblBranch.FindAsync(branch.BranchID);
                if (entity == null) throw new KeyNotFoundException("Branch not found.");

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
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating branch with ID {branch.BranchID}.", ex);
            }
        }

        public async Task<List<int?>> GetBranchIDsAsync(int branchID)
        {
            try
            {
                var branchIDs = await _dbContext.tblBranch
                .Where(b => b.BrchID == branchID || b.BranchID == branchID)
                .Select(b => b.BrchID)
                .ToListAsync();

                branchIDs.Add(branchID);
                return branchIDs;
            }
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error retrieving sub branches with branch ID {branchID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
