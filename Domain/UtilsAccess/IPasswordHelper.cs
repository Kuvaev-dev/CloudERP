namespace Domain.UtilsAccess
{
    public interface IPasswordHelper
    {
        string HashPassword(string password, out string salt);
        bool VerifyPassword(string password, string storedHash, string storedSalt);
    }
}
