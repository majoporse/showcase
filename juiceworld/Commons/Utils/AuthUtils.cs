using System.Security.Cryptography;
using System.Text;

namespace Commons.Utils;

public static class AuthUtils
{
    public static string GenerateSalt(int bytesCount = 16)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytesCount));
    }

    public static string HashPassword(string password, string salt, int rounds)
    {
        using var rfc2898DeriveBytes =
            new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), rounds, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(32));
    }
}