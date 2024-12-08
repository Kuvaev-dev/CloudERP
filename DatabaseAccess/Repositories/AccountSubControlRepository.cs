using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface IAccountSubControlRepository
    {
        IEnumerable<tblAccountSubControl> GetAll(int companyId, int branchId);
        tblAccountSubControl GetById(int id);
        void Add(tblAccountSubControl entity);
        void Update(tblAccountSubControl entity);
    }

    public class AccountSubControlRepository : IAccountSubControlRepository
    {
        private readonly CloudDBEntities _db;

        public AccountSubControlRepository(CloudDBEntities db)
        {
            _db = db;
        }

        public IEnumerable<tblAccountSubControl> GetAll(int companyId, int branchId)
        {
            return _db.tblAccountSubControl
                .Include("tblAccountControl")
                .Include("tblAccountHead")
                .Include("tblUser")
                .Where(x => x.CompanyID == companyId && x.BranchID == branchId)
                .ToList();
        }

        public tblAccountSubControl GetById(int id)
        {
            return _db.tblAccountSubControl.Find(id);
        }

        public void Add(tblAccountSubControl entity)
        {
            _db.tblAccountSubControl.Add(entity);
            _db.SaveChanges();
        }

        public void Update(tblAccountSubControl entity)
        {
            _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
