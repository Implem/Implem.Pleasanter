using Implem.Pleasanter.Libraries.Requests;
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

        public static bool MustVerUp(Context context, BaseModel baseModel)
        {
            return
                baseModel.Updator.Id != context.UserId ||
                baseModel.UpdatedTime.DifferentDate(context: context);
        }
    }
}