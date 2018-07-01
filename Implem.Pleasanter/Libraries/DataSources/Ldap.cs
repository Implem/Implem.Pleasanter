using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Ldap
    {
        public static bool Authenticate(string loginId, string password)
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
                    new SysLogModel(e);
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
                    new SysLogModel(e);
                    return false;
                }
                if (result != null)
                {
                    entry = result.Entry(loginId, password);
                    UpdateOrInsert(
                        loginId: loginId,
                        entry: entry,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                    return true;
                }
            }
            return false;
        }

        public static void UpdateOrInsert(string loginId)
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
            string loginId,
            DirectoryEntry entry,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptCode = entry.Property(ldap.LdapDeptCode, ldap.LdapDeptCodePattern);
            var deptName = entry.Property(ldap.LdapDeptName, ldap.LdapDeptNamePattern);
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var userCode = entry.Property(ldap.LdapUserCode, ldap.LdapUserCodePattern);
            var name = Name(loginId, entry, ldap);
            var mailAddress = entry.Property(ldap.LdapMailAddress, ldap.LdapMailAddressPattern);
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
                .DeptId(0, _using: !deptExists)
                .LdapSearchRoot(ldap.LdapSearchRoot)
                .SynchronizedTime(synchronizedTime);
            ldap.LdapExtendedAttributes?.ForEach(attribute =>
                param.Add(
                    $"[Users].[{attribute.ColumnName}]",
                    attribute.ColumnName,
                    entry.Property(attribute.Name, attribute.Pattern)));
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
            statements.Add(StatusUtilities.UpdateStatus(
                StatusUtilities.Types.DeptsUpdated,
                tenantId: ldap.LdapTenantId));
            statements.Add(StatusUtilities.UpdateStatus(
                StatusUtilities.Types.UsersUpdated,
                tenantId: ldap.LdapTenantId));
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
        }

        public static void Sync()
        {
            var synchronizedTime = DateTime.Now;
            Parameters.Authentication.LdapParameters
                .ForEach(ldap => ldap.LdapSyncPatterns?
                    .ForEach(pattern => Sync(
                        ldap: ldap,
                        pattern: pattern,
                        synchronizedTime: synchronizedTime)));
        }

        private static void Sync(
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
                                loginId: NetBiosName(entry, ldap),
                                entry: entry,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                        else
                        {
                            UpdateOrInsert(
                                loginId: entry.Property(ldap.LdapSearchProperty, null),
                                entry: entry,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                        }
                    }
                }
                if (ldap.AutoDisable)
                {
                    Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                        param: Rds.UsersParam().Disabled(true),
                        where: Rds.UsersWhere()
                            .Disabled(false)
                            .LdapSearchRoot(ldap.LdapSearchRoot)
                            .SynchronizedTime(_operator: " is not null")
                            .SynchronizedTime(synchronizedTime, _operator: "<>")));
                }
                if (ldap.AutoEnable)
                {
                    Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                        param: Rds.UsersParam().Disabled(false),
                        where: Rds.UsersWhere()
                            .Disabled(true)
                            .LdapSearchRoot(ldap.LdapSearchRoot)
                            .SynchronizedTime(_operator: " is not null")
                            .SynchronizedTime(synchronizedTime)));
                }
            }
            catch (Exception e)
            {
                new SysLogModel(e, logs);
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

        private static string Property(this DirectoryEntry entry, string name, string pattern)
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
                catch (Exception e) { new SysLogModel(e, logs); }
            }
            return string.Empty;
        }

        private static string Name(
            string loginId, DirectoryEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            var name = "{0} {1}".Params(
                entry.Property(ldap.LdapLastName, ldap.LdapLastNamePattern),
                entry.Property(ldap.LdapFirstName, ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(DirectoryEntry entry, ParameterAccessor.Parts.Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + entry.Property("sAMAccountName", null);
        }
    }
}