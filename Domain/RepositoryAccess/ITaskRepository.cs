using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetAllAsync(int companyId, int branchId, int userId);
        Task<IEnumerable<TaskModel>> GetTasksToRemind();
        Task<TaskModel> GetByIdAsync(int id);
        Task AddAsync(TaskModel task);
        Task UpdateAsync(TaskModel task);
        Task DeleteAsync(int id);
    }
}
