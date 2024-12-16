using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<tblCompany>> GetAllAsync();
        Task<tblCompany> GetByIdAsync(int id);
        Task AddAsync(tblCompany company);
        Task UpdateAsync(tblCompany company);
    }

    public class CompanyRepository : ICompanyRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CompanyRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblCompany>> GetAllAsync()
        {
            return await _dbContext.tblCompany.ToListAsync();
        }

        public async Task<tblCompany> GetByIdAsync(int id)
        {
            return await _dbContext.tblCompany.FindAsync(id);
        }

        public async Task AddAsync(tblCompany company)
        {
            _dbContext.tblCompany.Add(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblCompany company)
        {
            _dbContext.Entry(company).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
