﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Saml
    {
        private static bool processing = false;

        public static SamlAttributes MapAttributes(IEnumerable<Claim> claims, string nameId)
        {
            var attributes = new SamlAttributes();
            var addressIsNameId = true == (Parameters.Authentication?.SamlParameters?
                .Attributes?["MailAddress"] == "{NameId}");
            if(addressIsNameId)
            {
                attributes.Add("MailAddress", GetSafeMailAddress(nameId));
            }
            foreach (var claim in claims)
            {
                var attribute = Parameters
                    .Authentication?
                    .SamlParameters?
                    .Attributes?
                    .FirstOrDefault(kvp => kvp.Value == claim.Type) ?? default(KeyValuePair<string, string>);
                if (attribute.Key == null)
                {
                    continue;
                }   
                if((typeof(UserModel).GetField(attribute.Key) != null
                    || attribute.Key == "Dept" || attribute.Key == "DeptCode"
                    || (addressIsNameId == false && attribute.Key == "MailAddress")))
                {
                    attributes.Add(attribute.Key, claim.Value);
                }
            }
            return attributes;
        }

        private static string GetSafeMailAddress(string address)
        {
            try
            {
                return new System.Net.Mail.MailAddress(address).Address;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static void UpdateOrInsert(
             Context context,
             int tenantId,
             string loginId,
             string name,
             string mailAddress,
             DateTime synchronizedTime,
             SamlAttributes attributes)
        {
            var deptCode = attributes[nameof(UserModel.DeptCode)];
            var deptName = attributes[nameof(UserModel.Dept)];
            var deptSettings = !deptCode.IsNullOrEmpty()
                && !deptName.IsNullOrEmpty();
            var isEmptyDeptCode = deptCode == string.Empty;
            var user = new UserModel().Get(
                context: context,
                ss: null,
                where: Rds.UsersWhere()
                    .TenantId(tenantId)
                    .LoginId(loginId)
                    .Name(name)
                    .TenantManager(
                        attributes.TenantManager,
                        _using: attributes[nameof(UserModel.TenantManager)] != null)
                    .FirstName(
                        attributes[nameof(UserModel.FirstName)],
                        _using: attributes[nameof(UserModel.FirstName)] != null)
                    .LastName(
                        attributes[nameof(UserModel.LastName)],
                        _using: attributes[nameof(UserModel.LastName)] != null)
                    .FirstAndLastNameOrder(
                        attributes[nameof(UserModel.FirstAndLastNameOrder)],
                        _using: attributes[nameof(UserModel.FirstAndLastNameOrder)] != null)
                    .UserCode(
                        attributes[nameof(UserModel.UserCode)],
                        _using: attributes[nameof(UserModel.UserCode)] != null)
                    .Birthday(
                        attributes[nameof(UserModel.Birthday)],
                        _using: attributes[nameof(UserModel.Birthday)] != null)
                    .Gender(
                        attributes[nameof(UserModel.Gender)],
                        _using: attributes[nameof(UserModel.Gender)] != null)
                    .Language(
                        attributes[nameof(UserModel.Language)],
                        _using: attributes[nameof(UserModel.Language)] != null)
                    .TimeZone(
                        attributes[nameof(UserModel.TimeZone)],
                        _using: attributes[nameof(UserModel.TimeZone)] != null)
                    .DeptId(
                        sub: Rds.SelectDepts(
                            column: Rds.DeptsColumn().DeptId(),
                            where: Rds.DeptsWhere().DeptCode(deptCode)),
                        _using: deptSettings)
                    .Body(
                        attributes[nameof(UserModel.Body)],
                        _using: attributes[nameof(UserModel.Body)] != null));
            if (!isEmptyDeptCode
                && user.AccessStatus == Databases.AccessStatuses.Selected)
            {
                if (mailAddress.IsNullOrEmpty())
                {
                    return;
                }
                var addressCount = Repository.ExecuteScalar_long(
                    context: context,
                    statements: new[]
                    {
                        Rds.SelectMailAddresses(
                            dataTableName: "Count",
                            column: Rds.MailAddressesColumn().MailAddressesCount(),
                            where: Rds.MailAddressesWhere()
                            .OwnerType("Users")
                            .OwnerId(sub: Rds.SelectUsers(
                                column: Rds.UsersColumn().UserId(),
                                where: Rds.UsersWhere().LoginId(loginId)))
                            .MailAddress(mailAddress))
                    });
                if (addressCount > 0)
                {
                    return;
                }
            }
            var statements = new List<SqlStatement>();
            if (deptSettings)
            {
                statements.Add(Rds.UpdateOrInsertDepts(
                    param: Rds.DeptsParam()
                        .TenantId(tenantId)
                        .DeptCode(deptCode)
                        .DeptName(deptName),
                    where: Rds.DeptsWhere().DeptCode(deptCode)));
            }
            var param = Rds.UsersParam()
                .TenantId(tenantId)
                .LoginId(loginId)
                .Name(name)
                .TenantManager(attributes.TenantManager,
                    _using: attributes[nameof(UserModel.TenantManager)] != null)
                .SynchronizedTime(synchronizedTime)
                .FirstName(
                        attributes[nameof(UserModel.FirstName)],
                        _using: attributes[nameof(UserModel.FirstName)] != null)
                .LastName(
                    attributes[nameof(UserModel.LastName)],
                    _using: attributes[nameof(UserModel.LastName)] != null)
                .FirstAndLastNameOrder(
                    attributes[nameof(UserModel.FirstAndLastNameOrder)],
                    _using: attributes[nameof(UserModel.FirstAndLastNameOrder)] != null)
                .UserCode(
                        attributes[nameof(UserModel.UserCode)],
                        _using: attributes[nameof(UserModel.UserCode)] != null)
                .DeptId(
                    sub: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        where: Rds.DeptsWhere().DeptCode(deptCode)),
                    _using: deptSettings)
                .DeptId(0, _using: isEmptyDeptCode)
                .Birthday(
                    attributes[nameof(UserModel.Birthday)],
                    _using: attributes[nameof(UserModel.Birthday)] != null)
                .Gender(
                    attributes[nameof(UserModel.Gender)],
                    _using: attributes[nameof(UserModel.Gender)] != null)
                .Language(
                    attributes[nameof(UserModel.Language)],
                    _using: attributes[nameof(UserModel.Language)] != null)
                .TimeZone(
                    attributes[nameof(UserModel.TimeZone)],
                    _using: attributes[nameof(UserModel.TimeZone)] != null)
                .Body(
                    attributes[nameof(UserModel.Body)],
                    _using: attributes[nameof(UserModel.Body)] != null);
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
                tenantId: tenantId, type: StatusUtilities.Types.DeptsUpdated));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: tenantId, type: StatusUtilities.Types.UsersUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
        }
        
        public static void RegisterSamlConfiguration()
        {
            if (Parameters.Authentication.Provider != "SAML-MultiTenant") { return; }
            var context = new Context(request: false, sessionStatus: false, sessionData: false, user: false);
            foreach (var tenant in new TenantCollection(context, SiteSettingsUtilities.TenantsSiteSettings(context),
                where: Rds.TenantsWhere()
                    .Comments(_operator: " is not null")
                    .Comments("", _operator:"<>")))
            {
                SetIdpConfiguration(context, tenant.TenantId, true);
                new SysLogModel(context, "SetIdpConfiguration:" + "[" + tenant.TenantId + "]" + tenant.Title);
            }
        }

        public static string SetIdpConfiguration(Context context, int tenantId, bool startup = false)
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
                                    Sustainsys.Saml2.Mvc.Saml2Controller.Options = Options.FromConfiguration;
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
                            if (startup == false)
                            {
                                var spOptions = new SPOptions(SustainsysSaml2Section.Current);
                                var options = new Options(spOptions);
                                SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(options);
                                SustainsysSaml2Section.Current.Federations.RegisterFederations(options);
                                var optionsFromConfiguration = typeof(Options).GetField("optionsFromConfiguration",
                                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                                optionsFromConfiguration.SetValue(null, new Lazy<Options>(() => options, true));
                                Sustainsys.Saml2.Mvc.Saml2Controller.Options = Options.FromConfiguration;
                            }
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
                var today = DateTime.Today;
                if (certs[0].NotBefore.Date >  today|| certs[0].NotAfter.Date < today)
                {
                    new SysLogModel(context, $"Certificate expired ({certs[0].NotBefore.ToString("yyyy/MM/dd")} - {certs[0].NotAfter.ToString("yyyy/MM/dd")})");
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