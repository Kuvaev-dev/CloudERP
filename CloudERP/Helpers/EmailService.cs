using MailKit.Net.Smtp;
using MimeKit;
using System.Configuration;

namespace CloudERP.Helpers
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string toEmailAddress, string subject, string body)
        {
            var smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            var smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            var fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Cloud ERP", fromEmail));
            message.To.Add(new MailboxAddress("", toEmailAddress));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                // Connect to the SMTP server
                client.Connect(smtpServer, smtpPort, false);
                client.Authenticate(smtpUsername, smtpPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}