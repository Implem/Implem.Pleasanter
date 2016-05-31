using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class MailAddresses
    {
        public static IEnumerable<string> GetEnumerable(string mailAddresses)
        {
            return mailAddresses.Split(';')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        public static string BadMailAddress(string mailAddresses)
        {
            foreach (var mailAddress in GetEnumerable(mailAddresses))
            {
                if (Get(mailAddress) == string.Empty)
                {
                    return mailAddress;
                }
            }
            return string.Empty;
        }

        public static string Get(string mailAddress)
        {
            return mailAddress.RegexFirst(
                @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}