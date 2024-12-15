using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;

namespace Domain.Mapping
{
    public class TaskMapper : IMapper<TaskModel, tblTask>
    {
        public TaskModel MapToDomain(tblTask dbModel)
        {
            return new TaskModel
            {
                TaskID = dbModel.TaskID,
                Title = dbModel.Title,
                Description = dbModel.Description,
                DueDate = dbModel.DueDate,
                ReminderDate = dbModel.ReminderDate,
                IsCompleted = dbModel.IsCompleted,
                CompanyID = dbModel.CompanyID,
                BranchID = dbModel.BranchID,
                UserID = dbModel.UserID
            };
        }

        public tblTask MapToDatabase(TaskModel domainModel)
        {
            return new tblTask
            {
                TaskID = domainModel.TaskID,
                Title = domainModel.Title,
                Description = domainModel.Description,
                DueDate = domainModel.DueDate,
                ReminderDate = domainModel.ReminderDate,
                IsCompleted = domainModel.IsCompleted,
                CompanyID = domainModel.CompanyID,
                BranchID = domainModel.BranchID,
                UserID = domainModel.UserID
            };
        }
    }
}
