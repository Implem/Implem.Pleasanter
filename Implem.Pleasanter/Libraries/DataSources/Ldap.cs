using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.DirectoryServices;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Ldap
    {
        public static bool Authenticate()
        {
            var loginId = Forms.Data("Users_LoginId");
            var password = Forms.Data("Users_Password");
            try
            {
                var searchRoot = new DirectoryEntry(
                    Def.AuthenticationParameters.LdapSearchRoot, loginId, password);
                var directorySearcher = new DirectorySearcher(searchRoot);
                directorySearcher.Filter = "({0}={1})"
                    .Params(Def.AuthenticationParameters.LdapSearchProperty, loginId);
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

        private static void UpdateOrInsert(string loginId, DirectoryEntry entry)
        {
            var deptCode = entry.Property(Def.AuthenticationParameters.LdapDeptCode);
            var deptName = entry.Property(Def.AuthenticationParameters.LdapDeptName);
            var userCode = entry.Property(Def.AuthenticationParameters.LdapUserCode);
            var firstName = entry.Property(Def.AuthenticationParameters.LdapFirstName);
            var lastName = entry.Property(Def.AuthenticationParameters.LdapLastName);
            var mailAddress = entry.Property(Def.AuthenticationParameters.LdapMailAddress);
            Rds.ExecuteNonQuery(statements: new SqlStatement[]
            {
                    Rds.UpdateOrInsertDepts(
                        param: Rds.DeptsParam()
                            .TenantId(Def.AuthenticationParameters.LdapTenantId)
                            .ParentDeptId(0)
                            .DeptCode(deptCode)
                            .DeptName(deptName),
                        where: Rds.DeptsWhere().DeptCode(deptCode)),
                    Rds.UpdateOrInsertUsers(
                        param: Rds.UsersParam()
                            .TenantId(Def.AuthenticationParameters.LdapTenantId)
                            .LoginId(loginId)
                            .UserCode(userCode)
                            .FirstName(firstName)
                            .LastName(lastName)
                            .DeptId(sub: Rds.SelectDepts(
                                column: Rds.DeptsColumn().DeptId(),
                                where: Rds.DeptsWhere().DeptCode(deptCode))),
                        where: Rds.UsersWhere().LoginId(loginId)),
                    Rds.UpdateOrInsertMailAddresses(
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
                                where: Rds.UsersWhere().LoginId(loginId))))
            });
        }

        private static string Property(this DirectoryEntry entry, string propertyName)
        {
            if (propertyName != "N/A")
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