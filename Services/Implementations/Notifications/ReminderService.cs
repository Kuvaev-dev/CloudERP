using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class ReminderService : IReminderService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IEmailService _emailService;

        public ReminderService(
            IEmailService emailService, 
            IUserRepository userRepository, 
            ITaskRepository taskRepository)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public async Task SendReminders()
        {
            var tasksToRemind = await _taskRepository.GetTasksToRemind();

            foreach (var task in tasksToRemind)
            {
                var user = await _userRepository.GetByIdAsync(task.UserID);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    SendEmailReminder(task, user.Email);

                    await _taskRepository.UpdateAsync(task);
                }
            }
        }

        private void SendEmailReminder(TaskModel task, string email)
        {
            string subject = $"{Localization.Services.Localization.ReminderFor}: {task.Title}";
            string body = BuildEmailBody(task);
            string toEmail = email;

            _emailService.SendEmail(toEmail, subject, body);
        }

        private static string BuildEmailBody(TaskModel task)
        {
            return $@"
            <h1>{Localization.Services.Localization.ReminderForTask}: {task.Title}</h1>
            <p>{task.Description}</p>
            <p>{Localization.Services.Localization.DueDate}: {task.DueDate:g}</p>
            <p>{Localization.Services.Localization.MakeSureToCompleteTask}</p>";
        }
    }
}