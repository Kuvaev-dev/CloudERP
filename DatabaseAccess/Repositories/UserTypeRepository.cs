using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IUserTypeRepository
    {
        IEnumerable<tblUserType> GetAll();
        tblUserType GetById(int id);
        void Add(tblUserType userType);
        void Update(tblUserType userType);
        void Delete(tblUserType userType);
    }

    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public UserTypeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<tblUserType> GetAll()
        {
            return _dbContext.tblUserType.ToList();
        }

        public tblUserType GetById(int id)
        {
            return _dbContext.tblUserType.FirstOrDefault(ut => ut.UserTypeID == id);
        }

        public void Add(tblUserType userType)
        {
            _dbContext.tblUserType.Add(userType);
            _dbContext.SaveChanges();
        }

        public void Update(tblUserType userType)
        {
            var existingUserType = _dbContext.tblUserType.Find(userType.UserTypeID);
            if (existingUserType == null)
            {
                throw new KeyNotFoundException("UserType not found.");
            }

            existingUserType.UserType = userType.UserType;
            _dbContext.Entry(existingUserType).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(tblUserType userType)
        {
            var existingUserType = _dbContext.tblUserType.Find(userType.UserTypeID);
            if (existingUserType == null)
            {
                throw new KeyNotFoundException("UserType not found.");
            }

            _dbContext.tblUserType.Remove(existingUserType);
            _dbContext.SaveChanges();
        }
    }
}
