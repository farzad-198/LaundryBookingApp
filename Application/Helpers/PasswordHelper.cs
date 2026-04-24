using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // make password hash
            string cleanPassword = password.Trim();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(cleanPassword);
            byte[] hashBytes = SHA256.HashData(passwordBytes);

            return Convert.ToHexString(hashBytes);
        }

        public static bool IsPasswordMatch(string savedPassword, string enteredPassword)
        {
            // check password with hash
            string enteredPasswordHash = HashPassword(enteredPassword);
            return savedPassword == enteredPasswordHash;
        }

        public static bool IsHashedPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length != 64)
            {
                return false;
            }

            foreach (char item in password)
            {
                bool isNumber = item >= '0' && item <= '9';
                bool isUpperLetter = item >= 'A' && item <= 'F';

                if (!isNumber && !isUpperLetter)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
