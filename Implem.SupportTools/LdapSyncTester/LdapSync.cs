using Implem.SupportTools.Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text.RegularExpressions;
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
            get => $"{nameof(LoginId)},{nameof(Name)},{nameof(Enabled)},{nameof(MailAddress)}," +
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
        public string LdapLoginPattern;
        public string LdapSearchProperty;
        public string LdapSearchPattern;
        public string LdapAuthenticationType;
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
        public int LdapSyncPageSize;
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
            if (ldap.LdapSyncPageSize == 0)
            {
                directorySearcher.PageSize = 1000;
            }
            else if (ldap.LdapSyncPageSize > 0)
            {
                directorySearcher.PageSize = ldap.LdapSyncPageSize;
            }
            var results = directorySearcher.FindAll();
            foreach (SearchResult result in results)
            {
                string loginId = result.Property(ldap.LdapSearchProperty);
                logger.Info(nameof(LdapSyncTester), $"processing...({loginId})");

                var ldapResult = new LdapResult()
                {
                    LoginId = result.Property(ldap.LdapSearchProperty),
                    Name = Name(loginId, result, ldap: ldap),
                    Enabled = Enabled(result, ldap) ? "True" : "False",
                    MailAddress = result.Property(ldap.LdapMailAddress, ldap.LdapMailAddressPattern),
                    UserCode = result.Property(ldap.LdapUserCode, ldap.LdapUserCodePattern),
                    DeptCode = result.Property(ldap.LdapDeptCode, ldap.LdapDeptCodePattern),
                    DeptName = result.Property(ldap.LdapDeptName, ldap.LdapDeptNamePattern),
                    ExtendedAttributes = string.Join(", ", ldap.LdapExtendedAttributes?.Select(attr => result.Property(attr.Name, attr.Pattern)) ?? new string[0]),
                };
                yield return ldapResult;

            }
        }

        private static DirectorySearcher DirectorySearcher(
            string loginId, string password, Ldap ldap)
        {
            if (!Enum.TryParse<AuthenticationTypes>(ldap.LdapAuthenticationType, out AuthenticationTypes type)
                || !Enum.IsDefined(typeof(AuthenticationTypes), type))
            {
                type = AuthenticationTypes.Secure;
            }
            if (loginId == null || password == null)
            {
                type = AuthenticationTypes.Anonymous;
            }
            return new DirectorySearcher(new DirectoryEntry(ldap.LdapSearchRoot, loginId, password, type));
        }

        private static bool Enabled(SearchResult result, Ldap ldap)
        {
            var accountDisabled = 2;
            return
                !ldap.LdapExcludeAccountDisabled || !result.Properties.Contains("UserAccountControl") ||
                (result.Properties["UserAccountControl"].ToLong() & accountDisabled) == 0;
        }
        private static string Property(
            this SearchResult result, string name, string pattern = null)
        {

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    if (result.Properties.Contains(name))
                    {
                        if (string.IsNullOrEmpty(pattern))
                        {
                            return result.Properties[name][0].ToString();
                        }
                        foreach (Object obj in result.Properties[name])
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
                    logger.Error(nameof(LdapSyncTester), $"Get property error: name = {name}, pattern = {pattern}", e);
                }
            }
            return string.Empty;
        }

        private static string Name(
            string loginId,
            SearchResult result,
            Ldap ldap)
        {
            var name = "{0} {1}".Params(
                result.Property(
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                result.Property(
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(
            SearchResult result, Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + result.Property(name: "sAMAccountName");
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
        public static bool RegexExists(
           this string self,
           string pattern,
           RegexOptions regexOptions = RegexOptions.Singleline)
        {
            return self.RegexMatches(pattern, regexOptions).Count > 0;
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
