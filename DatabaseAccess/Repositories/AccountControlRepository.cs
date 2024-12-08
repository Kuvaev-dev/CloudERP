using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IAccountControlRepository
    {
        IEnumerable<tblAccountControl> GetAll(int companyId, int branchId);
        tblAccountControl GetById(int id);
        void Add(tblAccountControl accountControl);
        void Update(tblAccountControl accountControl);
    }

    public class AccountControlRepository : IAccountControlRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountControlRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<tblAccountControl> GetAll(int companyId, int branchId)
        {
            return _dbContext.tblAccountControl
                .Include("tblUser")
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId)
                .ToList();
        }

        public tblAccountControl GetById(int id)
        {
            return _dbContext.tblAccountControl.FirstOrDefault(ac => ac.AccountControlID == id);
        }

        public void Add(tblAccountControl accountControl)
        {
            _dbContext.tblAccountControl.Add(accountControl);
            _dbContext.SaveChanges();
        }

        public void Update(tblAccountControl accountControl)
        {
            var entity = _dbContext.tblAccountControl.Find(accountControl.AccountControlID);
            if (entity == null) throw new KeyNotFoundException("AccountControl not found.");

            entity.AccountControlName = accountControl.AccountControlName;
            entity.AccountHeadID = accountControl.AccountHeadID;
            entity.BranchID = accountControl.BranchID;
            entity.CompanyID = accountControl.CompanyID;
            entity.UserID = accountControl.UserID;

            _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
