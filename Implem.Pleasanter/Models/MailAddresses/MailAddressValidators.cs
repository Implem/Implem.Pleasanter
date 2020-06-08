using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Models
{
    public static class MailAddressValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData BadMailAddress(string addresses, bool only = false)
        {
            if (only && Libraries.Mails.Addresses.GetSplitList(addresses).Count() > 1)
            {
                return new ErrorData(
                    type: Error.Types.BadMailAddress,
                    data: new string[] { addresses });
            }
            var data = Libraries.Mails.Addresses.BadAddress(addresses: addresses);
            if (data != string.Empty)
            {
                return new ErrorData(
                    type: Error.Types.BadMailAddress,
                    data: new string[] { data });
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData ExternalMailAddress(string addresses)
        {
            var data = Libraries.Mails.Addresses.ExternalMailAddress(addresses: addresses);
            if (data != string.Empty)
            {
                return new ErrorData(
                    type: Error.Types.ExternalMailAddress,
                    data: new string[] { data });
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
