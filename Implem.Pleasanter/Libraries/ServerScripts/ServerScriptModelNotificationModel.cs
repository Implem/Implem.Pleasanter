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
        private readonly Notification Notification;
        private string Title;
        private string Body;

        public ServerScriptModelNotificationModel(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
            Notification = new Notification()
            {
                Type = Notification.Types.Mail
            };
        }

        public ServerScriptModelNotificationModel(Context context, SiteSettings ss, int Id)
        {
            Context = context;
            SiteSettings = ss;
            Notification = NotificationUtilities.Get(context: Context, ss: SiteSettings)
                ?.Where(o => o.Id == Id)
                .FirstOrDefault();
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
                case nameof(Settings.Notification.Id):
                    Notification.Id = value.ToInt();
                    return true;
                case nameof(Settings.Notification.Type):
                    Notification.Type = (Notification.Types)value.ToInt();
                    return true;
                case nameof(Settings.Notification.Prefix):
                    Notification.Prefix = value.ToStr();
                    return true;
                case nameof(Settings.Notification.Address):
                    Notification.Address = value.ToStr();
                    return true;
                case nameof(Settings.Notification.Token):
                    Notification.Token = value.ToStr();
                    return true;
                case nameof(Settings.Notification.UseCustomFormat):
                    Notification.UseCustomFormat = value.ToBool();
                    return true;
                case nameof(Settings.Notification.Format):
                    Notification.Format = value.ToStr();
                    return true;
                case nameof(Settings.Notification.Disabled):
                    Notification.Disabled = value.ToBool();
                    return true;
                case "Title":
                    Title = value.ToStr();
                    return true;
                case "Body":
                    Body = value.ToStr();
                    return true;
            }
            return true;
        }

        public bool Send()
        {
            Notification?.Send(
                context: Context,
                ss: SiteSettings,
                title: Title,
                body: Body);
            return true;
        }
    }
}