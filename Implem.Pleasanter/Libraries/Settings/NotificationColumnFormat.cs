using Implem.Libraries.Utilities;
using DiffMatchPatch;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class NotificationColumnFormat
    {
        public string Name;
        public string Prefix;
        public string Delimiter;
        public string Allow;
        public string DiffTypes;
        public string StartBracket;
        public string EndBracket;
        public string DeletePrefixSymbol;
        public string DeleteSuffixSymbol;
        public string AddPrefixSymbol;
        public string AddSuffixSymbol;
        public bool? Always;
        public ValueDisplayTypes? DisplayTypes;
        public bool? ValueOnly;
        public bool? ConsiderMultiLine;

        public enum ValueDisplayTypes
        {
            BeforeAndAfterChange = 0,
            BeforeChange = 1,
            AfterChange = 2
        }

        public NotificationColumnFormat()
        {
        }

        public NotificationColumnFormat(string columnName)
        {
            Name = $"[{columnName}]";
        }

        public string DisplayText(
            string self,
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            return update
                ? updated
                    ? !saved.IsNullOrEmpty()
                        ? GetColumnDisplayText(
                            column:column,
                            saved:saved,
                            self:self)
                        : $"{Header(column)}{Self(self)}"
                    : Always == true
                        ? $"{Header(column)}{Self(self)}"
                        : string.Empty
                : $"{Header(column)}{Self(self)}";
        }

        private string GetColumnDisplayText(Column column, string saved, string self)
        {
            switch(DisplayTypes)
            {
                case ValueDisplayTypes.BeforeChange:
                    return $"{Header(column)}{Self(saved)}";
                case ValueDisplayTypes.AfterChange:
                    return $"{Header(column)}{Self(self)}";
                case ValueDisplayTypes.BeforeAndAfterChange:
                case null:
                default:
                    return GetDiffTypes() == "DiffMatchPatch" ?
                        Self(GetDiff(column, saved, self)) :
                        $"{Header(column)}{saved}{SeperateLine(column)}{GetAllow()}{SeperateLine(column)}{Self(self)}";
            }
        }

        private string SeperateLine(Column column)
        {
            return (column.ControlType == "MarkDown" && ConsiderMultiLine != false)
                ? "\n"
                : string.Empty;
        }
        private string Header(Column column)
        {
            return ValueOnly == true
                ? string.Empty
                : $"{Prefix}{column.LabelText}{GetDelimiter()}{SeperateLine(column)}";
        }

        private string GetDelimiter()
        {
            return Strings.CoalesceEmpty(Delimiter, " : ");
        }

        private string GetAllow()
        {
            return Strings.CoalesceEmpty(Allow, " => ");
        }

        private string GetDiffTypes()
        {
            return Strings.CoalesceEmpty(DiffTypes, "standard");
        }

        private string Self(string self)
        {
            return self?.EndsWith("\n") == true
                ? self
                : self + "\n";
        }

        private string GetDiff(Column column, string saved, string self)
        {
            string startBracket = Strings.CoalesceEmpty(StartBracket, "(");
            string endBracket = Strings.CoalesceEmpty(EndBracket, ")");
            string deletePrefixSymbol = Strings.CoalesceEmpty(DeletePrefixSymbol, "-");
            string deleteSuffixSymbol = Strings.CoalesceEmpty(DeleteSuffixSymbol, "");
            string addPrefixSymbol = Strings.CoalesceEmpty(AddPrefixSymbol, "+");
            string addSuffixSymbol = Strings.CoalesceEmpty(AddSuffixSymbol, "");
            string diffText = "";
            diff_match_patch dmp = new diff_match_patch();
            dmp.Diff_Timeout = 0;
            List<Diff> results = dmp.diff_main(saved, self);
            foreach (Diff diff in results)
            {
                string start = diff.text.StartsWith("\n") ? "\n" + startBracket : startBracket;
                string end = diff.text.EndsWith("\n") ? endBracket + "\n" : endBracket;
                switch (diff.operation)
                {
                    case Operation.EQUAL:
                        diffText += diff.text;
                        break;
                    case Operation.DELETE:
                        diffText += $"{start}{deletePrefixSymbol}{diff.text.Trim('\n')}{deleteSuffixSymbol}{end}";
                        break;
                    case Operation.INSERT:
                        diffText += $"{start}{addPrefixSymbol}{diff.text.Trim('\n')}{addSuffixSymbol}{end}";
                        break;
                }
            }
            return $"{Header(column)}{Self(diffText)}";
        }
    }
}