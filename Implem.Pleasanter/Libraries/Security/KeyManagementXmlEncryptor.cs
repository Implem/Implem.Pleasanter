using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class KeyManagementXmlEncryptor : IXmlEncryptor
    {
        public EncryptedXmlInfo Encrypt(XElement plaintextElement)
        {
            var encryptedData = AesCryptography.AESEncrypt(
                JsonConvert.SerializeObject(plaintextElement),
                KeyManagemenXmltUtils.AesKey,
                KeyManagemenXmltUtils.AesIv);
            var newElement = new XElement("encryptedKey",
                new XComment(" This key is encrypted with AES."),
                new XElement("value", encryptedData));
            return new EncryptedXmlInfo(newElement, typeof(KeyManagementXmlDecryptor));
        }
    }
}
