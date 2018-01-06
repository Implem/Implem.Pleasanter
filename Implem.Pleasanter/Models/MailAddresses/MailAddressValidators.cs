using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class MailAddressValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types BadMailAddress(string mailAddresses, out string data)
        {
            data = Libraries.Mails.Addresses.BadAddress(mailAddresses);
            if (data != string.Empty)
            {
                return Error.Types.BadMailAddress;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types ExternalMailAddress(string mailAddresses, out string data)
        {
            data = Libraries.Mails.Addresses.ExternalMailAddress(mailAddresses);
            if (data != string.Empty)
            {
                return Error.Types.ExternalMailAddress;
            }
            return Error.Types.None;
        }
    }
}
