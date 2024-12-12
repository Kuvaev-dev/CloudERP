using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IUserTypeRepository
    {
        Task<IEnumerable<tblUserType>> GetAllAsync();
        Task<tblUserType> GetByIdAsync(int id);
        Task AddAsync(tblUserType userType);
        Task UpdateAsync(tblUserType userType);
        Task DeleteAsync(tblUserType userType);
    }

    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblUserType>> GetAllAsync()
        {
            return await _dbContext.tblUserType.ToListAsync();
        }

        public async Task<tblUserType> GetByIdAsync(int id)
        {
            return await _dbContext.tblUserType.FirstOrDefaultAsync(ut => ut.UserTypeID == id);
        }

        public async Task AddAsync(tblUserType userType)
        {
            _dbContext.tblUserType.Add(userType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblUserType userType)
        {
            var existingUserType = await _dbContext.tblUserType.FindAsync(userType.UserTypeID);
            if (existingUserType == null)
            {
                throw new KeyNotFoundException("UserType not found.");
            }

            existingUserType.UserType = userType.UserType;
            _dbContext.Entry(existingUserType).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(tblUserType userType)
        {
            var existingUserType = await _dbContext.tblUserType.FindAsync(userType.UserTypeID);
            if (existingUserType == null)
            {
                throw new KeyNotFoundException("UserType not found.");
            }

            _dbContext.tblUserType.Remove(existingUserType);
            await _dbContext.SaveChangesAsync();
        }
    }
}
