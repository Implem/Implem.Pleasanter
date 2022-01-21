using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Saml
    {
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
            var paramSPOptions = Parameters.Authentication.SamlParameters.SPOptions;
            options.SPOptions.AuthenticateRequestSigningBehavior
                = paramSPOptions.AuthenticateRequestSigningBehavior
                    .ToEnum(SigningBehavior.IfIdpWantAuthnRequestsSigned);
            options.SPOptions.ReturnUrl
                = paramSPOptions.ReturnUrl.IsNullOrEmpty() ? null : new Uri(paramSPOptions.ReturnUrl);
            options.SPOptions.EntityId
                = paramSPOptions.EntityId.IsNullOrEmpty() ? null : new EntityId(paramSPOptions.EntityId);
            options.SPOptions.MinIncomingSigningAlgorithm
                = paramSPOptions.MinIncomingSigningAlgorithm
                ?? options.SPOptions.MinIncomingSigningAlgorithm;
            options.SPOptions.OutboundSigningAlgorithm
                = paramSPOptions.OutboundSigningAlgorithm
                ?? options.SPOptions.OutboundSigningAlgorithm;
            options.SPOptions.PublicOrigin
                = paramSPOptions.PublicOrigin.IsNullOrEmpty()? null: new Uri(paramSPOptions.PublicOrigin);
            if(paramSPOptions.IgnoreMissingInResponseTo == true)
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
                        SingleSignOnServiceUrl = paramIdp.SignOnUrl.IsNullOrEmpty() ? null : new Uri(paramIdp.SignOnUrl),
                        SingleLogoutServiceUrl = paramIdp.LogoutUrl.IsNullOrEmpty() ? null : new Uri(paramIdp.LogoutUrl),
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
            options.Notifications.GetIdentityProvider = (entityId, rd, opt) =>
            {
                if (!rd.ContainsKey("SamlLoginUrl")
                    && !rd.ContainsKey("SamlThumbprint"))
                {
                    return opt.IdentityProviders.Default;
                }
                var loginUrl = rd["SamlLoginUrl"];
                var thumbprint = rd["SamlThumbprint"];

                var defaultIdp = opt.IdentityProviders.Default;
                var metadataPath = "~/App_Data/Temp/SamlMetadata/test001.xml";
                var idp = new Sustainsys.Saml2.IdentityProvider(entityId, options.SPOptions)
                {
                    MetadataLocation = metadataPath
                    //SingleSignOnServiceUrl = new Uri(loginUrl),
                    //SingleLogoutServiceUrl = defaultIdp.SingleLogoutServiceUrl,
                    //AllowUnsolicitedAuthnResponse = defaultIdp.AllowUnsolicitedAuthnResponse,
                    //Binding = defaultIdp.Binding,
                    //WantAuthnRequestsSigned = defaultIdp.WantAuthnRequestsSigned,
                    //DisableOutboundLogoutRequests = defaultIdp.DisableOutboundLogoutRequests,
                };
                idp.LoadMetadata = true;
                //var store = new X509Store(
                //    StoreName.My,
                //    StoreLocation.CurrentUser);
                //store.Open(OpenFlags.OpenExistingOnly);
                //try
                //{
                //    var certs = store.Certificates.Find(
                //        X509FindType.FindByThumbprint,
                //        thumbprint,
                //        false);
                //    idp.SigningKeys.AddConfiguredKey(certs[0]);
                //}
                //finally
                //{
                //    store.Close();
                //}

                
                return idp;
            };

        }

        public static ContractSettings GetTenantSamlSettings(Context context, int tenantId)
        {
            var contractSettings = TenantUtilities.GetContractSettings(context, tenantId);
            if (contractSettings == null
                || contractSettings.SamlCompanyCode.IsNullOrEmpty()
                || contractSettings.SamlLoginUrl.IsNullOrEmpty()
                || contractSettings.SamlThumbprint.IsNullOrEmpty())
            {
                return null;
            }
            if (!FindCert(context, contractSettings.SamlThumbprint))
            {
                return null;
            }
            return contractSettings; 
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
                if (certs[0].NotBefore.Date > today || certs[0].NotAfter.Date < today)
                {
                    new SysLogModel(context, $"Certificate expired ({certs[0].NotBefore.ToString("yyyy/MM/dd")} - {certs[0].NotAfter.ToString("yyyy/MM/dd")})");
                    return false;
                }
            }
            catch (Exception e)
            {
                new SysLogModel(context, e);
                return false;
            }
            finally
            {
                store.Close();
            }
            
            return true;
        }
    }
}