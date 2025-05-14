#pragma warning disable CA1416
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
using Novell.Directory.Ldap;
using System.DirectoryServices;
using Implem.ParameterAccessor.Parts;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class LdapNovellDs
    {
        public static bool Authenticate(Context context, string loginId, string password)
        {
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                try
                {
                    using (var con = LdapConnection(
                        ldap.LdapLoginPattern != null
                            ? ldap.LdapLoginPattern.Replace("{loginId}", loginId)
                            : loginId,
                        password,
                        ldap))
                    {
                        var entry = con.Search(
                            con.DN(ldap),
                            Novell.Directory.Ldap.LdapConnection.ScopeSub,
                            ldap.LdapSearchPattern != null
                                ? ldap.LdapSearchPattern.Replace("{loginId}", loginId)
                                : $"({ldap.LdapSearchProperty}={loginId})",
                            null,
                            false).FindOne();
                        if (entry != null)
                        {
                            con.Bind(entry.Dn, password);
                            UpdateOrInsert(
                                context: context,
                                entry: entry,
                                ldap: ldap,
                                synchronizedTime: DateTime.Now);
                            return true;
                        }
                    }
                }
                catch (LdapException e)
                {
                    if (e.LdapErrorMessage?.Contains("data 52e") != true)
                    {
                        new SysLogModel(
                            context: context,
                            e: e,
                            extendedErrorMessage: e.LdapErrorMessage?.TrimEnd('\0'));
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
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
                    var searchResult = searcher.FindOne();
                    var entry = new DirectoryEntry(searchResult.Path);
                    UpdateOrInsert(
                        context: context,
                        loginId: loginId,
                        entry: entry,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                }
                catch (LdapException e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: e.LdapErrorMessage?.TrimEnd('\0'));
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
            DirectoryEntry entry,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptCode = entry.Property(
                context: context,
                name: ldap.LdapDeptCode,
                pattern: ldap.LdapDeptCodePattern);
            var deptName = entry.Property(
                context: context,
                name: ldap.LdapDeptName,
                pattern: ldap.LdapDeptNamePattern);
            var userCode = entry.Property(
                context: context,
                name: ldap.LdapUserCode,
                pattern: ldap.LdapUserCodePattern);
            var name = Name(
                context: context,
                loginId: loginId,
                entry: entry,
                ldap: ldap);
            var mailAddress = entry.Property(
                context: context,
                name: ldap.LdapMailAddress,
                pattern: ldap.LdapMailAddressPattern);
            var attributes = ldap.LdapExtendedAttributes?
                .Select(attribute =>
                new KeyValuePair<LdapExtendedAttribute, string>(
                    attribute,
                    entry.Property(
                        context: context,
                        name: attribute.Name,
                        pattern: attribute.Pattern)))
                        .ToList();
            UpdateOrInsert(
                context: context,
                loginId: loginId,
                deptCode: deptCode,
                deptName: deptName,
                userCode: userCode,
                name: name,
                mailAddress: mailAddress,
                attributes: attributes,
                ldap: ldap,
                synchronizedTime: synchronizedTime);
        }

        private static void UpdateOrInsert(
            Context context,
            LdapEntry entry,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptCode = entry.Property(
                context: context,
                name: ldap.LdapDeptCode,
                pattern: ldap.LdapDeptCodePattern);
            var deptName = entry.Property(
                context: context,
                name: ldap.LdapDeptName,
                pattern: ldap.LdapDeptNamePattern);
            var userCode = entry.Property(
                context: context,
                name: ldap.LdapUserCode,
                pattern: ldap.LdapUserCodePattern);
            var loginId = LoginId(
                context: context,
                ldap: ldap,
                entry: entry);
            var name = Name(
                context: context,
                loginId: loginId,
                entry: entry,
                ldap: ldap);
            var mailAddress = entry.Property(
                context: context,
                name: ldap.LdapMailAddress,
                pattern: ldap.LdapMailAddressPattern);
            var attributes = ldap.LdapExtendedAttributes?
                .Select(attribute =>
                new KeyValuePair<LdapExtendedAttribute, string>(
                    attribute,
                    entry.Property(
                        context: context,
                        name: attribute.Name,
                        pattern: attribute.Pattern)))
                        .ToList();
            UpdateOrInsert(
                context: context,
                loginId: loginId,
                deptCode: deptCode,
                deptName: deptName,
                userCode: userCode,
                name: name,
                mailAddress: mailAddress,
                attributes: attributes,
                ldap: ldap,
                synchronizedTime: synchronizedTime);
        }

        private static void UpdateOrInsert(
            Context context,
            string loginId,
            string deptCode,
            string deptName,
            string userCode,
            string name,
            string mailAddress,
            List<KeyValuePair<LdapExtendedAttribute, string>> attributes,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var deptSettings = !ldap.LdapDeptCode.IsNullOrEmpty() && !ldap.LdapDeptName.IsNullOrEmpty();
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
            var exists = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere().LoginId(
                        value: context.Sqls.EscapeValue(loginId),
                        _operator: context.Sqls.LikeWithEscape))) == 1;
            var param = Rds.UsersParam()
                .TenantId(ldap.LdapTenantId)
                .LoginId(loginId, _using: !exists)
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
            attributes?.ForEach(attributeAndName =>
                param.Add(
                    $"\"{attributeAndName.Key.ColumnName}\"",
                    attributeAndName.Key.ColumnName,
                    attributeAndName.Value));
            statements.Add(Rds.UpdateOrInsertUsers(
                param: param,
                where: Rds.UsersWhere().LoginId(
                    value: context.Sqls.EscapeValue(loginId),
                    _operator: context.Sqls.LikeWithEscape),
                addUpdatorParam: false,
                addUpdatedTimeParam: false));
            if (!mailAddress.IsNullOrEmpty())
            {
                statements.Add(Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerType("Users")
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(loginId),
                                _operator: context.Sqls.LikeWithEscape)))));
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(loginId),
                                _operator: context.Sqls.LikeWithEscape)))
                        .OwnerType("Users")
                        .MailAddress(mailAddress)));
            }
            var ss = new SiteSettings();
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.DeptsUpdated));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.UsersUpdated));
            Repository.ExecuteNonQuery(
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
                            Repository.ExecuteNonQuery(
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
                            Repository.ExecuteNonQuery(
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
                using (var con = LdapConnection(
                    ldap.LdapSyncUser,
                    ldap.LdapSyncPassword,
                    ldap))
                {
                    var results = con.Search(
                        con.DN(ldap),
                        Novell.Directory.Ldap.LdapConnection.ScopeSub,
                        pattern,
                        null,
                        false,
                        new LdapSearchConstraints() { MaxResults = 0, ReferralFollowing = true });
                    logs.Add("results", results.Count.ToString());
                    while (results.HasMore())
                    {
                        var entry = results.Next();
                        if (Enabled(entry, ldap))
                        {
                            logs.Add("entry", entry.Dn);
                            UpdateOrInsert(
                                context: context,
                                entry: entry,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                    }
                }
            }
            catch (LdapException le)
            {
                logs.Add(new Log("LdapErrorMessage", le.LdapErrorMessage?.TrimEnd('\0')));
                new SysLogModel(context: context, e: le, logs: logs);
            }
            catch (Exception e)
            {
                new SysLogModel(context: context, e: e, logs: logs);
            }
        }

        private static bool Enabled(LdapEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            var accountDisabled = 2;
            if (!ldap.LdapExcludeAccountDisabled) return true;
            if (entry.GetAttributeSet().Any(o => o.Key == "userAccountControl"))
            {
                var userAccountControl = entry.GetAttribute("userAccountControl")?.StringValue;
                return userAccountControl.IsNullOrEmpty()
                    ? true
                    : (userAccountControl.ToLong() & accountDisabled) == 0;
            }
            else
            {
                return true;
            }
        }

        private static LdapConnection LdapConnection(
            string loginId, string password, ParameterAccessor.Parts.Ldap ldap)
        {
            var url = new LdapUrl(ldap.LdapSearchRoot);
            var con = new LdapConnection();
            con.Connect(url.Host, url.Port);
            if (loginId != null && password != null)
                con.Bind(loginId, password);
            else
                con.Bind(null, null);
            return con;
        }

        private static string Property(
            this DirectoryEntry entry,
            Context context,
            string name,
            string pattern = null)
        {
            var logs = new Logs()
            {
                new Log("entry", entry.Path),
                new Log("propertyName", name)
            };
            if (!name.IsNullOrEmpty())
            {
                try
                {
                    return entry.Properties[name].Value != null
                        ? pattern.IsNullOrEmpty()
                            ? entry.Properties[name].Value.ToString()
                            : entry.Properties[name].Value.ToString().RegexFirst(pattern)
                        : string.Empty;
                }
                catch (LdapException e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: e.LdapErrorMessage?.TrimEnd('\0'));
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e, logs: logs);
                }
            }
            return string.Empty;
        }

        private static string LoginId(
            Context context,
            ParameterAccessor.Parts.Ldap ldap,
            LdapEntry entry)
        {
            var loginId = Authentications.Windows(context: context)
                ? NetBiosName(
                    context: context,
                    entry: entry,
                    ldap: ldap)
                : entry.Property(
                    context: context,
                    name: ldap.LdapSearchProperty);
            return loginId;
        }

        private static string Name(
            Context context,
            string loginId,
            DirectoryEntry entry,
            ParameterAccessor.Parts.Ldap ldap)
        {
            var name = "{0} {1}".Params(
                entry.Property(
                    context: context,
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                entry.Property(
                    context: context,
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        private static string Property(
            this LdapEntry entry,
            Context context,
            string name,
            string pattern = null)
        {
            var logs = new Logs()
            {
                new Log("entry", entry.Dn),
                new Log("propertyName", name)
            };
            if (!name.IsNullOrEmpty())
            {
                try
                {
                    return entry.GetAttribute(name)?.StringValue != null
                        ? pattern.IsNullOrEmpty()
                            ? entry.GetAttribute(name).StringValue
                            : entry.GetAttribute(name).StringValue.RegexFirst(pattern)
                        : string.Empty;
                }
                catch (KeyNotFoundException)
                {
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e, logs: logs);
                }
            }
            return string.Empty;
        }

        private static string Name(
            Context context,
            string loginId,
            LdapEntry entry,
            ParameterAccessor.Parts.Ldap ldap)
        {
            var name = "{0} {1}".Params(
                entry.Property(
                    context: context,
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                entry.Property(
                    context: context,
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(
            Context context, LdapEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + entry.Property(
                context: context,
                name: "sAMAccountName");
        }

        private static string DN(this LdapConnection con, ParameterAccessor.Parts.Ldap ldap)
        {
            return (new LdapUrl(ldap.LdapSearchRoot)).GetDn();
        }

        private static LdapEntry FindOne(this ILdapSearchResults results)
        {
            return results.HasMore() ? results.Next() : null;
        }
    }
}