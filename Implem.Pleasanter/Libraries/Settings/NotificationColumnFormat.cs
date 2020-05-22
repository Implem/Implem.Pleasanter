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
                        ? $"{Header(column)}{saved}{GetAllow()}{Self(self)}"
                        : $"{Header(column)}{Self(self)}"
                    : Always == true
                        ? $"{Header(column)}{Self(self)}"
                        : string.Empty
                : $"{Header(column)}{Self(self)}";
        }

        private string Header(Column column)
        {
            return $"{Prefix}{column.LabelText}{GetDelimiter()}";
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