using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using Implem.SupportTools.Common;

namespace Implem.SupportTools.LdapSyncTester
{
    public class LdapResult
    {
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string Enabled { get; set; }
        public string MailAddress { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string UserCode { get; set; }
        public string ExtendedAttributes { get; set; }

        public static string CsvHeader
        {
            get => $"{nameof(LoginId)},{nameof(Name)},{nameof(Enabled)},{nameof(MailAddress)},"+
                $"{nameof(DeptCode)},{nameof(DeptName)},{nameof(UserCode)},{nameof(ExtendedAttributes)}";
        }

        public string ToCsv()
        {
            return $"{ToCsvField(LoginId)},{ToCsvField(Name)},{ToCsvField(Enabled)},{ToCsvField(MailAddress)}," +
                $"{ToCsvField(DeptCode)},{ToCsvField(DeptName)},{ToCsvField(UserCode)},{ToCsvField(ExtendedAttributes)}";
        }

        private string ToCsvField(object value)
        {
            var field = value?.ToString() ?? "";
            if (field.Any(c => new[] { '"', ',', '\n', '\r' }.Contains(c)))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }


    }
    public class Authentication
    {
        public string Provider;
        public string ServiceId;
        public string ExtensionUrl;
        public int PasswordExpirationPeriod;
        public bool RejectUnregisteredUser;
        public List<Ldap> LdapParameters;
    }

    public class Ldap
    {
        public string LdapSearchRoot;
        public string LdapSearchProperty;
        public string NetBiosDomainName;
        public int LdapTenantId;
        public string LdapDeptCode;
        public string LdapDeptCodePattern;
        public string LdapDeptName;
        public string LdapDeptNamePattern;
        public string LdapUserCode;
        public string LdapUserCodePattern;
        public string LdapFirstName;
        public string LdapFirstNamePattern;
        public string LdapLastName;
        public string LdapLastNamePattern;
        public string LdapMailAddress;
        public string LdapMailAddressPattern;
        public List<LdapExtendedAttribute> LdapExtendedAttributes;
        public List<string> LdapSyncPatterns;
        public bool LdapExcludeAccountDisabled;
        public bool AutoDisable;
        public bool AutoEnable;
        public string LdapSyncUser;
        public string LdapSyncPassword;
    }

    public class LdapExtendedAttribute
    {
        public string Name;
        public string Pattern;
        public string ColumnName;
    }

    public static class LdapSync
    {
        private static ILogger logger;

        public static IEnumerable<LdapResult> Sync(Authentication auth, ILogger logger)
        {
            LdapSync.logger = logger;

            foreach (var ldap in auth.LdapParameters)
            {
                foreach (var pattern in ldap.LdapSyncPatterns)
                {
                    foreach (var entry in Sync(ldap, pattern))
                    {
                        yield return entry;
                    }
                }
            }
        }

        private static IEnumerable<LdapResult> Sync(Ldap ldap, string pattern)
        {
            logger.Info(nameof(LdapSyncTester), $"processing... sync pattern ={pattern}");
            var directorySearcher = DirectorySearcher(
                ldap.LdapSyncUser,
                ldap.LdapSyncPassword,
                ldap);
            directorySearcher.Filter = pattern;
            directorySearcher.PageSize = 1000;
            var results = directorySearcher.FindAll();
            foreach (SearchResult result in results)
            {
                DirectoryEntry entry = result.Entry(
                    ldap.LdapSyncUser,
                    ldap.LdapSyncPassword);
                
                    string loginId = entry.Property(ldap.LdapSearchProperty);
                    logger.Info(nameof(LdapSyncTester), $"processing...({loginId})");

                    var ldapResult = new LdapResult()
                    {
                        LoginId = entry.Property(ldap.LdapSearchProperty),
                        Name = Name(loginId, entry, ldap: ldap),
                        Enabled = Enabled(entry, ldap)?"True":"False",
                        MailAddress = entry.Property(ldap.LdapMailAddress, ldap.LdapMailAddressPattern),
                        UserCode = entry.Property(ldap.LdapUserCode, ldap.LdapUserCodePattern),
                        DeptCode = entry.Property(ldap.LdapDeptCode, ldap.LdapDeptCodePattern),
                        DeptName = entry.Property(ldap.LdapDeptName, ldap.LdapDeptNamePattern),
                        ExtendedAttributes = string.Join(", ", ldap.LdapExtendedAttributes?.Select(attr => entry.Property(attr.Name, attr.Pattern))?? new string[0] ),
                    };
                    yield return ldapResult;
                
            }
        }

        private static DirectorySearcher DirectorySearcher(
            string loginId, string password, Ldap ldap)
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

        private static bool Enabled(DirectoryEntry entry, Ldap ldap)
        {
            var accountDisabled = 2;
            return
                !ldap.LdapExcludeAccountDisabled ||
                (entry.Properties["UserAccountControl"].Value.ToLong() & accountDisabled) == 0;
        }
        private static string Property(
            this DirectoryEntry entry, string name, string pattern = null)
        {

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    return entry.Properties[name].Value != null
                        ? string.IsNullOrEmpty(pattern)
                            ? entry.Properties[name].Value.ToString()
                            : entry.Properties[name].Value.ToString().RegexFirst(pattern)
                        : string.Empty;
                }
                catch (Exception e)
                {
                    logger.Error(nameof(LdapSyncTester), $"Get property error: name = {name}, pattern = {pattern}", e);
                }
            }
            return string.Empty;
        }

        private static string Name(
            string loginId,
            DirectoryEntry entry,
            Ldap ldap)
        {
            var name = "{0} {1}".Params(
                entry.Property(
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                entry.Property(
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(
            DirectoryEntry entry, Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + entry.Property(name: "sAMAccountName");
        }

        public static long ToLong(this object self)
        {
            if (self != null)
            {
                long data;
                if (long.TryParse(self.ToString(), out data))
                {
                    return data;
                }
                if (self is Enum)
                {
                    return Convert.ToInt64(self);
                }
            }
            return 0;
        }

        public static string RegexFirst(
            this string self,
            string pattern,
            RegexOptions regexOptions = RegexOptions.Singleline)
        {
            foreach (Match match in self.RegexMatches(pattern, regexOptions))
            {
                return match.Value;
            }
            return string.Empty;
        }

        public static MatchCollection RegexMatches(
           this string self,
           string pattern,
           RegexOptions regexOptions = RegexOptions.Singleline)
        {
            return Regex.Matches(self, pattern, regexOptions);
        }

        public static string Params(this string format, params object[] args)
        {
            return args.Any()
                ? string.Format(format, args)
                : string.Empty;
        }
    }
}
