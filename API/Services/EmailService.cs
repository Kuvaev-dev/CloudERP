using Domain.ServiceAccess;
using System.Net;
using System.Net.Mail;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmailAddress, string subject, string body)
        {
            var smtpServer = _configuration["Smtp:Server"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUsername = _configuration["Smtp:Username"];
            var smtpPassword = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Cloud ERP"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmailAddress));

            client.Send(message);
        }
    }
}