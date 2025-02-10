using System.Threading.Tasks;

namespace Domain.ServiceAccess
{
    public interface IReminderService
    {
        Task SendReminders();
    }
}
