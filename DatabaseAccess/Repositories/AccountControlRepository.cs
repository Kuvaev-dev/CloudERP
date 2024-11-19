using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IAccountControlRepository
    {
        IEnumerable<tblAccountControl> GetAccountControls(int companyId, int branchId);
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

        public IEnumerable<tblAccountControl> GetAccountControls(int companyId, int branchId)
        {
            return _dbContext.tblAccountControl
                .Include(ac => ac.tblBranch)
                .Include(ac => ac.tblCompany)
                .Include(ac => ac.tblUser)
                .Where(ac => ac.CompanyID == companyId && ac.BranchID == branchId)
                .ToList();
        }

        public tblAccountControl GetById(int id)
        {
            return _dbContext.tblAccountControl.Find(id);
        }

        public void Add(tblAccountControl accountControl)
        {
            _dbContext.tblAccountControl.Add(accountControl);
            _dbContext.SaveChanges();
        }

        public void Update(tblAccountControl accountControl)
        {
            _dbContext.Entry(accountControl).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
