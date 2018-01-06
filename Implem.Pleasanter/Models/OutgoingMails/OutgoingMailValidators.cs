using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class OutgoingMailValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEditing(SiteSettings ss)
        {
            if (!ss.CanSendMail())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnSending(
            SiteSettings ss, OutgoingMailModel outgoingMailModel, out string data)
        {
            data = null;
            if (!ss.CanSendMail())
            {
                return Error.Types.HasNotPermission;
            }
            if (DefinitionAccessor.Parameters.Mail.SmtpHost == "smtp.sendgrid.net" &&
                outgoingMailModel.To == string.Empty)
            {
                return Error.Types.RequireTo;
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
