using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ICustomerRepository
    {
        CloudDBEntities DbContext { get; }
        Task<IEnumerable<tblCustomer>> GetAllAsync();
        Task<IEnumerable<tblCustomer>> GetByCompanyAndBranchAsync(int companyId, int branchId);
        Task<IEnumerable<tblCustomer>> GetByBranchesAsync(IEnumerable<int> branchIds);
        Task<tblCustomer> GetByIdAsync(int id);
        Task AddAsync(tblCustomer customer);
        Task UpdateAsync(tblCustomer customer);
        Task DeleteAsync(int id);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public CloudDBEntities DbContext => _dbContext;

        public async Task<IEnumerable<tblCustomer>> GetAllAsync()
        {
            return await _dbContext.tblCustomer
                .Include(c => c.tblBranch)
                .Include(c => c.tblCompany)
                .Include(c => c.tblUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<tblCustomer>> GetByCompanyAndBranchAsync(int companyId, int branchId)
        {
            return await _dbContext.tblCustomer
                .Where(c => c.CompanyID == companyId && c.BranchID == branchId)
                .Include(c => c.tblBranch)
                .Include(c => c.tblCompany)
                .Include(c => c.tblUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<tblCustomer>> GetByBranchesAsync(IEnumerable<int> branchIds)
        {
            return await _dbContext.tblCustomer
                .Where(c => branchIds.Contains(c.BranchID))
                .Include(c => c.tblBranch)
                .Include(c => c.tblCompany)
                .Include(c => c.tblUser)
                .ToListAsync();
        }

        public async Task<tblCustomer> GetByIdAsync(int id)
        {
            return await _dbContext.tblCustomer
                .Include(c => c.tblBranch)
                .Include(c => c.tblCompany)
                .Include(c => c.tblUser)
                .FirstOrDefaultAsync(c => c.CustomerID == id);
        }

        public async Task AddAsync(tblCustomer customer)
        {
            _dbContext.tblCustomer.Add(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblCustomer customer)
        {
            _dbContext.Entry(customer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _dbContext.tblCustomer.FindAsync(id);
            if (customer != null)
            {
                _dbContext.tblCustomer.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
