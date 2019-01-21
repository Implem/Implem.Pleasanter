using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Saml
    {
        private static bool processing = false;

        public static void UpdateOrInsert(
             Context context,
             int tenantId,
             string loginId,
             string name,
             string mailAddress,
             bool tenantManager,
             DateTime synchronizedTime)
        {
            var user = new UserModel().Get(
                context: context,
                ss: null,
                where: Rds.UsersWhere()
                    .TenantId(tenantId)
                    .LoginId(loginId)
                    .Name(name)
                    .TenantManager(tenantManager));
            if (user.AccessStatus == Databases.AccessStatuses.Selected)
            {
                return;
            }

            var statements = new List<SqlStatement>();

            var param = Rds.UsersParam()
                .TenantId(tenantId)
                .LoginId(loginId)
                .Name(name)
                .TenantManager(tenantManager)
                .SynchronizedTime(synchronizedTime);

            statements.Add(Rds.UpdateOrInsertUsers(
                param: param,
                where: Rds.UsersWhere().TenantId(tenantId).LoginId(loginId),
                addUpdatorParam: true,
                addUpdatedTimeParam: true));
            if (!mailAddress.IsNullOrEmpty())
            {
                statements.Add(Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerType("Users")
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(loginId)))));
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(loginId)))
                        .OwnerType("Users")
                        .MailAddress(mailAddress)));
            }
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: tenantId, type: StatusUtilities.Types.UsersUpdated));
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
        }
        
        public static void RegisterSamlConfiguration()
        {
            if (Parameters.Authentication.Provider != "SAML") { return; }
            var context = new Context(request: false, sessionStatus: false, sessionData: false, user: false);
            foreach (var tenant in new TenantCollection(context, SiteSettingsUtilities.TenantsSiteSettings(context)))
            {
                SetIdpConfiguration(context, tenant.TenantId);
            }
        }

        public static string SetIdpConfiguration(Context context, int tenantId)
        {
            var contractSettings = TenantUtilities.GetContractSettings(context, tenantId);
            if (contractSettings == null
                || contractSettings.SamlCompanyCode.IsNullOrEmpty()
                || contractSettings.SamlLoginUrl.IsNullOrEmpty()
                || contractSettings.SamlThumbprint.IsNullOrEmpty())
            {
                return null;
            }
            if(!FindCert(context, contractSettings.SamlThumbprint))
            {
                return null;
            }
            try
            {
                var section = (SustainsysSaml2Section)ConfigurationManager.GetSection("sustainsys.saml2");
                var loginUrl = contractSettings.SamlLoginUrl;
                var idp = loginUrl.TrimEnd(new[] { '/' }).Substring(0, loginUrl.LastIndexOf('/') + 1);
                if (processing) { System.Threading.Thread.Sleep(300); }
                if (processing == false)
                {
                    processing = true;
                    try
                    {
                        IdentityProviderElement newProvider = null;
                        CertificateElement newCert = null;
                        var provider = section.IdentityProviders.FirstOrDefault(p => p.EntityId == idp);
                        if (provider != null)
                        {
                            string signOnUrl = provider.SignOnUrl.ToString();
                            string findValue = provider.SigningCertificate.FindValue;
                            if (signOnUrl == contractSettings.SamlLoginUrl
                                && findValue == contractSettings.SamlThumbprint)
                            {
                                return $"~/Saml2/SignIn?idp={idp}";
                            }
                            else
                            {

                                newProvider = provider;
                                newCert = provider.SigningCertificate;

                                WriteIdPSettings(contractSettings?.SamlLoginUrl, contractSettings?.SamlThumbprint, idp, newProvider, newCert);
                                try
                                {
                                    var spOptions = new SPOptions(SustainsysSaml2Section.Current);
                                    var options = new Options(spOptions);
                                    SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(options);
                                    SustainsysSaml2Section.Current.Federations.RegisterFederations(options);

                                    var optionsFromConfiguration = typeof(Options).GetField("optionsFromConfiguration",
                                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                                    optionsFromConfiguration.SetValue(null, new Lazy<Options>(() => options, true));

                                }
                                catch
                                {
                                    WriteIdPSettings(signOnUrl, findValue, idp, newProvider, newCert);
                                    throw;
                                }

                            }
                        }
                        else
                        {
                            newProvider = new IdentityProviderElement();
                            newCert = new CertificateElement();
                            WriteIdPSettings(contractSettings?.SamlLoginUrl, contractSettings?.SamlThumbprint, idp, newProvider, newCert);
                            AddIdP(section, newProvider);
                        }
                    }
                    finally
                    {
                        processing = false;
                    }
                }
                return $"~/Saml2/SignIn?idp={idp}";
            }
            catch (System.Exception e)
            {
                new SysLogModel(context, e);
                return null;
            }
        }

        private static void AddIdP(SustainsysSaml2Section section, IdentityProviderElement newProvider)
        {
            var isReadonly = typeof(ConfigurationElementCollection).GetField("bReadOnly",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isReadonly.SetValue(section.IdentityProviders, false);
            var providers = typeof(ConfigurationElementCollection).GetMethod("BaseAdd",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, new[] { typeof(ConfigurationElement) }, null);
            providers.Invoke(section.IdentityProviders, new[] { newProvider });
        }

        private static void WriteIdPSettings(string samlLoginUrl, string samlThumbprint, string idp, IdentityProviderElement newProvider, CertificateElement newCert)
        {
            var providerReadonly = typeof(IdentityProviderElement).GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            providerReadonly.SetValue(newProvider, false);
            var providerIndexer = typeof(IdentityProviderElement).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .First(p => p.GetIndexParameters().Select(i => i.ParameterType).SequenceEqual(new[] { typeof(string) }));
            providerIndexer.SetValue(newProvider, idp, new[] { "entityId" });
            providerIndexer.SetValue(newProvider, new System.Uri(samlLoginUrl), new[] { "signOnUrl" });
            providerIndexer.SetValue(newProvider, true, new[] { "allowUnsolicitedAuthnResponse" });
            providerIndexer.SetValue(newProvider, Saml2BindingType.HttpRedirect, new[] { "binding" });
            var certReadonly = typeof(CertificateElement).GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            certReadonly.SetValue(newCert, false);
            var certIndexer = typeof(IdentityProviderElement).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .First(p => p.GetIndexParameters().Select(i => i.ParameterType).SequenceEqual(new[] { typeof(string) }));
            certIndexer.SetValue(newCert, StoreName.My, new[] { "storeName" });
            certIndexer.SetValue(newCert, StoreLocation.CurrentUser, new[] { "storeLocation" });
            certIndexer.SetValue(newCert, X509FindType.FindByThumbprint, new[] { "x509FindType" });
            certIndexer.SetValue(newCert, samlThumbprint, new[] { "findValue" });
            providerIndexer.SetValue(newProvider, newCert, new[] { "signingCertificate" });
        }

        private static bool FindCert(Context context, string findValue)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false);
                if (certs.Count != 1)
                {
                    new SysLogModel(context, $"Invalid SAML certificate. (Thumbrint={findValue})");
                    return false;
                }
            }
            finally
            {
                store.Close();
            }
            return true;
        }
    }
}