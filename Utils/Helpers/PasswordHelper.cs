using Domain.UtilsAccess;
using System.Security.Cryptography;

namespace Utils.Helpers
{
    public class PasswordHelper : IPasswordHelper
    {
        public string HashPassword(string password, out string salt)
        {
            using (var hmac = new HMACSHA512())
            {
                var saltBytes = hmac.Key;
                var hashedPassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                salt = Convert.ToBase64String(saltBytes);
                return Convert.ToBase64String(hashedPassword);
            }
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using (var hmac = new HMACSHA512(saltBytes))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var storedHashBytes = Convert.FromBase64String(storedHash);

                return computedHash.SequenceEqual(storedHashBytes);
            }
        }
    }
}