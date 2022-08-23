using Implem.Pleasanter.Libraries.Requests;
using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class FormsUtilities
    {
        public static Forms Get(params KeyValue[] keyValues)
        {
            var forms = new Forms();
            foreach (var keyValue in keyValues)
            {
                forms.Add(keyValue.Key, keyValue.Value);
            }
            return forms;
        }
    }
}
