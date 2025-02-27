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
            _emailService = emailService ?? throw new ArgumentNullException(nameof(IEmailService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(ITaskRepository));
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
            string subject = $"Reminder for: {task.Title}";
            string body = BuildEmailBody(task);
            string toEmail = email;

            _emailService.SendEmail(toEmail, subject, body);
        }

        private string BuildEmailBody(TaskModel task)
        {
            return $@"
            <h1>Reminder for Task: {task.Title}</h1>
            <p>{task.Description}</p>
            <p>Due Date: {task.DueDate:g}</p>
            <p>Make sure to complete task.</p>";
        }
    }
}