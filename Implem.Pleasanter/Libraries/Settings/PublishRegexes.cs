using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.Settings
{
    internal static partial class PublishRegexes
    {
        [GeneratedRegex(
            @"^\[\[(Users.*|Depts|Groups\*?)\]\]$",
            RegexOptions.Singleline)]
        internal static partial Regex DirectoryChoicesPattern();

        [GeneratedRegex("^[A-Za-z0-9_]+$")]
        internal static partial Regex ColumnNamePattern();
    }
}