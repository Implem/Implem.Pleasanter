using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class OutgoingMailValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnSending(
            Permissions.Types permissionType,
            OutgoingMailModel outgoingMailModel,
            out string data)
        {
            data = null;
            if (!permissionType.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            if ((outgoingMailModel.To +
                outgoingMailModel.Cc +
                outgoingMailModel.Bcc).Trim() == string.Empty)
            {
                return Error.Types.RequireMailAddresses;
            }
            var badTo = MailAddressValidators.BadMailAddress(
                outgoingMailModel.To, out data);
            if (badTo != Error.Types.None) return badTo;
            var badCc = MailAddressValidators.BadMailAddress(
                outgoingMailModel.Cc, out data);
            if (badCc != Error.Types.None) return badCc;
            var badBcc = MailAddressValidators.BadMailAddress(
                outgoingMailModel.Bcc, out data);
            if (badBcc != Error.Types.None) return badBcc;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                outgoingMailModel.To, out data);
            if (externalTo != Error.Types.None) return externalTo;
            var externalCc = MailAddressValidators.ExternalMailAddress(
                outgoingMailModel.Cc, out data);
            if (externalCc != Error.Types.None) return externalCc;
            var externalBcc = MailAddressValidators.ExternalMailAddress(
                outgoingMailModel.Bcc, out data);
            if (externalBcc != Error.Types.None) return externalBcc;
            return Error.Types.None;
        }
    }
}
