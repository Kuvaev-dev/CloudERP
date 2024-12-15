using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<Models.TaskModel>> GetAllAsync(int companyId, int branchId, int userId);
        Task<Models.TaskModel> GetByIdAsync(int id);
        Task CreateAsync(Models.TaskModel task);
        Task UpdateAsync(Models.TaskModel task);
        Task DeleteAsync(int id);
    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IMapper<Models.TaskModel, tblTask> _mapper;

        public TaskService(ITaskRepository repository, IMapper<Models.TaskModel, tblTask> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Models.TaskModel>> GetAllAsync(int companyId, int branchId, int userId)
        {
            var tasks = await _repository.GetAllAsync(companyId, branchId, userId);
            return tasks.Select(_mapper.MapToDomain);
        }

        public async Task<Models.TaskModel> GetByIdAsync(int id)
        {
            var task = await _repository.GetByIdAsync(id);
            return task == null ? null : _mapper.MapToDomain(task);
        }

        public async Task CreateAsync(Models.TaskModel task)
        {
            var dbModel = _mapper.MapToDatabase(task);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Models.TaskModel task)
        {
            var dbModel = _mapper.MapToDatabase(task);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
