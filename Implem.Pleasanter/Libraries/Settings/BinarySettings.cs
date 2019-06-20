using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class BinarySettings
    {
        public bool InitialValue(Context context)
        {
            return this.ToJson() == "[]";
        }
    }
}