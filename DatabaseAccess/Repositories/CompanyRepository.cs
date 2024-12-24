using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CompanyRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            try
            {
                var entities = await _dbContext.tblCompany
                .AsNoTracking()
                .ToListAsync();

                return entities.Select(c => new Company
                {
                    CompanyID = c.CompanyID,
                    Name = c.Name,
                    Logo = c.Logo,
                    Description = c.Description
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving companies.", ex);
            }
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblCompany.FindAsync(id);

                return entity == null ? null : new Company
                {
                    CompanyID = entity.CompanyID,
                    Name = entity.Name,
                    Logo = entity.Logo,
                    Description = entity.Description
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving company with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Company company)
        {
            try
            {
                if (company == null) throw new ArgumentNullException(nameof(company));

                var entity = new tblCompany
                {
                    CompanyID = company.CompanyID,
                    Name = company.Name,
                    Logo = company.Logo,
                    Description = company.Description
                };

                _dbContext.tblCompany.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new company.", ex);
            }
        }

        public async Task UpdateAsync(Company company)
        {
            try
            {
                if (company == null) throw new ArgumentNullException(nameof(company));

                var entity = await _dbContext.tblCompany.FindAsync(company.CompanyID);
                if (entity == null) throw new KeyNotFoundException("Company not found.");

                entity.CompanyID = company.CompanyID;
                entity.Name = company.Name;
                entity.Logo = company.Logo;
                entity.Description = company.Description;

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
                throw new InvalidOperationException($"Error updating company with ID {company.CompanyID}.", ex);
            }
            _dbContext.Entry(company).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
