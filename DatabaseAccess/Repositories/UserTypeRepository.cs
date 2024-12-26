using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{ 
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserType>> GetAllAsync()
        {
            try
            {
                var entities = await _dbContext.tblUserType.ToListAsync();

                return entities.Select(ut => new UserType
                {
                    UserTypeID = ut.UserTypeID,
                    UserTypeName = ut.UserType
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving user types.", ex);
            }
        }

        public async Task<UserType> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblUserType.FirstOrDefaultAsync(ut => ut.UserTypeID == id);

                return entity == null ? null : new UserType
                {
                    UserTypeID = entity.UserTypeID,
                    UserTypeName = entity.UserType
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving user type with ID {id}.", ex);
            }
        }

        public async Task AddAsync(UserType userType)
        {
            try
            {
                if (userType == null) throw new ArgumentNullException(nameof(userType));

                var entity = new tblUserType
                {
                    UserTypeID = userType.UserTypeID,
                    UserType = userType.UserTypeName
                };

                _dbContext.tblUserType.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new user type.", ex);
            }
        }

        public async Task UpdateAsync(UserType userType)
        {
            try
            {
                if (userType == null) throw new ArgumentNullException(nameof(userType));

                var entity = await _dbContext.tblUserType.FindAsync(userType.UserTypeID);
                if (entity == null) throw new KeyNotFoundException("User type not found.");

                entity.UserTypeID = userType.UserTypeID;
                entity.UserType = userType.UserTypeName;

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
                throw new InvalidOperationException($"Error updating user type with ID {userType.UserTypeID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
