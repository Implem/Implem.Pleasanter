using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Imports
    {
        public static string ColumnValidate(
            Context context,
            SiteSettings ss,
            IEnumerable<string> headers,
            params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (!headers.Contains(name))
                {
                    return Messages.ResponseNotIncludedRequiredColumn(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: name).LabelText).ToJson();
                }
            }
            return null;
        }

        public static string ApiColumnValidate(
            Context context,
            SiteSettings ss,
            IEnumerable<string> headers,
            params string[] columnNames)
        {
            return ServerScriptColumnValidate(
                context: context,
                ss: ss,
                headers: headers,
                columnNames: columnNames)?.Text;
        }

        public static Message ServerScriptColumnValidate(
            Context context,
            SiteSettings ss,
            IEnumerable<string> headers,
            params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (!headers.Contains(name))
                {
                    return Messages.NotIncludedRequiredColumn(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: name).LabelText);
                }
            }
            return null;
        }

        public static string Validate(Context context, Dictionary<int, string> hash, Column column)
        {
            return ResponseJson(
                context: context,
                message: ValidateMessage(
                    context: context,
                    hash: hash,
                    column: column));
        }

        public static Message ValidateMessage(
            Context context,
            Dictionary<int, string> hash,
            Column column)
        {
            foreach (var data in hash.Where(o => HasError(o.Value, column)))
            {
                return Messages.InvalidCsvData(
                    context: context,
                    data: new string[]
                    {
                        (data.Key + 2).ToString(),
                        column.LabelText
                    });
            }
            return null;
        }

        private static bool HasError(string data, Column column)
        {
            switch (column.TypeName)
            {
                case "datetime": return !Times.InRange(data.ToDateTime());
                default: return data == null;
            }
        }

        private static string ResponseJson(Context context, Message message)
        {
            return message == null
                ? null
                : new ResponseCollection(context: context)
                    .Message(message: message)
                    .ToJson();
        }

        public static Dictionary<string, Dictionary<string, string>> GetCsvHeaderSettings(Csv csv, SiteSettings ss, List<string> rows)
        {
            Dictionary<string, Dictionary<string, string>> settingsPerHeaders = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in csv.Headers.Select((header, index) => new { Header = header, Index = index }))
            {
                ss.ColumnHash.ForEach(column => {
                    if (column.Value.LabelText == item.Header)
                    {
                        settingsPerHeaders.Add(item.Header, new Dictionary<string, string> { { "ColumnName", column.Value.ColumnName }, { "Value", rows[item.Index] }, { "ValidateRequired", column.Value.ValidateRequired.ToString() }, { "ImportKey", column.Value.ImportKey.ToString() } });
                    }
                });
            }
            return settingsPerHeaders;
        }

        public static string CheckForBrankDataInValidateRequiredColumn(Dictionary<string, Dictionary<string, string>> settingsPerHeaders, Context context)
        {
            return ResponseJson(
                context: context,
                message: CheckForBrankDataInValidateRequiredColumnMessage(
                    settingsPerHeaders: settingsPerHeaders,
                    context: context));
        }

        public static Message CheckForBrankDataInValidateRequiredColumnMessage(
            Dictionary<string, Dictionary<string, string>> settingsPerHeaders,
            Context context)
        {
            Message message = null;
            settingsPerHeaders.ForEach(settingsByHeader =>
            {
                if (settingsByHeader.Value["ValidateRequired"].ToBool() && settingsByHeader.Value["Value"].IsNullOrEmpty())
                {
                    message = Messages.InvalidValidateRequiredCsvData(
                        context: context,
                        data: new string[]
                    {
                        settingsByHeader.Key
                    });
                }
            });
            return message;
        }

        public static string CheckForExistValidateRequiredColumn(List<string> csvHeaders, SiteSettings ss, Context context)
        {
            return ResponseJson(
                context: context,
                message: CheckForExistValidateRequiredColumnMessage(
                    csvHeaders: csvHeaders,
                    ss: ss,
                    context: context));
        }

        public static Message CheckForExistValidateRequiredColumnMessage(
            List<string> csvHeaders,
            SiteSettings ss,
            Context context)
        {
            Message message = null;
            ss.GridColumns.ForEach(ssGridColumn => {
                var gridColumnName = ssGridColumn;
                var gridColumnLabelText = ss.GridColumn(gridColumnName).LabelText;
                if (!csvHeaders.Contains(gridColumnLabelText) && ss.GridColumn(gridColumnName).ValidateRequired.ToBool())
                {
                    message = Messages.NotIncludedRequiredColumn(
                        context: context,
                        data: new string[]
                    {
                        gridColumnName
                    });
                }
            });
            return message;
        }
    }
}