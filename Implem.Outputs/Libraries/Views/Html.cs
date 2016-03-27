namespace Implem.Pleasanter.Libraries.Views
{
    public static class Html
    {
        public static HtmlBuilder Builder()
        {
            return new HtmlBuilder();
        }

        public static HtmlAttributes Attributes()
        {
            return new HtmlAttributes();
        }
    }
}