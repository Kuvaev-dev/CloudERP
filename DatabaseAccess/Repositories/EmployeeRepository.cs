using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<tblEmployee>> GetByBranchAsync(int companyId, int branchId);
        Task<tblEmployee> GetByIdAsync(int id);
        Task<tblEmployee> GetByUserIdAsync(int id);
        Task AddAsync(tblEmployee employee);
        Task UpdateAsync(tblEmployee employee);
        Task<bool> IsFirstLoginAsync(tblEmployee employee);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public EmployeeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblEmployee>> GetByBranchAsync(int companyId, int branchId)
        {
            return await _dbContext.tblEmployee
                .Where(e => e.CompanyID == companyId && e.BranchID == branchId)
                .ToListAsync();
        }

        public async Task<tblEmployee> GetByIdAsync(int id)
        {
            return await _dbContext.tblEmployee.FindAsync(id);
        }

        public async Task AddAsync(tblEmployee employee)
        {
            _dbContext.tblEmployee.Add(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblEmployee employee)
        {
            _dbContext.Entry(employee).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<tblEmployee> GetByUserIdAsync(int id)
        {
            return await _dbContext.tblEmployee
                .Include(b => b.tblBranch)
                .Where(e => e.UserID == id).FirstOrDefaultAsync();
        }

        public async Task<bool> IsFirstLoginAsync(tblEmployee employee)
        {
            var employeeRecord = await _dbContext.tblEmployee.FirstOrDefaultAsync(e => e.EmployeeID == employee.EmployeeID);

            if (employeeRecord.IsFirstLogin == true)
            {
                employeeRecord.IsFirstLogin = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
