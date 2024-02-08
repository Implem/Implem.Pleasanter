using Implem.Libraries.Utilities;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AspNetCoreKeyManagementXmlDecryptor : IXmlDecryptor
    {
        public XElement Decrypt(XElement encryptedElement)
        {
            var jsonXmlStr = CryptographyAes.AesDecrypt(
                (string)encryptedElement.Element("value"),
                AspNetCoreKeyManagemenXmltUtils.AesKey,
                AspNetCoreKeyManagemenXmltUtils.AesIv);
            return JsonConvert.DeserializeObject<XElement>(jsonXmlStr);
        }
    }
}
