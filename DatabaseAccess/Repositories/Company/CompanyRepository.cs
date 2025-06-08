using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Company
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CompanyRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Domain.Models.Company>> GetAllAsync()
        {
            var entities = await _dbContext.tblCompany
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(c => new Domain.Models.Company
            {
                CompanyID = c.CompanyID,
                Name = c.Name,
                Logo = c.Logo,
                Description = c.Description
            });
        }

        public async Task<Domain.Models.Company?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblCompany.FindAsync(id);

            return entity == null ? null : new Domain.Models.Company
            {
                CompanyID = entity.CompanyID,
                Name = entity.Name,
                Logo = entity.Logo,
                Description = entity.Description
            };
        }

        public async Task AddAsync(Domain.Models.Company company)
        {
            var entity = new tblCompany
            {
                Name = company.Name,
                Logo = company.Logo,
                Description = company.Description
            };

            _dbContext.tblCompany.Add(entity);
            await _dbContext.SaveChangesAsync();
            company.CompanyID = entity.CompanyID;
        }

        public async Task UpdateAsync(Domain.Models.Company company)
        {
            var entity = await _dbContext.tblCompany.FindAsync(company.CompanyID);

            entity.CompanyID = company.CompanyID;
            entity.Name = company.Name;
            entity.Logo = company.Logo;
            entity.Description = company.Description;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Domain.Models.Company?> GetByNameAsync(string name)
        {
            var entity = await _dbContext.tblCompany.FirstOrDefaultAsync(c => c.Name == name);

            return entity == null ? null : new Domain.Models.Company
            {
                CompanyID = entity.CompanyID,
                Name = entity.Name,
                Logo = entity.Logo,
                Description = entity.Description
            };
        }

        public async Task<bool> CheckCompanyExistsAsync(string name)
        {
            var companies = await GetAllAsync();
            return companies.Any(c => c.Name == name);
        }

        public async Task<bool> IsExists(Domain.Models.Company company)
        {
            return await _dbContext.tblCompany
                .AnyAsync(c => c.Name == company.Name && c.CompanyID != company.CompanyID);
        }
    }
}
