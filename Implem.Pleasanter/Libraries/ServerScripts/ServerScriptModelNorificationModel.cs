using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelNorificationModel : DynamicObject
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;
        private readonly Notification notification;
        private string Title;
        private string Body;

        public ServerScriptModelNorificationModel(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
            notification = new Notification()
            {
                Type = Notification.Types.Mail
            };
        }

        public ServerScriptModelNorificationModel(Context context, SiteSettings ss, int Id)
        {
            Context = context;
            SiteSettings = ss;
            notification = NotificationUtilities.Get(context: Context, ss: SiteSettings)
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
                case nameof(Notification.Id):
                    notification.Id = value.ToInt();
                    return true;
                case nameof(Notification.Type):
                    notification.Type = (Notification.Types)value.ToInt();
                    return true;
                case nameof(Notification.Prefix):
                    notification.Prefix = value.ToStr();
                    return true;
                case nameof(Notification.Address):
                    notification.Address = value.ToStr();
                    return true;
                case nameof(Notification.Token):
                    notification.Token = value.ToStr();
                    return true;
                case nameof(Notification.UseCustomFormat):
                    notification.UseCustomFormat = value.ToBool();
                    return true;
                case nameof(Notification.Format):
                    notification.Format = value.ToStr();
                    return true;
                case nameof(Notification.Disabled):
                    notification.Disabled = value.ToBool();
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
            notification?.Send(
                context: Context,
                ss: SiteSettings,
                title: Title,
                body: Body);
            return true;
        }
    }
}