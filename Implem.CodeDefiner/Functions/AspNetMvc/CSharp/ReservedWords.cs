using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class ReservedWords
    {
        internal static string ValidName(string valiableName)
        {
            return valiableName
                .Replace(" ", "_space_")
                .Replace(">", "_")
                .Replace(".", "_dot_")
                .Replace("#", "_sharp_")
                .Replace(",", "_comma_")
                .Replace(":", "_colon_")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("-", "_")
                .Replace("+", "_plus_")
                .Replace("=", "_equal_")
                .Replace("^", "_caret_")
                .Replace("\"", "_yen_")
                .Replace("*", "_asterisk_")
                .Replace("@", "_atmark_");
        }

        internal static string EscapeReservedWord(
            this string word, string additionalPrefix = "_", string additionalSuffix = "")
        {
            return CsReservedWordArray.Contains(word)
                ? additionalPrefix + word + additionalSuffix
                : word;
        }

        private static readonly string[] CsReservedWordArray =
        { "abstract", "as", "async", "await", "base", "bool", "break", "byte", "case", "catch", "char",
            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double",
            "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
            "namespace", "new", "null", "object", "operator", "out", "override", "params", "private",
            "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof",
            "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "volatile", "void",
            "while", "add", "dynamic", "get", "partial", "remove", "set", "value", "var", "where", "yield"
        };
    }
}
