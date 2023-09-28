using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Libraries.Security
{
    public class KeyManagemenXmltUtils
    {
        // AES暗号化 key生成文字列 (256bitキー(32文字))
        public static string AesKey
            => Parameters.Security.AspNetCoreDataProtection.XmlAesKey + "012345678901234567890123456789012345";

        // AES暗号化 初期化ベクトル文字列 (128bit(16文字))
        public static string AesIv => "pleasanterimplem";
    }
}
