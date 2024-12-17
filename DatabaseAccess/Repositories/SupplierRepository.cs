using DatabaseAccess.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ISupplierRepository
    {
        CloudDBEntities DbContext { get; }
        Task<IEnumerable<tblSupplier>> GetAllAsync();
        Task<IEnumerable<tblSupplier>> GetByCompanyAndBranchAsync(int companyID, int branchID);
        Task<tblSupplier> GetByIdAsync(int id);
        Task<tblSupplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo);
        Task<IEnumerable<tblSupplier>> GetSuppliersByBranchesAsync(int branchID, CloudDBEntities dbContext);
        Task AddAsync(tblSupplier supplier);
        Task UpdateAsync(tblSupplier supplier);
        Task DeleteAsync(int id);
    }

    public class SupplierRepository : ISupplierRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public CloudDBEntities DbContext => _dbContext;

        public async Task<IEnumerable<tblSupplier>> GetAllAsync()
        {
            return await _dbContext.tblSupplier.ToListAsync();
        }

        public async Task<IEnumerable<tblSupplier>> GetByCompanyAndBranchAsync(int companyID, int branchID)
        {
            return await _dbContext.tblSupplier
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                .ToListAsync();
        }

        public async Task<tblSupplier> GetByIdAsync(int id)
        {
            return await _dbContext.tblSupplier.FindAsync(id);
        }

        public async Task<tblSupplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo)
        {
            return await _dbContext.tblSupplier
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID && s.SupplierName == supplierName && s.SupplierConatctNo == contactNo)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<tblSupplier>> GetSuppliersByBranchesAsync(int branchID, CloudDBEntities dbContext)
        {
            List<int> branchIDs = BranchHelper.GetBranchsIDs(branchID, dbContext);

            return await dbContext.tblSupplier
                .Where(s => branchIDs.Contains(s.BranchID))
                .ToListAsync();
        }

        public async Task AddAsync(tblSupplier supplier)
        {
            _dbContext.tblSupplier.Add(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblSupplier supplier)
        {
            var existing = await _dbContext.tblSupplier.FindAsync(supplier.SupplierID);
            if (existing == null)
                throw new KeyNotFoundException("Supplier not found.");

            _dbContext.Entry(existing).CurrentValues.SetValues(supplier);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _dbContext.tblSupplier.FindAsync(id);
            if (supplier != null)
            {
                _dbContext.tblSupplier.Remove(supplier);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
