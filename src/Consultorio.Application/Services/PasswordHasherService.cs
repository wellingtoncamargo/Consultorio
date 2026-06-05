using System;
using System.Security.Cryptography;

namespace Consultorio.Application.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool Verify(string hashed, string password);
    }

    public class PasswordHasherService : IPasswordHasherService
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits

        public string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);

            using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = derive.GetBytes(KeySize);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(key);
        }

        public bool Verify(string hashed, string password)
        {
            try
            {
                var parts = hashed.Split('.', 2);
                if (parts.Length != 2) return false;
                var salt = Convert.FromBase64String(parts[0]);
                var expectedKey = Convert.FromBase64String(parts[1]);

                using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                var key = derive.GetBytes(KeySize);

                return CryptographicOperations.FixedTimeEquals(key, expectedKey);
            }
            catch
            {
                return false;
            }
        }
    }
}