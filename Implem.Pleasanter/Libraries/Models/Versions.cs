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

        public enum AfterCreateActionTypes : int
        {
            Default = 0,
            ReturnToList = 1,
            OpenNewEditor = 2,
        }

        public enum AfterUpdateActionTypes : int
        {
            Default = 0,
            ReturnToList = 1,
            MoveToNextRecord = 2,
        }

        public static bool VerUp(Context context, SiteSettings ss, bool verUp)
        {
            return verUp ||
                (ss.SiteId > 0
                && !(ss.IsSite(context: context) && context.Action == "update")
                && ss.AutoVerUpType == AutoVerUpTypes.Always);
        }

        public static bool MustVerUp(
            Context context,
            SiteSettings ss,
            BaseModel baseModel,
            bool isSite = false)
        {
            if (!isSite)
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
                (baseModel.UpdatedTime?.DifferentDate(context: context) ?? false);
        }
    }
}