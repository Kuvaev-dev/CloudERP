using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;\
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class PurchaseCartDetailRepository : IPurchaseCartDetailRepository
    {
        private readonly CloudDBEntities _db;

        public PurchaseCartDetailRepository(CloudDBEntities db)
        {
            _db = db;
        }

        public async Task AddAsync(tblPurchaseCartDetail tblPurchaseCartDetail)
        {
            _db.tblPurchaseCartDetail.Add(tblPurchaseCartDetail);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(tblPurchaseCartDetail tblPurchaseCartDetail)
        {
            _db.Entry(tblPurchaseCartDetail).State = EntityState.Deleted;
            await _db.SaveChangesAsync();
        }

        public async Task<List<tblPurchaseCartDetail>> GetByBranchAndCompanyAsync(int branchId, int companyId)
        {
            return await _db.tblPurchaseCartDetail.Where(pd => pd.BranchID == branchId && pd.CompanyID == companyId).ToListAsync();
        }

        public async Task<List<tblPurchaseCartDetail>> GetByDefaultSettingsAsync(int branchId, int companyId, int userId)
        {
            return await _db.tblPurchaseCartDetail.Where(i => i.BranchID == branchId && i.CompanyID == companyId && i.UserID == userId).ToListAsync();
        }

        public async Task<tblPurchaseCartDetail> GetByIdAsync(int PCID)
        {
            return await _db.tblPurchaseCartDetail.FindAsync(PCID);
        }

        public async Task<tblPurchaseCartDetail> GetByProductIdAsync(int branchId, int companyId, int productId)
        {
            return await _db.tblPurchaseCartDetail.FirstOrDefaultAsync(i => i.ProductID == productId && i.BranchID == branchId && i.CompanyID == companyId);
        }

        public async Task<bool> IsCanceled(int branchId, int companyId, int userId)
        {
            bool cancelstatus = false;

            foreach (var item in await GetByDefaultSettingsAsync(branchId, companyId, userId))
            {
                _db.Entry(item).State = EntityState.Deleted;
                int noofrecords = _db.SaveChanges();
                if (cancelstatus == false && noofrecords > 0)
                {
                    cancelstatus = true;
                }
            }

            return cancelstatus;
        }

        public async Task UpdateAsync(tblPurchaseCartDetail tblPurchaseCartDetail)
        {
            _db.tblPurchaseCartDetail.Add(tblPurchaseCartDetail);
            await _db.SaveChangesAsync();
        }
    }
}
