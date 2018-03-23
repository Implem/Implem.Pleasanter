using Implem.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class BinarySettings
    {
        public bool InitialValue()
        {
            return this.ToJson() == "[]";
        }
    }
}