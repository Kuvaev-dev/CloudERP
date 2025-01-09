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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
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

        public async Task<Company> GetByIdAsync(int id)
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

        public async Task AddAsync(Company company)
        {
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

        public async Task UpdateAsync(Company company)
        {
            var entity = await _dbContext.tblCompany.FindAsync(company.CompanyID);

            entity.CompanyID = company.CompanyID;
            entity.Name = company.Name;
            entity.Logo = company.Logo;
            entity.Description = company.Description;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Company> GetByNameAsync(string name)
        {
            var entity = await _dbContext.tblCompany.FindAsync(name);

            return entity == null ? null : new Company
            {
                CompanyID = entity.CompanyID,
                Name = entity.Name,
                Logo = entity.Logo,
                Description = entity.Description
            };
        }
    }
}
