using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Security;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Xml.Linq;
using Xunit;
using PleasanterStartup = Implem.Pleasanter.NetCore.Startup;

namespace Implem.PleasanterTest.Tests.Security
{
    [Collection(nameof(AspNetCoreDataProtectionTests))]
    public class AspNetCoreDataProtectionTests : IDisposable
    {
        private readonly ParameterAccessor.Parts.Security savedSecurity;

        public AspNetCoreDataProtectionTests()
        {
            savedSecurity = Parameters.Security;
            Parameters.Security = new ParameterAccessor.Parts.Security()
            {
                AspNetCoreDataProtection = new AspNetCoreDataProtection()
                {
                    XmlAesKey = "unit-test-data-protection-key"
                }
            };
        }

        public void Dispose()
        {
            Parameters.Security = savedSecurity;
        }

        [Fact]
        public void DataProtectionParameterDefaultsDoNotEnableKeyValueStore()
        {
            var dataProtection = new AspNetCoreDataProtection();

            Assert.Null(dataProtection.BlobContainerUri);
            Assert.Null(dataProtection.KeyIdentifier);
            Assert.Null(dataProtection.KeyFileName);
            Assert.Null(dataProtection.XmlAesKey);
            Assert.Null(dataProtection.KeyValueStoreConnectionString);
            Assert.Null(dataProtection.KeyValueStoreKeyName);
        }

        [Fact]
        public void KvsParameterDefaultsDoNotUseRedisForSession()
        {
            var kvs = new Kvs();

            Assert.Null(kvs.ConnectionStringForSession);
        }

        [Fact]
        public void AspNetCoreKeyManagementXmlEncryptorEncryptsAndDecryptsKeyElement()
        {
            var plaintextElement = new XElement("key",
                new XElement("descriptor", "plain-text-key-material"));

            var encryptedInfo = new AspNetCoreKeyManagementXmlEncryptor().Encrypt(plaintextElement);

            Assert.Equal("encryptedKey", encryptedInfo.EncryptedElement.Name.LocalName);
            Assert.DoesNotContain(
                "plain-text-key-material",
                encryptedInfo.EncryptedElement.ToString(SaveOptions.DisableFormatting));
            Assert.Equal(typeof(AspNetCoreKeyManagementXmlDecryptor), encryptedInfo.DecryptorType);
            Assert.True(XNode.DeepEquals(
                plaintextElement,
                new AspNetCoreKeyManagementXmlDecryptor().Decrypt(encryptedInfo.EncryptedElement)));
        }

        [Fact]
        public void StartupConfiguresDataProtectionXmlEncryptor()
        {
            var services = new ServiceCollection();
            var method = typeof(PleasanterStartup).GetMethod(
                "AddAspNetCoreDataProtectionXmlEncryptor",
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: [typeof(IServiceCollection)],
                modifiers: null);
            Assert.NotNull(method);

            method.Invoke(null, [services]);

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;
            Assert.IsType<AspNetCoreKeyManagementXmlEncryptor>(options.XmlEncryptor);
        }

        [Fact]
        public void StartupRequiresXmlAesKeyWhenKeyValueStoreConnectionStringIsSet()
        {
            Parameters.Security.AspNetCoreDataProtection.XmlAesKey = null;
            var method = typeof(PleasanterStartup).GetMethod(
                "ValidateAspNetCoreDataProtectionKeyValueStoreParameters",
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);
            Assert.NotNull(method);

            var exception = Assert.Throws<TargetInvocationException>(() =>
                method.Invoke(null, ["localhost:6379,abortConnect=false"]));
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void StartupUsesServiceNameForDefaultKeyValueStoreKeyName()
        {
            var method = typeof(PleasanterStartup).GetMethod(
                "GetAspNetCoreDataProtectionKeyValueStoreKeyName",
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);
            Assert.NotNull(method);

            var keyName = Assert.IsType<string>(method.Invoke(null, []));
            Assert.EndsWith(":DataProtection-Keys", keyName);
        }
    }
}
