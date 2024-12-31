using DatabaseAccess.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupplierRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var entities = await _dbContext.tblSupplier.ToListAsync();

            return entities.Select(s => new Supplier
            {
                SupplierID = s.SupplierID,
                SupplierName = s.SupplierName,
                SupplierAddress = s.SupplierName,
                SupplierConatctNo = s.SupplierConatctNo,
                SupplierEmail = s.SupplierEmail,
                Discription = s.Discription,
                CompanyID = s.CompanyID,
                BranchID = s.BranchID,
                UserID = s.UserID
            });
        }

        public async Task<IEnumerable<Supplier>> GetByCompanyAndBranchAsync(int companyID, int branchID)
        {
            var entities = await _dbContext.tblSupplier
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                .ToListAsync();

            return entities.Select(s => new Supplier
            {
                SupplierID = s.SupplierID,
                SupplierName = s.SupplierName,
                SupplierAddress = s.SupplierName,
                SupplierConatctNo = s.SupplierConatctNo,
                SupplierEmail = s.SupplierEmail,
                Discription = s.Discription,
                CompanyID = s.CompanyID,
                BranchID = s.BranchID,
                UserID = s.UserID
            });
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblSupplier.FindAsync(id);

            return entity == null ? null : new Supplier
            {
                SupplierID = entity.SupplierID,
                SupplierName = entity.SupplierName,
                SupplierAddress = entity.SupplierName,
                SupplierConatctNo = entity.SupplierConatctNo,
                SupplierEmail = entity.SupplierEmail,
                Discription = entity.Discription,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<Supplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo)
        {
            var entity = await _dbContext.tblSupplier
                .Where(s => s.CompanyID == companyID 
                    && s.BranchID == branchID 
                    && s.SupplierName == supplierName 
                    && s.SupplierConatctNo == contactNo)
                .FirstOrDefaultAsync();

            return entity == null ? null : new Supplier
            {
                SupplierID = entity.SupplierID,
                SupplierName = entity.SupplierName,
                SupplierAddress = entity.SupplierName,
                SupplierConatctNo = entity.SupplierConatctNo,
                SupplierEmail = entity.SupplierEmail,
                Discription = entity.Discription,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByBranchesAsync(int branchID)
        {
            List<int> branchIDs = BranchHelper.GetBranchsIDs(branchID, _dbContext);

            var entities = await _dbContext.tblSupplier
                .Where(s => branchIDs.Contains(s.BranchID))
                .ToListAsync();

            return entities.Select(s => new Supplier
            {
                SupplierID = s.SupplierID,
                SupplierName = s.SupplierName,
                SupplierAddress = s.SupplierName,
                SupplierConatctNo = s.SupplierConatctNo,
                SupplierEmail = s.SupplierEmail,
                Discription = s.Discription,
                CompanyID = s.CompanyID,
                BranchID = s.BranchID,
                UserID = s.UserID
            });
        }

        public async Task AddAsync(Supplier supplier)
        {
            var entity = new tblSupplier
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                SupplierConatctNo = supplier.SupplierConatctNo,
                SupplierAddress = supplier.SupplierAddress,
                SupplierEmail = supplier.SupplierEmail,
                Discription = supplier.Discription,
                BranchID = supplier.BranchID,
                CompanyID = supplier.CompanyID,
                UserID = supplier.UserID
            };

            _dbContext.tblSupplier.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            var entity = await _dbContext.tblSupplier.FindAsync(supplier.SupplierID);

            entity.SupplierID = supplier.SupplierID;
            entity.SupplierName = supplier.SupplierName;
            entity.SupplierAddress = supplier.SupplierName;
            entity.SupplierConatctNo = supplier.SupplierConatctNo;
            entity.SupplierEmail = supplier.SupplierEmail;
            entity.Discription = supplier.Discription;
            entity.CompanyID = supplier.CompanyID;
            entity.BranchID = supplier.BranchID;
            entity.UserID = supplier.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
