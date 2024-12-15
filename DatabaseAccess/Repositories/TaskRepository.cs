using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<tblTask>> GetAllAsync(int companyId, int branchId, int userId);
        Task<tblTask> GetByIdAsync(int id);
        Task AddAsync(tblTask task);
        Task UpdateAsync(tblTask task);
        Task DeleteAsync(int id);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly CloudDBEntities _dbContext;

        public TaskRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblTask>> GetAllAsync(int companyId, int branchId, int userId)
        {
            return await _dbContext.tblTask
                .Where(t => t.CompanyID == companyId && t.BranchID == branchId && t.UserID == userId)
                .ToListAsync();
        }

        public async Task<tblTask> GetByIdAsync(int id)
        {
            return await _dbContext.tblTask.FindAsync(id);
        }

        public async Task AddAsync(tblTask task)
        {
            _dbContext.tblTask.Add(task);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblTask task)
        {
            var existingTask = await _dbContext.tblTask.FindAsync(task.TaskID);
            if (existingTask != null)
            {
                _dbContext.Entry(existingTask).CurrentValues.SetValues(task);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _dbContext.tblTask.FindAsync(id);
            if (task != null)
            {
                _dbContext.tblTask.Remove(task);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
