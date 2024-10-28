using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelNotificationModel : DynamicObject
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;
        public int Id;
        public Notification.Types Type;
        public string Prefix;
        public string Address;
        public string CcAddress;
        public string BccAddress;
        public string Token;
        public bool? UseCustomFormat;
        public string Format;
        public bool? Disabled;
        public string Title;
        public string Body;

        public ServerScriptModelNotificationModel(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
            Type = Notification.Types.Mail;
        }

        public ServerScriptModelNotificationModel(Context context, SiteSettings ss, int id)
        {
            Context = context;
            SiteSettings = ss;
            var notification = NotificationUtilities.Get(context: Context, ss: SiteSettings)
                ?.Where(o => o.Id == id)
                .FirstOrDefault();
            this.Id = notification.Id;
            Type = notification.Type;
            Prefix = notification.Prefix;
            Address = notification.Address;
            CcAddress = notification.CcAddress;
            BccAddress = notification.BccAddress;
            UseCustomFormat = notification.UseCustomFormat;
            Format = notification.Format;
            Title = notification.Subject;
            Body = notification.Body;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;
            switch (name)
            {
                case nameof(Notification.Id):
                    Id = value.ToInt();
                    return true;
                case nameof(Notification.Type):
                    Type = (Notification.Types)value.ToInt();
                    return true;
                case nameof(Notification.Prefix):
                    Prefix = value.ToStr();
                    return true;
                case nameof(Notification.Address):
                    Address = value.ToStr();
                    return true;
                case nameof(Notification.CcAddress):
                    CcAddress = value.ToStr();
                    return true;
                case nameof(Notification.BccAddress):
                    BccAddress = value.ToStr();
                    return true;
                case nameof(Notification.Token):
                    Token = value.ToStr();
                    return true;
                case nameof(Notification.UseCustomFormat):
                    UseCustomFormat = value.ToBool();
                    return true;
                case nameof(Notification.Format):
                    Format = value.ToStr();
                    return true;
                case nameof(Notification.Disabled):
                    Disabled = value.ToBool();
                    return true;
                case "Title":
                    Title = value.ToStr();
                    return true;
                case nameof(Notification.Body):
                    Body = value.ToStr();
                    return true;
            }
            return true;
        }

        public bool Send()
        {
            var notification = new Notification()
            {
                Id = Id,
                Type = Type,
                Prefix = Prefix,
                Address = Address,
                CcAddress = CcAddress,
                BccAddress = BccAddress,
                Token = Token,
                UseCustomFormat = UseCustomFormat,
                Format = Format,
                Disabled = Disabled,
                Body = Body
            };
            notification?.Send(
                context: Context,
                ss: SiteSettings,
                title: Title,
                body: Body);
            return true;
        }
    }
}