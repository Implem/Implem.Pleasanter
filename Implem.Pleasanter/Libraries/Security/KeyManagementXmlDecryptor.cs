using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class KeyManagementXmlDecryptor : IXmlDecryptor
    {
        public XElement Decrypt(XElement encryptedElement)
        {
            var jsonXmlStr = AesCryptography.AESDecrypt(
                (string)encryptedElement.Element("value"),
                KeyManagemenXmltUtils.AesKey,
                KeyManagemenXmltUtils.AesIv);
            return JsonConvert.DeserializeObject<XElement>(jsonXmlStr);
        }
    }
}
