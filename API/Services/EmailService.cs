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

        public async Task SendEmail(string toEmailAddress, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var enableSsl = bool.Parse(smtpSection["EnableSsl"]);
            var user = smtpSection["User"];
            var password = smtpSection["Password"];
            var from = smtpSection["From"];

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = enableSsl
            };

            var message = new MailMessage(from, toEmailAddress, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}