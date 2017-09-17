using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
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
            try
            {
                var directorySearcher = DirectorySearcher(loginId, password);
                directorySearcher.Filter = "({0}={1})"
                    .Params(Parameters.Authentication.LdapSearchProperty, loginId);
                var searchResult = directorySearcher.FindOne();
                if (searchResult == null) return false;
                var entry = searchResult.Entry(loginId, password);
                UpdateOrInsert(loginId, entry);
                return true;
            }
            catch (Exception e)
            {
                new SysLogModel(e);
                return false;
            }
        }

        public static void UpdateOrInsert(string loginId)
        {
            var searchRoot = new DirectoryEntry(Parameters.Authentication.LdapSearchRoot);
            var directorySearcher = new DirectorySearcher(searchRoot);
            directorySearcher.Filter = "({0}={1})".Params(
                Parameters.Authentication.LdapSearchProperty,
                loginId.Split_2nd('\\'));
            var searchResult = directorySearcher.FindOne();
            var entry = new DirectoryEntry(searchResult.Path);
            UpdateOrInsert(loginId, entry);
        }

        private static void UpdateOrInsert(string loginId, DirectoryEntry entry)
        {
            var deptCode = entry.Property(Parameters.Authentication.LdapDeptCode);
            var deptName = entry.Property(Parameters.Authentication.LdapDeptName);
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var userCode = entry.Property(Parameters.Authentication.LdapUserCode);
            var name = Name(loginId, entry);
            var mailAddress = entry.Property(Parameters.Authentication.LdapMailAddress);
            var statements = new List<SqlStatement>();
            if (deptExists)
            {
                statements.Add(Rds.UpdateOrInsertDepts(
                    param: Rds.DeptsParam()
                        .TenantId(Parameters.Authentication.LdapTenantId)
                        .DeptCode(deptCode)
                        .DeptName(deptName),
                    where: Rds.DeptsWhere().DeptCode(deptCode)));
            }
            statements.Add(Rds.UpdateOrInsertUsers(
                param: Rds.UsersParam()
                    .TenantId(Parameters.Authentication.LdapTenantId)
                    .LoginId(loginId)
                    .UserCode(userCode)
                    .Name(name)
                    .DeptId(
                        sub: Rds.SelectDepts(
                            column: Rds.DeptsColumn().DeptId(),
                            where: Rds.DeptsWhere().DeptCode(deptCode)),
                        _using: deptExists)
                    .DeptId(0, _using: !deptExists),
                where: Rds.UsersWhere().LoginId(loginId)));
            if (!mailAddress.IsNullOrEmpty())
            {
                statements.Add(Rds.UpdateOrInsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(loginId)))
                        .OwnerType("Users")
                        .MailAddress(mailAddress),
                    where: Rds.MailAddressesWhere()
                        .OwnerType("Users")
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(loginId)))));
            }
            statements.Add(StatusUtilities.UpdateStatus(
                StatusUtilities.Types.DeptsUpdated,
                tenantId: Parameters.Authentication.LdapTenantId));
            statements.Add(StatusUtilities.UpdateStatus(
                StatusUtilities.Types.UsersUpdated,
                tenantId: Parameters.Authentication.LdapTenantId));
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
        }

        public static void Sync()
        {
            Parameters.Authentication.LdapSyncPatterns?.ForEach(pattern =>
                Sync(pattern));
        }

        private static void Sync(string pattern)
        {
            try
            {
                var directorySearcher = DirectorySearcher(
                    Parameters.Authentication.LdapSyncUser,
                    Parameters.Authentication.LdapSyncPassword);
                directorySearcher.Filter = pattern;
                var results = directorySearcher.FindAll();
                foreach (SearchResult result in results)
                {
                    var entry = result.Entry(
                        Parameters.Authentication.LdapSyncUser,
                        Parameters.Authentication.LdapSyncPassword);
                    UpdateOrInsert(
                        entry.Property(Parameters.Authentication.LdapSearchProperty),
                        entry);
                }
            }
            catch (Exception e)
            {
                new SysLogModel(e);
            }
        }

        private static DirectorySearcher DirectorySearcher(string loginId, string password)
        {
            return new DirectorySearcher(loginId != null && password != null
                ? new DirectoryEntry(Parameters.Authentication.LdapSearchRoot, loginId, password)
                : new DirectoryEntry(Parameters.Authentication.LdapSearchRoot));
        }

        private static DirectoryEntry Entry(
            this SearchResult searchResult, string loginId, string password)
        {
            return loginId != null && password != null
                ? new DirectoryEntry(searchResult.Path, loginId, password)
                : new DirectoryEntry(searchResult.Path);
        }

        private static string Property(this DirectoryEntry entry, string propertyName)
        {
            if (!propertyName.IsNullOrEmpty())
            {
                try
                {
                    return entry.Properties[propertyName][0].ToString();
                }
                catch (Exception e) { new SysLogModel(e); }
            }
            return string.Empty;
        }

        private static string Name(string loginId, DirectoryEntry entry)
        {
            var name = "{0} {1}".Params(
                entry.Property(Parameters.Authentication.LdapLastName),
                entry.Property(Parameters.Authentication.LdapFirstName));
            return name != " "
                ? name
                : loginId;
        }
    }
}