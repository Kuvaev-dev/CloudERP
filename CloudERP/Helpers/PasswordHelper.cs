using System;
using System.Linq;
using System.Security.Cryptography;

namespace CloudERP.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, out string salt)
        {
            using (var hmac = new HMACSHA512())
            {
                var saltBytes = hmac.Key;
                var hashedPassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                salt = Convert.ToBase64String(saltBytes);
                return Convert.ToBase64String(hashedPassword);
            }
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
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