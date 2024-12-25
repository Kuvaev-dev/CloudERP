using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class FinancialYearRepository : IFinancialYearRepository
    {
        private readonly CloudDBEntities _dbContext;

        public FinancialYearRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FinancialYear>> GetAllAsync()
        {
            try
            {
                var entities = await _dbContext.tblFinancialYear
                    .Include(u => u.tblUser)
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving financial years.", ex);
            }
        }

        public async Task<IEnumerable<FinancialYear>> GetAllActiveAsync()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving active financial years.", ex);
            }
        }

        public async Task<FinancialYear> GetSingleActiveAsync()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving active financial year.", ex);
            }
        }

        public async Task<FinancialYear> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving financial year.", ex);
            }
        }

        public async Task AddAsync(FinancialYear financialYear)
        {
            try
            {
                if (financialYear == null) throw new ArgumentNullException(nameof(financialYear));

                var entity = new tblFinancialYear
                {
                    FinancialYearID = financialYear.FinancialYearID,
                    FinancialYear = financialYear.FinancialYearName,
                    StartDate = financialYear.StartDate,
                    EndDate = financialYear.EndDate,
                    IsActive = financialYear.IsActive,
                    UserID = financialYear.UserID
                };

                _dbContext.tblFinancialYear.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new financial year.", ex);
            }
        }

        public async Task UpdateAsync(FinancialYear financialYear)
        {
            try
            {
                if (financialYear == null) throw new ArgumentNullException(nameof(financialYear));

                var entity = await _dbContext.tblFinancialYear.FindAsync(financialYear.FinancialYearID);
                if (entity == null) throw new KeyNotFoundException("Financial year not found.");

                entity.FinancialYearID = financialYear.FinancialYearID;
                entity.FinancialYear = financialYear.FinancialYearName;
                entity.StartDate = financialYear.StartDate;
                entity.EndDate = financialYear.EndDate;
                entity.IsActive = financialYear.IsActive;
                entity.UserID = financialYear.UserID;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating financial year with ID {financialYear.FinancialYearID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
