using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Actions
    {
        public static string GridRows(Context context)
        {
            switch (context.Action)
            {
                case "index":
                    return "GridRows";
                default:
                    return context.Action;
            }
        }
    }
}