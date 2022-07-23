using Implem.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class NotificationColumnFormat
    {
        public string Name;
        public string Prefix;
        public string Delimiter;
        public string Allow;
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
                    return $"{Header(column)}{saved}{SeperateLine(column)}{GetAllow()}{SeperateLine(column)}{Self(self)}";
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

        private string Self(string self)
        {
            return self?.EndsWith("\n") == true
                ? self
                : self + "\n";
        }
    }
}