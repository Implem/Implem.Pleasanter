﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Saml
    {
        private static ConcurrentDictionary<string, (string Guid, Sustainsys.Saml2.IdentityProvider Idp)> IdpCache
            = new ConcurrentDictionary<string, (string Guid, Sustainsys.Saml2.IdentityProvider Idp)>();
        private static Saml2Options Options;

        public static SamlAttributes MapAttributes(IEnumerable<Claim> claims, string nameId)
        {
            var attributes = new SamlAttributes();
            var setMailAddress = false;
            if (Parameters.Authentication?.SamlParameters?
                    .Attributes?["MailAddress"] == "{NameId}")
            {
                setMailAddress = true;
                attributes.Add("MailAddress", nameId);
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
                if ((typeof(UserModel).GetField(attribute.Key) != null
                    || attribute.Key == "Dept" || attribute.Key == "DeptCode"
                    || (setMailAddress == false && attribute.Key == "MailAddress")))
                {
                    attributes.Add(attribute.Key, claim.Value);
                }
            }
            return attributes;
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

            var userExists = Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere()
                        .TenantId(tenantId)
                        .LoginId(loginId))) > 0;
            if (userExists)
            {
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
                .Name(name,
                    _using: (Parameters.Authentication.SamlParameters.DisableOverwriteName == true)
                        ? (false == userExists)
                        : true)
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

        public static void SetSPOptions(Saml2Options options)
        {
            Options = options;
            var paramSPOptions = Parameters.Authentication.SamlParameters.SPOptions;
            options.SPOptions.AuthenticateRequestSigningBehavior = paramSPOptions.AuthenticateRequestSigningBehavior
                .ToEnum(SigningBehavior.IfIdpWantAuthnRequestsSigned);
            options.SPOptions.ReturnUrl = paramSPOptions.ReturnUrl.IsNullOrEmpty()
                ? null
                : new Uri(paramSPOptions.ReturnUrl);
            options.SPOptions.EntityId = paramSPOptions.EntityId.IsNullOrEmpty()
                ? null
                : new EntityId(paramSPOptions.EntityId);
            options.SPOptions.MinIncomingSigningAlgorithm = paramSPOptions.MinIncomingSigningAlgorithm
                ?? options.SPOptions.MinIncomingSigningAlgorithm;
            options.SPOptions.OutboundSigningAlgorithm = paramSPOptions.OutboundSigningAlgorithm
                ?? options.SPOptions.OutboundSigningAlgorithm;
            options.SPOptions.PublicOrigin = paramSPOptions.PublicOrigin.IsNullOrEmpty()
                ? null
                : new Uri(paramSPOptions.PublicOrigin);
            if (paramSPOptions.IgnoreMissingInResponseTo == true)
            {
                options.SPOptions.Compatibility.IgnoreMissingInResponseTo = true;
            }
            if (paramSPOptions.ServiceCertificates != null)
            {
                foreach (var cert in paramSPOptions.ServiceCertificates)
                {
                    var x509Store = new X509Store(
                        cert.StoreName.ToEnum(StoreName.My),
                        cert.StoreLocation.ToEnum(StoreLocation.CurrentUser));
                    x509Store.Open(OpenFlags.OpenExistingOnly);
                    try
                    {
                        var cer = x509Store.Certificates.Find(
                            cert.X509FindType.ToEnum(X509FindType.FindByThumbprint),
                            cert.FindValue,
                            false);
                        var serviceCert = new Sustainsys.Saml2.ServiceCertificate()
                        {
                            Certificate = cer[0],
                            Status = cert.Status.ToEnum(CertificateStatus.Current),
                            Use = cert.Use.ToEnum(CertificateUse.Both)
                        };
                        options.SPOptions.ServiceCertificates.Add(serviceCert);
                    }
                    finally
                    {
                        x509Store.Close();
                    }
                }
            }
            var paramIdps = Parameters.Authentication.SamlParameters.IdentityProviders;
            if (paramIdps != null)
            {
                foreach (var paramIdp in paramIdps)
                {
                    var idp = new Sustainsys.Saml2.IdentityProvider(
                        new EntityId(paramIdp.EntityId),
                        options.SPOptions)
                    {
                        SingleSignOnServiceUrl = paramIdp.SignOnUrl.IsNullOrEmpty()
                            ? null
                            : new Uri(paramIdp.SignOnUrl),
                        SingleLogoutServiceUrl = paramIdp.LogoutUrl.IsNullOrEmpty()
                            ? null
                            : new Uri(paramIdp.LogoutUrl),
                        AllowUnsolicitedAuthnResponse = paramIdp.AllowUnsolicitedAuthnResponse,
                        Binding = paramIdp.Binding.ToEnum(Saml2BindingType.HttpRedirect),
                        WantAuthnRequestsSigned = paramIdp.WantAuthnRequestsSigned,
                        DisableOutboundLogoutRequests = paramIdp.DisableOutboundLogoutRequests
                    };
                    if (paramIdp.LoadMetadata)
                    {
                        idp.MetadataLocation = paramIdp.MetadataLocation.IsNullOrEmpty()
                            ? idp.MetadataLocation
                            : paramIdp.MetadataLocation;
                        idp.LoadMetadata = paramIdp.LoadMetadata;
                    }
                    if (paramIdp.SigningCertificate != null)
                    {
                        var store = new X509Store(
                            paramIdp.SigningCertificate.StoreName.ToEnum(StoreName.My),
                            paramIdp.SigningCertificate.StoreLocation.ToEnum(StoreLocation.CurrentUser));
                        store.Open(OpenFlags.OpenExistingOnly);
                        try
                        {
                            var certs = store.Certificates.Find(
                                paramIdp.SigningCertificate.X509FindType.ToEnum(X509FindType.FindByThumbprint),
                                paramIdp.SigningCertificate.FindValue, false);
                            idp.SigningKeys.AddConfiguredKey(certs[0]);
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                    options.IdentityProviders.Add(idp);
                }
            }
            if (Parameters.Authentication.Provider == "SAML-MultiTenant")
            {
                options.Notifications.SelectIdentityProvider = (entityId, rd) =>
                {
                    return GetSamlIdp(entityId);
                };
                options.Notifications.GetIdentityProvider = (entityId, rd, op) =>
                {
                    return GetSamlIdp(entityId) ?? op.IdentityProviders.Default;
                };
            }
        }

        private static Sustainsys.Saml2.IdentityProvider GetSamlIdp(EntityId entityId)
        {
            if (entityId == null)
            {
                return null;
            }
            //Cacheに存在している場合、設定内容が変更されていなければChacheのデータを返す
            if (IdpCache.TryGetValue(entityId.Id, out (string Guid, Sustainsys.Saml2.IdentityProvider Idp) cachedIdp))
            {
                return cachedIdp.Idp;
            }
            return null;
        }

        private static Sustainsys.Saml2.IdentityProvider CreateIdpFromMetadata(EntityId entityId, string loginUrl, byte[] metadata)
        {
            using (var stream = new System.IO.MemoryStream(metadata))
            {
                var doc = XDocument.Load(stream);
                var md = (XNamespace)"urn:oasis:names:tc:SAML:2.0:metadata";
                var entityDescriptor = doc.Element(md + "EntityDescriptor");
                if (entityDescriptor == null)
                {
                    return null;
                }
                var x_entityId = entityDescriptor
                    .Attributes("entityID")
                    .FirstOrDefault()?
                    .Value;
                if (x_entityId == null || x_entityId != entityId.Id)
                {
                    return null;
                }
                var ds = (XNamespace)"http://www.w3.org/2000/09/xmldsig#";
                var x509Certificate = entityDescriptor
                    .Descendants(ds + "X509Certificate")?
                    .FirstOrDefault()?
                    .Value;
                if (x509Certificate == null)
                {
                    return null;
                }
                var idp = new Sustainsys.Saml2.IdentityProvider(entityId, Options.SPOptions)
                {
                    SingleSignOnServiceUrl = new Uri(loginUrl),
                    AllowUnsolicitedAuthnResponse = true,
                    WantAuthnRequestsSigned = false,
                    DisableOutboundLogoutRequests = true,
                    Binding = Saml2BindingType.HttpRedirect
                };

                idp.SigningKeys.AddConfiguredKey(
                    X509Certificate2.CreateFromPem($"-----BEGIN CERTIFICATE-----{x509Certificate}-----END CERTIFICATE-----"));
                return idp;
            }
        }

        public static void RegisterSamlConfiguration(Context context, Saml2Options options)
        {
            if (Parameters.Authentication.Provider != "SAML-MultiTenant") { return; }
            //SAML認証設定のあるテナントを取得し、その情報からIDPオブジェクトを作成する
            //作成したIDPオブジェクトはCacheとしてメモリ上に保持される
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn()
                        .TenantId()
                        .ContractSettings(),
                    where: Rds.TenantsWhere()
                        .Add(raw: $"(\"Tenants\".\"ContractDeadline\" is null or \"Tenants\".\"ContractDeadline\" >= {context.Sqls.CurrentDateTime})")
                        .Comments(_operator: " is not null")
                        .Comments("", _operator: "<>")))
                            .AsEnumerable();
            foreach (var dataRow in dataRows)
            {
                try
                {
                    var tenantId = dataRow.Int("TenantId");
                    var contractSettings = dataRow.String("ContractSettings").Deserialize<ContractSettings>()
                        ?? new ContractSettings();
                    SetIdpConfiguration(
                        context: context,
                        options: options,
                        tenantId: tenantId,
                        contractSettings: contractSettings);
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: context,
                        e: e);
                }
            }
        }

        public static void SetIdpConfiguration(Context context, Saml2Options options, int tenantId, ContractSettings contractSettings)
        {
            if (!HasSamlSettings(contractSettings))
            {
                new SysLogModel(
                    context: context,
                    method: nameof(SetIdpConfiguration),
                    message: $"Saml settings is incomplete. [tenantId: {tenantId}]",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
                return;
            }
            var entityId = contractSettings.SamlLoginUrl.Substring(0, contractSettings.SamlLoginUrl.TrimEnd('/').LastIndexOf('/') + 1);
            if (!SetIdpCache(
                context: context,
                tenantId: tenantId,
                entityId: entityId,
                contractSettings: contractSettings))
            {
                new SysLogModel(
                    context: context,
                    method: nameof(SetIdpConfiguration),
                    message: $"Saml settings is incomplete. {contractSettings.Name}, SamlLoginUrl={contractSettings.SamlLoginUrl}, Metadata={contractSettings.SamlMetadataGuid}",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
        }

        public static bool HasSamlSettings(ContractSettings contractSettings)
        {
            return contractSettings != null
                && !contractSettings.SamlCompanyCode.IsNullOrEmpty()
                && !contractSettings.SamlLoginUrl.IsNullOrEmpty()
                && !contractSettings.SamlMetadataGuid.IsNullOrEmpty();
        }

        public static bool SetIdpCache(Context context, int tenantId, string entityId, ContractSettings contractSettings)
        {
            try
            {
                var loginUrl = contractSettings.SamlLoginUrl;
                var guid = contractSettings.SamlMetadataGuid;
                if (IdpCache.TryGetValue(entityId, out (string Guid, Sustainsys.Saml2.IdentityProvider Idp) cachedData))
                {
                    //キャッシュ登録済みの場合は何もしない
                    if (cachedData.Guid == guid
                        && cachedData.Idp.SingleSignOnServiceUrl == new Uri(loginUrl))
                    {
                        return true;
                    }
                }
                //キャッシュにない場合のみ、Binariesテーブルからメタデータを取得してIdpを作成・キャッシュに登録する。
                var metadata = Repository.ExecuteScalar_bytes(
                    context: context,
                    transactional: false,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectBinaries(
                            column: Rds.BinariesColumn().Bin(),
                            where: Rds.BinariesWhere()
                                .TenantId(tenantId)
                                .Guid(guid))
                    });
                if (metadata == null)
                {
                    new SysLogModel(
                       context: context,
                       method: nameof(SetIdpConfiguration),
                       message: $"Metadata not found. {contractSettings.Name}, EntityId={entityId}, Metadata={contractSettings.SamlMetadataGuid}",
                       sysLogType: SysLogModel.SysLogTypes.SystemError);
                    return false;
                }
                var idp = CreateIdpFromMetadata(
                    entityId: new EntityId(entityId),
                    loginUrl: loginUrl,
                    metadata: metadata);
                if (idp == null)
                {
                    new SysLogModel(
                       context: context,
                       method: nameof(SetIdpConfiguration),
                       message: $"Invalid metadata format. {contractSettings.Name}, EntityId={entityId}, Metadata={contractSettings.SamlMetadataGuid}",
                       sysLogType: SysLogModel.SysLogTypes.SystemError);
                    return false;
                }
                IdpCache.AddOrUpdate(entityId, _ => (guid, idp), (_, __) => (guid, idp));
                new SysLogModel(
                    context: context,
                    method: nameof(SetIdpConfiguration),
                    message: $"{contractSettings.Name}, EntityId={entityId}, Metadata={contractSettings.SamlMetadataGuid}",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                return true;
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    e: e);
                return false;
            }
        }

        public static (string redirectUrl, string redirectResultUrl, string html) SamlLogin(Context context)
        {
            if (!Authentications.SAML()
                || context.AuthenticationType != "Federation"
                || context.IsAuthenticated != true)
            {
                return (null, Responses.Locations.SamlLoginFailed(context: context), null);
            }
            Authentications.SignOut(context: context);
            var loginId = context.UserClaims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var attributes = Saml.MapAttributes(context.UserClaims, loginId.Value);
            var name = attributes.UserName;
            TenantModel tenant;
            if (Parameters.Authentication.Provider == "SAML-MultiTenant")
            {
                if (string.IsNullOrEmpty(name))
                {
                    return (null, Responses.Locations.EmptyUserName(context: context), null);
                }
                var ssocode = loginId.Issuer.TrimEnd('/').Substring(loginId.Issuer.TrimEnd('/').LastIndexOf('/') + 1);
                tenant = new TenantModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                    where: Rds.TenantsWhere().Comments(ssocode));
            }
            else
            {
                tenant = new TenantModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                    where: Rds.TenantsWhere().TenantId(Parameters.Authentication.SamlParameters.SamlTenantId));
                if (tenant.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    Rds.ExecuteNonQuery(
                        context: context,
                        connectionString: Parameters.Rds.OwnerConnectionString,
                        statements: new[] {
                            Rds.IdentityInsertTenants(factory: context, on: true),
                            Rds.InsertTenants(
                                param: Rds.TenantsParam()
                                    .TenantId(Parameters.Authentication.SamlParameters.SamlTenantId)
                                    .TenantName("DefaultTenant")),
                            Rds.IdentityInsertTenants(factory: context, on: false)
                        });
                    tenant.TenantId = Parameters.Authentication.SamlParameters.SamlTenantId;
                }
            }
            if (Parameters.Authentication.RejectUnregisteredUser)
            {
                var exists = Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere()
                            .TenantId(tenant.TenantId)
                            .LoginId(loginId.Value))) > 0;
                if (!exists)
                {
                    return (null, Responses.Locations.SamlLoginFailed(context: context), null);
                }
            }
            try
            {
                Saml.UpdateOrInsert(
                    context: context,
                    tenantId: tenant.TenantId,
                    loginId: loginId.Value,
                    name: string.IsNullOrEmpty(name)
                        ? loginId.Value
                        : name,
                    mailAddress: attributes["MailAddress"],
                    synchronizedTime: System.DateTime.Now,
                    attributes: attributes);
            }
            catch (DbException e)
            {
                if (context.SqlErrors.ErrorCode(e) == 2601)
                {
                    return (null, Responses.Locations.LoginIdAlreadyUse(context: context), null);
                }
                throw;
            }
            var user = new UserModel().Get(
                context: context,
                ss: null,
                where: Rds.UsersWhere()
                    .TenantId(tenant.TenantId)
                    .LoginId(loginId.Value));
            if (user.AccessStatus == Databases.AccessStatuses.Selected)
            {
                if (user.Disabled)
                {
                    return (null, Responses.Locations.UserDisabled(context: context), null);
                }
                if (user.Lockout)
                {
                    return (null, Responses.Locations.UserLockout(context: context), null);
                }
                user.Allow(context: context, returnUrl: Responses.Locations.Top(context), createPersistentCookie: true);
                return (null, Responses.Locations.Top(context), null);
            }
            else
            {
                return (null, Responses.Locations.SamlLoginFailed(context: context), null);
            }
        }
    }
}