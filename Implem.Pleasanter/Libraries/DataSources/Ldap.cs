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
                var searchRoot = new DirectoryEntry(
                    Parameters.Authentication.LdapSearchRoot, loginId, password);
                var directorySearcher = new DirectorySearcher(searchRoot);
                directorySearcher.Filter = "({0}={1})"
                    .Params(Parameters.Authentication.LdapSearchProperty, loginId);
                var searchResult = directorySearcher.FindOne();
                if (searchResult == null) return false;
                var entry = new DirectoryEntry(searchResult.Path, loginId, password);
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
            var firstName = entry.Property(Parameters.Authentication.LdapFirstName);
            var lastName = entry.Property(Parameters.Authentication.LdapLastName);
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
                    .Name(lastName + " " + firstName)
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
    }
}