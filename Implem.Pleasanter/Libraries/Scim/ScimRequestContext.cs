using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Libraries.Scim
{
    public class ScimRequestContext
    {
        public const string ItemKey = "Pleasanter.Scim.Context";

        public Context Context { get; set; }
        public string TokenPrefix { get; set; }
    }
}
