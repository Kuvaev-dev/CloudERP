using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;

namespace CloudERP.Mapping
{
    public class TaskMapper : BaseMapper<TaskModel, TaskMV>
    {
        public override TaskModel MapToDomain(TaskMV viewModel)
        {
            return new TaskModel
            {
                TaskID = viewModel.TaskID,
                Title = viewModel.Title,
                Description = viewModel.Description,
                DueDate = viewModel.DueDate,
                ReminderDate = viewModel.ReminderDate,
                IsCompleted = viewModel.IsCompleted,
                BranchID = viewModel.BranchID,
                CompanyID = viewModel.CompanyID,
                UserID = viewModel.UserID
            };
        }

        public override TaskMV MapToViewModel(TaskModel domainModel)
        {
            return new TaskMV
            {
                TaskID = domainModel.TaskID,
                Title = domainModel.Title,
                Description = domainModel.Description,
                DueDate = domainModel.DueDate,
                ReminderDate = domainModel.ReminderDate,
                IsCompleted = domainModel.IsCompleted
            };
        }
    }
}