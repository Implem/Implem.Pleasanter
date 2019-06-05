using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class OutgoingMailValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEditing(Context context, SiteSettings ss)
        {
            if (!context.CanSendMail(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnSending(
            Context context, SiteSettings ss, OutgoingMailModel outgoingMailModel, out string data)
        {
            data = null;
            if (!context.CanSendMail(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (Parameters.Mail.SmtpHost == "smtp.sendgrid.net" &&
                outgoingMailModel.To == string.Empty)
            {
                return new ErrorData(type: Error.Types.RequireTo);
            }
            if ((outgoingMailModel.To +
                outgoingMailModel.Cc +
                outgoingMailModel.Bcc).Trim() == string.Empty)
            {
                return new ErrorData(type: Error.Types.RequireMailAddresses);
            }
            var badTo = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: outgoingMailModel.To,
                data: out data);
            if (badTo.Type != Error.Types.None) return badTo;
            var badCc = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: outgoingMailModel.Cc,
                data: out data);
            if (badCc.Type != Error.Types.None) return badCc;
            var badBcc = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: outgoingMailModel.Bcc,
                data: out data);
            if (badBcc.Type != Error.Types.None) return badBcc;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                context: context,
                addresses: outgoingMailModel.To,
                data: out data);
            if (externalTo.Type != Error.Types.None) return externalTo;
            var externalCc = MailAddressValidators.ExternalMailAddress(
                context: context,
                addresses: outgoingMailModel.Cc,
                data: out data);
            if (externalCc.Type != Error.Types.None) return externalCc;
            var externalBcc = MailAddressValidators.ExternalMailAddress(
                context: context,
                addresses: outgoingMailModel.Bcc,
                data: out data);
            if (externalBcc.Type != Error.Types.None) return externalBcc;
            return new ErrorData(type: Error.Types.None);
        }
    }
}
