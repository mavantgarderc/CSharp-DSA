using System.Security.Cryptography;
using System.Text;

namespace Application.Utilities;

public static class PasswordHelper
{
    public static string EncodePasswordMd5(string pass)
    {
        byte[] originalBytes = Encoding.ASCII.GetBytes(pass);
        using var md5 = MD5.Create();
        byte[] encodedBytes = md5.ComputeHash(originalBytes);

        return BitConverter.ToString(encodedBytes).Replace("-", "");
    }
}
