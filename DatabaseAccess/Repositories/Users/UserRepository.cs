using DatabaseAccess.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DatabaseQuery));
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var entities = await _dbContext.tblUser.ToListAsync();

            return entities.Select(u => new User
            {
                UserID = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                ContactNo = u.ContactNo,
                UserName = u.UserName,
                Password = u.Password,
                Salt = u.Salt,
                UserTypeID = u.UserTypeID,
                UserTypeName = u.tblUserType.UserType,
                IsActive = u.IsActive
            });
        }

        public async Task<IEnumerable<User>> GetByBranchAsync(int companyId, int branchTypeId, int branchId)
        {
            var query = _dbContext.tblUser
                .Join(_dbContext.tblEmployee, u => u.UserID, e => e.UserID, (u, e) => new { u, e });

            if (branchTypeId == 1) // Main Branch
            {
                query = query.Where(x => x.e.CompanyID == companyId);
            }
            else // Sub Branch
            {
                query = query.Where(x => x.e.tblBranch.BrchID == branchId);
            }

            var entities = await query.Select(x => new User
            {
                UserID = x.u.UserID,
                FullName = x.u.FullName,
                Email = x.u.Email,
                ContactNo = x.u.ContactNo,
                UserName = x.u.UserName,
                Password = x.u.Password,
                Salt = x.u.Salt,
                UserTypeID = x.u.UserTypeID,
                UserTypeName = x.u.tblUserType.UserType,
                IsActive = x.u.IsActive
            }).ToListAsync();

            return entities;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblUser.FindAsync(id);

            return entity == null ? null : new User
            {
                UserID = entity.UserID,
                FullName = entity.FullName,
                Email = entity.Email,
                ContactNo = entity.ContactNo,
                UserName = entity.UserName,
                Password = entity.Password,
                Salt = entity.Salt,
                UserTypeID = entity.UserTypeID,
                UserTypeName = entity.tblUserType.UserType,
                IsActive = entity.IsActive
            };
        }

        public async Task AddAsync(User user)
        {
            var entity = new tblUser
            {
                UserID = user.UserID,
                UserTypeID = 2,
                FullName = user.FullName,
                Email = user.Email,
                ContactNo = user.ContactNo,
                UserName = user.UserName,
                Password = user.Password,
                ResetPasswordCode = "",
                Salt = user.Salt,
                LastPasswordResetRequest = null,
                ResetPasswordExpiration = null,
                IsActive = true
            };

            _dbContext.tblUser.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var entity = await _dbContext.tblUser.FindAsync(user.UserID);

            entity.UserID = user.UserID;
            entity.UserTypeID = user.UserTypeID;
            entity.FullName = user.FullName;
            entity.Email = user.Email;
            entity.ContactNo = user.ContactNo;
            entity.UserName = user.UserName;
            entity.Password = user.Password;
            entity.ResetPasswordCode = user.ResetPasswordCode;
            entity.LastPasswordResetRequest = user.LastPasswordResetRequest;
            entity.ResetPasswordExpiration = user.ResetPasswordExpiration;
            entity.IsActive = user.IsActive;
            entity.Salt = user.Salt;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity = await _dbContext.tblUser.SingleOrDefaultAsync(u => u.Email == email);

            return entity == null ? null : new User
            {
                UserID = entity.UserID,
                FullName = entity.FullName,
                Email = entity.Email,
                ContactNo = entity.ContactNo,
                UserName = entity.UserName,
                Password = entity.Password,
                Salt = entity.Salt,
                UserTypeID = entity.UserTypeID,
                UserTypeName = entity.tblUserType.UserType,
                IsActive = entity.IsActive
            };
        }

        public async Task<User> GetByPasswordCodesAsync(string id, DateTime expiration)
        {
            var entity = await _dbContext.tblUser
                .FirstOrDefaultAsync(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > expiration);

            return entity == null ? null : new User
            {
                UserID = entity.UserID,
                FullName = entity.FullName,
                Email = entity.Email,
                ContactNo = entity.ContactNo,
                UserName = entity.UserName,
                Password = entity.Password,
                Salt = entity.Salt,
                UserTypeID = entity.UserTypeID,
                UserTypeName = entity.tblUserType.UserType,
                IsActive = entity.IsActive
            };
        }
    }
}
