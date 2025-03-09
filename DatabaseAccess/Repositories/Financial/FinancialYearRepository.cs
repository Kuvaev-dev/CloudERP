using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Financial
{
    public class FinancialYearRepository : IFinancialYearRepository
    {
        private readonly CloudDBEntities _dbContext;

        public FinancialYearRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<FinancialYear>> GetAllAsync()
        {
            var entities = await _dbContext.tblFinancialYear
                .Include(u => u.User)
                .ToListAsync();

            return entities.Select(f => new FinancialYear
            {
                FinancialYearID = f.FinancialYearID,
                FinancialYearName = f.FinancialYear,
                StartDate = f.StartDate,
                EndDate = f.EndDate,
                IsActive = f.IsActive,
                UserID = f.UserID,
                UserName = f.User.UserName
            });
        }

        public async Task<IEnumerable<FinancialYear>> GetAllActiveAsync()
        {
            var entities = await _dbContext.tblFinancialYear
                .Where(f => f.IsActive)
                .ToListAsync();

            return entities.Select(f => new FinancialYear
            {
                FinancialYearID = f.FinancialYearID,
                FinancialYearName = f.FinancialYear,
                StartDate = f.StartDate,
                EndDate = f.EndDate,
                IsActive = f.IsActive,
                UserID = f.UserID
            });
        }

        public async Task<FinancialYear?> GetSingleActiveAsync()
        {
            var entity = await _dbContext.tblFinancialYear.FirstOrDefaultAsync(f => f.IsActive);

            return entity == null ? null : new FinancialYear
            {
                FinancialYearID = entity.FinancialYearID,
                FinancialYearName = entity.FinancialYear,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                UserID = entity.UserID
            };
        }

        public async Task<FinancialYear?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblFinancialYear.FindAsync(id);

            return entity == null ? null : new FinancialYear
            {
                FinancialYearID = entity.FinancialYearID,
                FinancialYearName = entity.FinancialYear,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(FinancialYear financialYear)
        {
            var entity = new tblFinancialYear
            {
                FinancialYear = financialYear.FinancialYearName,
                StartDate = financialYear.StartDate,
                EndDate = financialYear.EndDate,
                IsActive = financialYear.IsActive,
                UserID = financialYear.UserID
            };

            _dbContext.tblFinancialYear.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(FinancialYear financialYear)
        {
            var entity = await _dbContext.tblFinancialYear.FindAsync(financialYear.FinancialYearID);

            entity.FinancialYear = financialYear.FinancialYearName;
            entity.StartDate = financialYear.StartDate;
            entity.EndDate = financialYear.EndDate;
            entity.IsActive = financialYear.IsActive;
            entity.UserID = financialYear.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
