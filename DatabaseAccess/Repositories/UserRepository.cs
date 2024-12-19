using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<tblUser>> GetAllAsync();
        Task<IEnumerable<tblUser>> GetByBranchAsync(int companyId, int branchTypeId, int branchId);
        Task<tblUser> GetByEmailAsync(string email);
        Task<tblUser> GetByPasswordCodesAsync(string id, DateTime expiration);
        Task<tblUser> GetByIdAsync(int id);
        Task AddAsync(tblUser user);
        Task UpdateAsync(tblUser user);
        Task DeleteAsync(int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblUser>> GetAllAsync()
        {
            return await _dbContext.tblUser.Include(u => u.tblUserType).ToListAsync();
        }

        public async Task<IEnumerable<tblUser>> GetByBranchAsync(int companyId, int branchTypeId, int branchId)
        {
            if (branchTypeId == 1) // Main Branch
            {
                return await _dbContext.tblUser
                    .Join(_dbContext.tblEmployee, u => u.UserID, e => e.UserID, (u, e) => new { u, e })
                    .Where(x => x.e.CompanyID == companyId)
                    .Select(x => x.u)
                    .ToListAsync();
            }
            else // Sub Branch
            {
                return await _dbContext.tblUser
                    .Join(_dbContext.tblEmployee, u => u.UserID, e => e.UserID, (u, e) => new { u, e })
                    .Where(x => x.e.tblBranch.BrchID == branchId)
                    .Select(x => x.u)
                    .ToListAsync();
            }
        }

        public async Task<tblUser> GetByIdAsync(int id)
        {
            return await _dbContext.tblUser.FindAsync(id);
        }

        public async Task AddAsync(tblUser user)
        {
            _dbContext.tblUser.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblUser user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _dbContext.tblUser.FindAsync(id);
            if (user != null)
            {
                _dbContext.tblUser.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<tblUser> GetByEmailAsync(string email)
        {
            return await _dbContext.tblUser.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<tblUser> GetByPasswordCodesAsync(string id, DateTime expiration)
        {
            return await _dbContext.tblUser.FirstOrDefaultAsync(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > expiration);
        }
    }
}
