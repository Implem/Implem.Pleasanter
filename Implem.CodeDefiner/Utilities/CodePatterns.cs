namespace Implem.CodeDefiner.Utilities
{
    internal static class CodePatterns
    {
        internal const string IdPlaceholder = "<!--.*?-->";
        internal const string Id = "[A-Za-z0-9_]+";
        internal const string ReplacementPlaceholder = "(?<=#)[^#]+?(?=#)";
    }
}
