using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Resources;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateUserAsync(string email, string password);
        Task<bool> IsPasswordResetRequestedRecentlyAsync(string email);
        void SendPasswordResetEmailAsync(string resetLink, string email, string resetPasswordCode);
        Task<bool> ResetPasswordAsync(string id, string newPassword, string confirmPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly PasswordHelper _passwordHelper;

        public AuthService(IUserRepository userRepository, PasswordHelper passwordHelper, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _emailService = emailService;
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && _passwordHelper.VerifyPassword(password, user.Password, user.Salt))
            {
                return user;
            }

            return null;
        }

        public async Task<bool> IsPasswordResetRequestedRecentlyAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && (DateTime.Now - user.LastPasswordResetRequest)?.TotalMinutes < 5)
            {
                return true;
            }

            return false;
        }

        public void SendPasswordResetEmailAsync(string resetLink, string email, string resetPasswordCode)
        {
            var subject = "Password Reset";
            var body = $"<strong>Please reset your password by clicking the following link: <a href='{resetLink}'>Reset Password</a></strong>";

            _emailService.SendEmail(email, subject, body);
        }

        public async Task<bool> ResetPasswordAsync(string id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                return false;
            }

            var user = await _userRepository.GetByPasswordCodesAsync(id, DateTime.Now);
            if (user != null)
            {
                user.Password = _passwordHelper.HashPassword(newPassword, out string salt);
                user.Salt = salt;
                user.ResetPasswordCode = null;
                user.ResetPasswordExpiration = null;
                user.LastPasswordResetRequest = null;

                await _userRepository.UpdateAsync(user);
                return true;
            }

            return false;
        }
    }
}
