namespace Domain.ServiceAccess
{
    public interface IEmailService
    {
        Task SendEmail(string toEmailAddress, string subject, string body);
    }
}