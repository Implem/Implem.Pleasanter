using Implem.Libraries.Utilities;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AspNetCoreKeyManagementXmlEncryptor : IXmlEncryptor
    {
        public EncryptedXmlInfo Encrypt(XElement plaintextElement)
        {
            var encryptedData = CryptographyAes.AesEncrypt(
                JsonConvert.SerializeObject(plaintextElement),
                AspNetCoreKeyManagemenXmltUtils.AesKey,
                AspNetCoreKeyManagemenXmltUtils.AesIv);
            var newElement = new XElement("encryptedKey",
                new XComment(" This key is encrypted with AES."),
                new XElement("value", encryptedData));
            return new EncryptedXmlInfo(newElement, typeof(AspNetCoreKeyManagementXmlDecryptor));
        }
    }
}
