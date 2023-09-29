using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Implem.Libraries.Utilities
{
    public class CryptographyAes
    {
        public static string AesEncrypt(string encryptString, string encryptKey, string aesIv)
        {
            if (encryptString.Trim().IsNullOrEmpty()) return null;
            if (encryptKey.Trim().IsNullOrEmpty()) return null;
            using (var aes = Aes.Create())
            {
                using var encryptor = aes.CreateEncryptor(
                    Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32)),
                    Encoding.UTF8.GetBytes(aesIv));
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using var sw = new StreamWriter(cs);
                    sw.Write(encryptString);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string AesDecrypt(string decryptString, string decryptKey, string aesIv)
        {
            if (decryptString.Trim().IsNullOrEmpty()) return null;
            if (decryptKey.Trim().IsNullOrEmpty()) return null;
            using (var aes = Aes.Create())
            {
                using var decryptor = aes.CreateDecryptor(
                    Encoding.UTF8.GetBytes(decryptKey.Substring(0, 32)),
                    Encoding.UTF8.GetBytes(aesIv));
                using var ms = new MemoryStream(Convert.FromBase64String(decryptString));
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
        }
    }
}
