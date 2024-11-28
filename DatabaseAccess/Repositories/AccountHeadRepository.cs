using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IAccountHeadRepository
    {
        IEnumerable<tblAccountHead> GetAll();
        tblAccountHead GetById(int id);
        void Add(tblAccountHead accountHead);
        void Update(tblAccountHead accountHead);
        void Delete(tblAccountHead accountHead);
    }

    public class AccountHeadRepository : IAccountHeadRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountHeadRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<tblAccountHead> GetAll()
        {
            return _dbContext.tblAccountHead.ToList();
        }

        public tblAccountHead GetById(int id)
        {
            return _dbContext.tblAccountHead.FirstOrDefault(ah => ah.AccountHeadID == id);
        }

        public void Add(tblAccountHead accountHead)
        {
            _dbContext.tblAccountHead.Add(accountHead);
            _dbContext.SaveChanges();
        }

        public void Update(tblAccountHead accountHead)
        {
            var entity = _dbContext.tblAccountHead.Find(accountHead.AccountHeadID);
            if (entity == null) throw new KeyNotFoundException("AccountHead not found.");

            entity.AccountHeadName = accountHead.AccountHeadName;
            entity.Code = accountHead.Code;
            entity.UserID = accountHead.UserID;

            _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(tblAccountHead accountHead)
        {
            _dbContext.tblAccountHead.Remove(accountHead);
            _dbContext.SaveChanges();
        }
    }
}
