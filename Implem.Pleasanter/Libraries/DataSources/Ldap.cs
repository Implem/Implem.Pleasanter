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
            DirectoryEntry entry = null;
            DirectorySearcher searcher = null;
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                try
                {
                    searcher = DirectorySearcher(loginId, password, ldap);
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
                }
                SearchResult result = null;
                searcher.Filter = $"({ldap.LdapSearchProperty}={loginId})";
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
                    entry = result.Entry(loginId, password);
                    UpdateOrInsert(
                        context: context,
                        loginId: loginId,
                        entry: entry,
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
                searcher.Filter = "({0}={1})".Params(
                    ldap.LdapSearchProperty, loginId.Split_2nd('\\'));
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
                catch (Exception)
                {
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
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var deptSettings = !ldap.LdapDeptCode.IsNullOrEmpty() && !ldap.LdapDeptName.IsNullOrEmpty();
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
                    entry.Property(
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
                directorySearcher.PageSize = 1000;
                var results = directorySearcher.FindAll();
                logs.Add("results", results.Count.ToString());
                foreach (SearchResult result in results)
                {
                    DirectoryEntry entry = result.Entry(
                        ldap.LdapSyncUser,
                        ldap.LdapSyncPassword);
                    if (Enabled(entry, ldap))
                    {
                        logs.Add("entry", entry.Path);
                        if (Authentications.Windows())
                        {
                            UpdateOrInsert(
                                context: context,
                                loginId: NetBiosName(
                                    context: context,
                                    entry: entry,
                                    ldap: ldap),
                                entry: entry,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                        else
                        {
                            UpdateOrInsert(
                                context: context,
                                loginId: entry.Property(
                                    context: context,
                                    name: ldap.LdapSearchProperty),
                                entry: entry,
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

        private static bool Enabled(DirectoryEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            var accountDisabled = 2;
            return
                !ldap.LdapExcludeAccountDisabled ||
                (entry.Properties["UserAccountControl"].Value.ToLong() & accountDisabled) == 0;
        }

        private static DirectorySearcher DirectorySearcher(
            string loginId, string password, ParameterAccessor.Parts.Ldap ldap)
        {
            return new DirectorySearcher(loginId != null && password != null
                ? new DirectoryEntry(ldap.LdapSearchRoot, loginId, password)
                : new DirectoryEntry(ldap.LdapSearchRoot));
        }

        private static DirectoryEntry Entry(
            this SearchResult searchResult, string loginId, string password)
        {
            return loginId != null && password != null
                ? new DirectoryEntry(searchResult.Path, loginId, password)
                : new DirectoryEntry(searchResult.Path);
        }

        private static string Property(
            this DirectoryEntry entry, Context context, string name, string pattern = null)
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

        public static string NetBiosName(
            Context context, DirectoryEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + entry.Property(
                context: context,
                name: "sAMAccountName");
        }
    }
}