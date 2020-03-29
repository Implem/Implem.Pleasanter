using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Ldap
    {
        public static bool Authenticate(Context context, string loginId, string password)
        {
            DirectorySearcher searcher = null;
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                try
                {
                    searcher = DirectorySearcher(
                        loginId: ldap.LdapLoginPattern != null
                            ? ldap.LdapLoginPattern.Replace("{loginId}", loginId)
                            : loginId,
                        password: password,
                        ldap: ldap);
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
                }
                SearchResult result = null;
                searcher.Filter = ldap.LdapSearchPattern != null
                    ? ldap.LdapSearchPattern.Replace("{loginId}", loginId)
                    : $"({ldap.LdapSearchProperty}={loginId})";
                try
                {
                    result = searcher.FindOne();
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
                }
                if (result != null)
                {
                    UpdateOrInsert(
                        context: context,
                        loginId: loginId,
                        result: result,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                    return true;
                }
            }
            return false;
        }

        public static void UpdateOrInsert(Context context, string loginId)
        {
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                var root = new DirectoryEntry(ldap.LdapSearchRoot);
                var searcher = new DirectorySearcher(root);
                searcher.Filter = ldap.LdapSearchPattern != null
                    ? ldap.LdapSearchPattern.Replace("{loginId}", loginId)
                    : $"({ldap.LdapSearchProperty}={loginId.Split_2nd('\\')})";
                try
                {
                    SearchResult result = searcher.FindOne();
                    UpdateOrInsert(
                        context: context,
                        loginId: loginId,
                        result: result,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                }
            }
        }

        private static void UpdateOrInsert(
            Context context,
            string loginId,
            SearchResult result,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptCode = result.Property(
                context: context,
                name: ldap.LdapDeptCode,
                pattern: ldap.LdapDeptCodePattern);
            var deptName = result.Property(
                context: context,
                name: ldap.LdapDeptName,
                pattern: ldap.LdapDeptNamePattern);
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var deptSettings = !ldap.LdapDeptCode.IsNullOrEmpty() && !ldap.LdapDeptName.IsNullOrEmpty();
            var userCode = result.Property(
                context: context,
                name: ldap.LdapUserCode,
                pattern: ldap.LdapUserCodePattern);
            var name = Name(
                context: context,
                loginId: loginId,
                result: result,
                ldap: ldap);
            var mailAddress = result.Property(
                context: context,
                name: ldap.LdapMailAddress,
                pattern: ldap.LdapMailAddressPattern);
            var statements = new List<SqlStatement>();
            if (deptExists)
            {
                statements.Add(Rds.UpdateOrInsertDepts(
                    param: Rds.DeptsParam()
                        .TenantId(ldap.LdapTenantId)
                        .DeptCode(deptCode)
                        .DeptName(deptName),
                    where: Rds.DeptsWhere().DeptCode(deptCode)));
            }
            var param = Rds.UsersParam()
                .TenantId(ldap.LdapTenantId)
                .LoginId(loginId)
                .UserCode(userCode)
                .Name(name)
                .DeptId(
                    sub: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        where: Rds.DeptsWhere().DeptCode(deptCode)),
                    _using: deptExists)
                .DeptId(0, _using: deptSettings && !deptExists)
                .LdapSearchRoot(ldap.LdapSearchRoot)
                .SynchronizedTime(synchronizedTime);
            ldap.LdapExtendedAttributes?.ForEach(attribute =>
                param.Add(
                    $"[Users].[{attribute.ColumnName}]",
                    attribute.ColumnName,
                    result.Property(
                        context: context,
                        name: attribute.Name,
                        pattern: attribute.Pattern)));
            statements.Add(Rds.UpdateOrInsertUsers(
                param: param,
                where: Rds.UsersWhere().LoginId(loginId),
                addUpdatorParam: false,
                addUpdatedTimeParam: false));
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
            var ss = new SiteSettings();
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.DeptsUpdated));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.UsersUpdated));
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
        }

        public static void Sync(Context context)
        {
            var synchronizedTime = DateTime.Now;
            Parameters.Authentication.LdapParameters
                .ForEach(ldap => ldap.LdapSyncPatterns?
                    .ForEach(pattern =>
                    {
                        Sync(
                          context: context,
                          ldap: ldap,
                          pattern: pattern,
                          synchronizedTime: synchronizedTime);
                        if (ldap.AutoDisable)
                        {
                            Rds.ExecuteNonQuery(
                                context: context,
                                statements: Rds.UpdateUsers(
                                    param: Rds.UsersParam().Disabled(true),
                                    where: Rds.UsersWhere()
                                        .Disabled(false)
                                        .LdapSearchRoot(ldap.LdapSearchRoot)
                                        .SynchronizedTime(_operator: " is not null")
                                        .SynchronizedTime(synchronizedTime, _operator: "<>"),
                                    addUpdatorParam: false,
                                    addUpdatedTimeParam: false));
                        }
                        if (ldap.AutoEnable)
                        {
                            Rds.ExecuteNonQuery(
                                context: context,
                                statements: Rds.UpdateUsers(
                                    param: Rds.UsersParam().Disabled(false),
                                    where: Rds.UsersWhere()
                                        .Disabled(true)
                                        .LdapSearchRoot(ldap.LdapSearchRoot)
                                        .SynchronizedTime(_operator: " is not null")
                                        .SynchronizedTime(synchronizedTime),
                                    addUpdatorParam: false,
                                    addUpdatedTimeParam: false));
                        }
                    }));
        }

        private static void Sync(
            Context context,
            ParameterAccessor.Parts.Ldap ldap,
            string pattern,
            DateTime synchronizedTime)
        {
            var logs = new Logs()
            {
                new Log("pattern", pattern)
            };
            try
            {
                var directorySearcher = DirectorySearcher(
                    ldap.LdapSyncUser,
                    ldap.LdapSyncPassword,
                    ldap);
                directorySearcher.Filter = pattern;
                if (ldap.LdapSyncPageSize == 0)
                {
                    directorySearcher.PageSize = 1000;
                }
                else if (ldap.LdapSyncPageSize > 0)
                {
                    directorySearcher.PageSize = ldap.LdapSyncPageSize;
                }
                var results = directorySearcher.FindAll();
                logs.Add("results", results.Count.ToString());
                foreach (SearchResult result in results)
                {
                    if (Enabled(result, ldap))
                    {
                        logs.Add("result", result.Path);
                        if (Authentications.Windows(context: context))
                        {
                            UpdateOrInsert(
                                context: context,
                                loginId: NetBiosName(
                                    context: context,
                                    result: result,
                                    ldap: ldap),
                                result: result,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                        else
                        {
                            UpdateOrInsert(
                                context: context,
                                loginId: result.Property(
                                    context: context,
                                    name: ldap.LdapSearchProperty),
                                result: result,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new SysLogModel(context: context, e: e, logs: logs);
            }
        }

        private static bool Enabled(SearchResult result, ParameterAccessor.Parts.Ldap ldap)
        {
            var accountDisabled = 2;
            return !ldap.LdapExcludeAccountDisabled
                || !result.Properties.Contains("UserAccountControl")
                || (result.Properties["UserAccountControl"].ToLong() & accountDisabled) == 0;
        }

        private static DirectorySearcher DirectorySearcher(
            string loginId, string password, ParameterAccessor.Parts.Ldap ldap)
        {
            if (!Enum.TryParse(ldap.LdapAuthenticationType, out AuthenticationTypes type)
                || !Enum.IsDefined(typeof(AuthenticationTypes), type))
            {
                type = AuthenticationTypes.Secure;
            }
            return new DirectorySearcher(loginId == null || password == null
                ? new DirectoryEntry(ldap.LdapSearchRoot)
                : new DirectoryEntry(ldap.LdapSearchRoot, loginId, password, type));
        }

        private static string Property(
            this SearchResult result, Context context, string name, string pattern = null)
        {
            if (!name.IsNullOrEmpty())
            {
                try
                {
                    if (result.Properties.Contains(name))
                    {
                        if (pattern.IsNullOrEmpty())
                        {
                            return result.Properties[name][0].ToString();
                        }
                        foreach (object obj in result.Properties[name])
                        {
                            if (obj.ToString().RegexExists(pattern))
                            {
                                return obj.ToString().RegexFirst(pattern);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    var logs = new Logs()
                    {
                        new Log("result", result.Path),
                        new Log("propertyName", name)
                    };
                    new SysLogModel(context: context, e: e, logs: logs);
                }
            }
            return string.Empty;
        }

        private static string Name(
            Context context,
            string loginId,
            SearchResult result,
            ParameterAccessor.Parts.Ldap ldap)
        {
            var name = "{0} {1}".Params(
                result.Property(
                    context: context,
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                result.Property(
                    context: context,
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(
            Context context, SearchResult result, ParameterAccessor.Parts.Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + result.Property(
                context: context,
                name: "sAMAccountName");
        }
    }
}