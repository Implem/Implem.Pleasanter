using Implem.DefinitionAccessor;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AspNetCoreKeyManagemenXmltUtils
    {
        public static string AesKey
            => Parameters.Security.AspNetCoreDataProtection.XmlAesKey + GetHashStr(Parameters.Security.AspNetCoreDataProtection.XmlAesKey);

        public static string AesIv => GenerateRandomIv();

        private static string GenerateRandomIv()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                return Convert.ToBase64String(aes.IV);
            }
        }

        private static string GetHashStr(string value)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));
        }
    }
}
