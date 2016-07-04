using Implem.Pleasanter.Libraries.Server;
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

        public static bool MustVerUp(BaseModel baseModel)
        {
            return
                baseModel.Updator.Id != Sessions.UserId() ||
                baseModel.UpdatedTime.DifferentDate();
        }
    }
}