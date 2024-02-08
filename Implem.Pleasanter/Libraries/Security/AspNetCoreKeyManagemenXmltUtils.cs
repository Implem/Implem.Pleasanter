using Implem.DefinitionAccessor;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AspNetCoreKeyManagemenXmltUtils
    {
        // AES暗号化 key生成文字列 (256bitキー(32文字))
        public static string AesKey
            => Parameters.Security.AspNetCoreDataProtection.XmlAesKey + GetHashStr(Parameters.Security.AspNetCoreDataProtection.XmlAesKey);

        // AES暗号化 初期化ベクトル文字列 (128bit(16文字))
        public static string AesIv => "pleasanterimplem";

        private static string GetHashStr(string value)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));
        }
    }
}
