using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly CloudDBEntities _dbContext;

        public TaskRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync(int companyId, int branchId, int userId)
        {
            var entities = await _dbContext.tblTask
                .Where(t => t.CompanyID == companyId && t.BranchID == branchId && t.UserID == userId)
                .ToListAsync();

            return entities.Select(t => new TaskModel
            {
                TaskID = t.TaskID,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                ReminderDate = t.ReminderDate,
                IsCompleted = t.IsCompleted,
                CompanyID = t.CompanyID,
                BranchID = t.BranchID,
                UserID = t.UserID
            });
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblTask.FindAsync(id);

            return entity == null ? null : new TaskModel
            {
                TaskID = entity.TaskID,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                ReminderDate = entity.ReminderDate,
                IsCompleted = entity.IsCompleted,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(TaskModel task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var entity = new tblTask
            {
                TaskID = task.TaskID,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                ReminderDate = task.ReminderDate,
                IsCompleted = task.IsCompleted,
                CompanyID = task.CompanyID,
                BranchID = task.BranchID,
                UserID = task.UserID
            };

            _dbContext.tblTask.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskModel task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var entity = await _dbContext.tblTask.FindAsync(task.TaskID);
            if (entity == null) throw new KeyNotFoundException("Task not found.");

            entity.TaskID = task.TaskID;
            entity.Title = task.Title;
            entity.Description = task.Description;
            entity.DueDate = task.DueDate;
            entity.ReminderDate = task.ReminderDate;
            entity.IsCompleted = task.IsCompleted;
            entity.CompanyID = task.CompanyID;
            entity.BranchID = task.BranchID;
            entity.UserID = task.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
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

        public async Task<IEnumerable<TaskModel>> GetTasksToRemind()
        {
            var entities = await _dbContext.tblTask
                .Where(t => t.ReminderDate.HasValue && !t.IsCompleted && t.ReminderDate.Value <= DateTime.Now)
                .ToListAsync();

            return entities.Select(t => new TaskModel
            {
                TaskID = t.TaskID,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                ReminderDate = t.ReminderDate,
                IsCompleted = t.IsCompleted,
                CompanyID = t.CompanyID,
                BranchID = t.BranchID,
                UserID = t.UserID
            });
        }
    }
}
