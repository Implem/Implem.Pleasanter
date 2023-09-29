using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    public static class DataChangeUtilities
    {
        public static Dictionary<string, string> Types(Context context)
        {
            var types = new Dictionary<string, string>()
            {
                {
                    DataChange.Types.CopyValue.ToString(),
                    Displays.CopyValue(context: context)
                },
                {
                    DataChange.Types.CopyDisplayValue.ToString(),
                    Displays.CopyDisplayValue(context: context)
                },
                {
                    DataChange.Types.InputValue.ToString(),
                    Displays.InputValue(context: context)

                },
                {
                    DataChange.Types.InputDate.ToString(),
                    Displays.InputDate(context: context)

                },
                {
                    DataChange.Types.InputDateTime.ToString(),
                    Displays.InputDateTime(context: context)

                },
                {
                    DataChange.Types.InputDept.ToString(),
                    Displays.InputDept(context: context)

                },
                {
                    DataChange.Types.InputUser.ToString(),
                    Displays.InputUser(context: context)

                }
            };
            return types;
        }

        public static Dictionary<string, string> ColumnSelectableOptions(
            Context context, SiteSettings ss)
        {
            return ss.ColumnDefinitionHash.EditorDefinitions(context: context)
                .Where(o => !o.Unique)
                .Where(o => o.ColumnName != "Ver")
                .Where(o => o.ColumnName != "Comments")
                .Where(o => o.ExtendedColumnType != "Attachments")
                .OrderBy(o => o.EditorColumn)
                .Select(o => ss.GetColumn(
                    context: context,
                    columnName: o.ColumnName))
                .ToDictionary(o => o.ColumnName, o => o.LabelText);
        }

        public static Dictionary<string, string> BaseDateTimeOptions(
            Context context, SiteSettings ss)
        {
            var ret = new Dictionary<string, string>()
            {
                { "CurrentDate", Displays.CurrentDate(context: context) },
                { "CurrentTime", Displays.CurrentTime(context: context) }
            };
            return ret
                .Concat(Def.ColumnDefinitionCollection
                    .Where(o => o.TableName == ss.ReferenceType)
                    .Where(o => o.TypeName == "datetime" || o.TypeName == "nvarchar")
                    .Where(o => o.ColumnName != "Comments")
                    .Where(o => o.ColumnName != "Timestamp")
                    .Where(o => o.ExtendedColumnType != "Attachments")
                    .OrderBy(o => o.EditorColumn)
                    .Select(o => ss.GetColumn(
                        context: context,
                        columnName: o.ColumnName))
                    .ToDictionary(o => o.ColumnName, o => o.LabelText))
                .ToDictionary(o => o.Key, o => o.Value);
        }
    }
}