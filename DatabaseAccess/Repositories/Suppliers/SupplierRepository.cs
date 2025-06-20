﻿using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using Localization.CloudERP.Modules.Branch;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Suppliers
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly CloudDBEntities _dbContext;
        private readonly IBranchHelper _branchHelper;

        public SupplierRepository(
            CloudDBEntities dbContext,
            IBranchHelper branchHelper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _branchHelper = branchHelper ?? throw new ArgumentNullException(nameof(branchHelper));
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var entities = await _dbContext.tblSupplier
                .Include(s => s.Company)
                .Include(s => s.Branch)
                .Include(s => s.User)
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
                CompanyName = s.Company.Name,
                BranchID = s.BranchID,
                BranchName = s.Branch.BranchName,
                UserID = s.UserID,
                UserName = s.User.UserName
            });
        }

        public async Task<IEnumerable<Supplier>> GetByCompanyAndBranchAsync(int companyID, int branchID)
        {
            var entities = await _dbContext.tblSupplier
                .Include(s => s.Company)
                .Include(s => s.Branch)
                .Include(s => s.User)
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
                CompanyName = s.Company.Name,
                BranchID = s.BranchID,
                BranchName = s.Branch.BranchName,
                UserID = s.UserID,
                UserName = s.User.FullName
            });
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblSupplier
                .Include(s => s.Company)
                .Include(s => s.Branch)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SupplierID == id);

            return entity == null ? null : new Supplier
            {
                SupplierID = entity.SupplierID,
                SupplierName = entity.SupplierName,
                SupplierAddress = entity.SupplierName,
                SupplierConatctNo = entity.SupplierConatctNo,
                SupplierEmail = entity.SupplierEmail,
                Discription = entity.Discription,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                UserID = entity.UserID,
                UserName = entity.User.UserName
            };
        }

        public async Task<Supplier?> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo)
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
            var branchIds = _branchHelper.GetBranchsIDs(branchID);

            var entities = await _dbContext.tblSupplier
                .Where(c => branchIds.Contains(c.BranchID))
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
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
                CompanyName = s.Company.Name,
                BranchID = s.BranchID,
                BranchName = s.Branch.BranchName,
                UserID = s.UserID,
                UserName = s.User.UserName
            });
        }

        public async Task AddAsync(Supplier supplier)
        {
            var entity = new tblSupplier
            {
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

        public async Task<bool> IsExists(Supplier supplier)
        {
            return await _dbContext.tblSupplier
                .AnyAsync(s => s.SupplierName == supplier.SupplierName
                            && s.SupplierConatctNo == supplier.SupplierConatctNo
                            && s.CompanyID == supplier.CompanyID
                            && s.BranchID == supplier.BranchID
                            && s.SupplierID != supplier.SupplierID);
        }
    }
}
