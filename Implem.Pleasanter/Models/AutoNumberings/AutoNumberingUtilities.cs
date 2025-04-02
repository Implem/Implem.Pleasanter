using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class AutoNumberingUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ExecuteAutomaticNumbering(
            Context context,
            SiteSettings ss,
            AutoNumbering autoNumbering,
            Dictionary<string, string> data,
            SqlStatement updateModel)
        {
            if (autoNumbering.Step == 0)
            {
                throw new ArgumentException(
                    $"AutomaticNumbering 'Step=0' error autoNumbering={autoNumbering.ToJson()}, siteId={ss.SiteId}");
            }
            var now = DateTime.Now.ToLocal(context: context);
            var format = Format(
                autoNumbering: autoNumbering,
                data: data);
            var key = Key(
                autoNumbering: autoNumbering,
                now: now,
                format: format);
            var success = false;
            string value;
            var retryCnt = 5;
            do
            {
                var number = NextNumber(
                    context: context,
                    ss: ss,
                    autoNumbering: autoNumbering,
                    key: key);
                value = Value(
                    context: context,
                    now: now,
                    format: format,
                    number: number);
                updateModel.SqlParamCollection = Rds.IssuesParam().Add(
                    columnBracket: $"\"{autoNumbering.ColumnName}\"",
                    value: value);
                try
                {
                    Rds.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertAutoNumberings(param: Rds.AutoNumberingsParam()
                                .TenantId(context.TenantId)
                                .ReferenceId(ss.SiteId)
                                .ColumnName(autoNumbering.ColumnName)
                                .Key(key)
                                .Number(number)),
                            updateModel
                        });
                    success = true;
                }
                catch (System.Data.Common.DbException e)
                {
                    if (context.SqlErrors.ErrorCode(e) != context.SqlErrors.ErrorCodeDuplicatePk)
                    {
                        throw;
                    }
                    if (--retryCnt <= 0)
                    {
                        new SysLogModel(
                            context: context,
                            method: nameof(ExecuteAutomaticNumbering),
                            message: $"AutomaticNumbering retry error autoNumbering={autoNumbering.ToJson()}, siteId={ss.SiteId}",
                            sysLogType: SysLogModel.SysLogTypes.Exception);
                        throw;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            while (!success);
            return value;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Format(
            AutoNumbering autoNumbering,
            Dictionary<string, string> data)
        {
            var format = autoNumbering.Format;
            data?.ForEach(o => format = format.Replace($"[{o.Key}]", o.Value));
            return format;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Key(
            AutoNumbering autoNumbering,
            DateTime now,
            string format)
        {
            switch (autoNumbering.ResetType)
            {
                case Column.AutoNumberingResetTypes.None:
                    return "None";
                case Column.AutoNumberingResetTypes.Year:
                    return $"{now.Year}";
                case Column.AutoNumberingResetTypes.Month:
                    return $"{now.Year}/{now.Month}";
                case Column.AutoNumberingResetTypes.Day:
                    return $"{now.Year}/{now.Month}/{now.Day}";
                default:
                    var key = format;
                    foreach (System.Text.RegularExpressions.Match match in key.RegexMatches(@"(?<=\[).+?(?=\])"))
                    {
                        if (match.Value.All(o => o == 'n') || match.Value.All(o => o == 'N'))
                        {
                            key = key.Replace($"[{match.Value}]", string.Empty);
                        }
                        else
                        {
                            key = key.Replace($"[{match.Value}]", now.ToString(match.Value));
                        }
                    }
                    return key;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static decimal NextNumber(
            Context context,
            SiteSettings ss,
            AutoNumbering autoNumbering,
            string key)
        {
            var table = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectAutoNumberings(
                    column: Rds.AutoNumberingsColumn()
                        .Number(function: Sqls.Functions.Max),
                    where: Rds.AutoNumberingsWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(ss.SiteId)
                        .ColumnName(autoNumbering.ColumnName)
                        .Add(or: Rds.AutoNumberingsWhere()
                            .Key(key)
                            // Issue#1657 過去の不具合データがテーブルに残っている場合、そのデータを加味するために追加
                            .Key(raw: "''", _using: key == "None")),
                    top: 1))
                .AsEnumerable()
                .FirstOrDefault();
            return table?.IsNull("NumberMax") ?? true
                ? autoNumbering.Default.ToDecimal()
                : table.Field<decimal>("NumberMax") + autoNumbering.Step.ToDecimal();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Value(
            Context context,
            DateTime now,
            string format,
            decimal number)
        {
            var value = format;
            foreach (System.Text.RegularExpressions.Match match in value.RegexMatches(@"(?<=\[).+?(?=\])"))
            {
                if (match.Value.All(o => o == 'n'))
                {
                    string numberString = NumberString(
                        number: number,
                        match: match);
                    value = value.Replace($"[{match.Value}]", numberString);
                }
                else if (match.Value.All(o => o == 'N'))
                {
                    string numberString = NumberString(
                        number: number,
                        match: match);
                    if (numberString.Length > match.Value.Length)
                    {
                        value = value.Replace($"[{match.Value}]", Displays.Overflow(context: context));
                    }
                    else
                    {
                        value = value.Replace($"[{match.Value}]", numberString);
                    }
                }
                else
                {
                    value = value.Replace($"[{match.Value}]", now.ToString(match.Value));
                }
            }
            return value;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string NumberString(decimal number, System.Text.RegularExpressions.Match match)
        {
            var format = $"{{0:D{match.Value.Length}}}";
            return string.Format(format, number.ToLong());
        }
    }
}
