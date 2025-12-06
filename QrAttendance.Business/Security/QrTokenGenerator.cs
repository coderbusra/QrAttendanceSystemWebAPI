using System.Security.Cryptography;
using QrAttendanceSystem.Core.Security;

namespace QrAttendanceSystem.Business.Security;

public class QrTokenGenerator : IQrTokenGenerator
{
    private const int TokenSize = 32; // byte

    public string Generate()
    {
        // Rastgele byte üret
        var bytes = RandomNumberGenerator.GetBytes(TokenSize);

        // Base64 URL-safe string'e çevir
        var token = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        return token;
    }
}
