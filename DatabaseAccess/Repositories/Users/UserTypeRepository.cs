using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;
using Utils.Helpers;

namespace DatabaseAccess.Repositories.Users
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DatabaseQuery));
        }

        public async Task<IEnumerable<UserType>> GetAllAsync()
        {
            var entities = await _dbContext.tblUserType.ToListAsync();

            return entities.Select(ut => new UserType
            {
                UserTypeID = ut.UserTypeID,
                UserTypeName = ut.UserType
            });
        }

        public async Task<UserType?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblUserType.FirstOrDefaultAsync(ut => ut.UserTypeID == id);

            return entity == null ? null : new UserType
            {
                UserTypeID = entity.UserTypeID,
                UserTypeName = entity.UserType
            };
        }

        public async Task AddAsync(UserType userType)
        {
            var entity = new tblUserType
            {
                UserTypeID = userType.UserTypeID,
                UserType = userType.UserTypeName
            };

            _dbContext.tblUserType.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserType userType)
        {
            var entity = await _dbContext.tblUserType.FindAsync(userType.UserTypeID);

            entity.UserTypeID = userType.UserTypeID;
            entity.UserType = userType.UserTypeName;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
