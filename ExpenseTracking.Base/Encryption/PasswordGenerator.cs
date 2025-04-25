using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracking.Base;

public static class PasswordGenerator
{
    public static string CreateSHA256(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }

    public static string CreateSHA256(string input, string salt)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = sha256.ComputeHash(
            System.Text.Encoding.UTF8.GetBytes(salt + input)
        );
        return Convert.ToHexString(bytes);
    }

    public static string GeneratePassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable
            .Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray()
        );
    }
}
