using System.Security.Cryptography;
using System.Text;

namespace MamlatdarEcourt.Helpers
{
    public static class HashHelper
    {
        public static string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool ConstantTimeEquals(string? a, string b)
        {
            if (a == null || b == null)
                return false;

            int diff = a.Length ^ b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}
