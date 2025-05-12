﻿using Implem.Libraries.Classes;
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
            foreach (var data in hash.Where(o => HasError(o.Value, column)))
            {
                return Messages.ResponseInvalidCsvData(
                    context: context,
                    data: new string[]
                    {
                        (data.Key + 2).ToString(),
                        column.LabelText
                    }).ToJson();
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
            string message = null;
            settingsPerHeaders.ForEach(settingsByHeader =>
            {
                if (settingsByHeader.Value["ValidateRequired"].ToBool() && settingsByHeader.Value["Value"].IsNullOrEmpty())
                {
                    message = Messages.ResponseInvalidValidateRequiredCsvData(
                        context: context,
                        data: new string[]
                    {
                        settingsByHeader.Key
                    }).ToJson();
                }
            });
            return message;
        }

        public static string CheckForExistValidateRequiredColumn(List<string> csvHeaders, SiteSettings ss, Context context)
        {
            string message = null;
            ss.GridColumns.ForEach(ssGridColumn => {
                var gridColumnName = ssGridColumn;
                var gridColumnLabelText = ss.GridColumn(gridColumnName).LabelText;
                if (!csvHeaders.Contains(gridColumnLabelText) && ss.GridColumn(gridColumnName).ValidateRequired.ToBool())
                {
                    message = Messages.ResponseNotIncludedRequiredColumn(
                        context: context,
                        data: new string[]
                    {
                        gridColumnName
                    }).ToJson();
                }
            });
            return message;
        }
    }
}