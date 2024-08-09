using DatabaseAccess;
using System;
using System.Linq;

namespace CloudERP.Helpers
{
    public class ReminderService
    {
        private readonly CloudDBEntities _db;
        private readonly EmailService _emailService;

        public ReminderService(CloudDBEntities db)
        {
            _db = db;
            _emailService = new EmailService();
        }

        public void SendReminders()
        {
            var tasksToRemind = _db.tblTask
                .Where(t => t.ReminderDate.HasValue && !t.IsCompleted && t.ReminderDate.Value <= DateTime.Now)
                .ToList();

            foreach (var task in tasksToRemind)
            {
                try
                {
                    var user = _db.tblUser.Find(task.UserID);

                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        SendEmailReminder(task, user.Email);

                        Console.WriteLine($"Reminder sent for task '{task.Title}' to {user.Email}");

                        _db.Entry(task).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send reminder for task '{task.Title}' because email is not available");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send reminder for task '{task.Title}': {ex.Message}");
                }
            }
        }

        private void SendEmailReminder(tblTask task, string email)
        {
            string subject = $"Reminder: {task.Title}";
            string body = BuildEmailBody(task);
            string toEmail = email;

            _emailService.SendEmail(toEmail, subject, body);
        }

        private string BuildEmailBody(tblTask task)
        {
            return $@"
            <h1>Reminder for Task: {task.Title}</h1>
            <p>{task.Description}</p>
            <p>Due Date: {task.DueDate:g}</p>
            <p>Please make sure to complete this task on time.</p>";
        }
    }
}