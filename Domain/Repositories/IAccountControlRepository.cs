using DatabaseAccess;
using System.Collections.Generic;

namespace Domain.Repositories
{
    public interface IAccountControlRepository
    {
        IEnumerable<tblAccountControl> GetAccountControls(int companyId, int branchId);
        tblAccountControl GetById(int id);
        void Add(tblAccountControl accountControl);
        void Update(tblAccountControl accountControl);
    }
}
