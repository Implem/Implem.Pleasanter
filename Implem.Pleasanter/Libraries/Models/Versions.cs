using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Versions
    {
        public enum VerTypes
        {
            Latest,
            History
        }

        public enum AutoVerUpTypes : int
        {
            Default = 1,
            Always = 2,
            Disabled = 3
        }

        public static bool VerUp(Context context, SiteSettings ss, bool verUp)
        {
            return verUp ||
                (ss.SiteId > 0
                && !ss.IsSite(context: context)
                && ss.AutoVerUpType == AutoVerUpTypes.Always);
        }

        public static bool MustVerUp(Context context, SiteSettings ss, BaseModel baseModel)
        {
            if (ss.SiteId > 0 && !ss.IsSite(context: context))
            {
                switch (ss.AutoVerUpType)
                {
                    case AutoVerUpTypes.Always:
                        return true;
                    case AutoVerUpTypes.Disabled:
                        return false;
                }
            }
            return baseModel.Updator.Id != context.UserId ||
                baseModel.UpdatedTime.DifferentDate(context: context);
        }
    }
}