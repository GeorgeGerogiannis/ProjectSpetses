using System.Security.Cryptography;
using System.Text;

namespace ProjectSpetses.Other
{
    public class Hashing
    {
        public Hashing()
        {

        }

        // Generate a random salt
        public string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        // Hash the password with the given salt
        public string HashPassword(string password, string salt)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
