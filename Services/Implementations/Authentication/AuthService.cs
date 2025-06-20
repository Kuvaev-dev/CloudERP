using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Domain.UtilsAccess;

namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordHelper _passwordHelper;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHelper passwordHelper, 
            IEmailService emailService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(IPasswordHelper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(IEmailService));
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return _passwordHelper.VerifyPassword(password, user.Password, user.Salt) ? user : throw new UnauthorizedAccessException("Invalid email or password.");
        }

        public async Task<bool> IsPasswordResetRequestedRecentlyAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null && (DateTime.Now - user.LastPasswordResetRequest)?.TotalMinutes < 5;
        }

        public async Task SendPasswordResetEmailAsync(string resetLink, string email, string resetPasswordCode)
        {
            var subject = "Cloud ERP - Password Reset";
            var body = $"<strong>Please reset your password by clicking the following link: <a href='{resetLink}'>Reset Password</a></strong>";

            await _emailService.SendEmail(email, subject, body);
        }

        public async Task<bool> ResetPasswordAsync(string id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword) return false;

            var user = await _userRepository.GetByPasswordCodesAsync(id, DateTime.Now);
            if (user == null) return false;

            user.Password = _passwordHelper.HashPassword(newPassword, out string salt);
            user.Salt = salt;
            user.ResetPasswordCode = null;
            user.ResetPasswordExpiration = null;
            user.LastPasswordResetRequest = null;

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
