using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class MailAddressValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData BadMailAddress(Context context, string addresses)
        {
            var data = Libraries.Mails.Addresses.BadAddress(
                context: context,
                addresses: addresses);
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
        public static ErrorData ExternalMailAddress(Context context, string addresses)
        {
            var data = Libraries.Mails.Addresses.ExternalMailAddress(
                context: context,
                addresses: addresses);
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
