namespace Domain.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string toEmailAddress, string subject, string body);
    }
}