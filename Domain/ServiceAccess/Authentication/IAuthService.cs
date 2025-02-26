using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IAuthService
    {
        Task<User> AuthenticateUserAsync(string email, string password);
        Task<bool> IsPasswordResetRequestedRecentlyAsync(string email);
        void SendPasswordResetEmailAsync(string resetLink, string email, string resetPasswordCode);
        Task<bool> ResetPasswordAsync(string id, string newPassword, string confirmPassword);
    }
}
