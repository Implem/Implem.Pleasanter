﻿using Implem.DefinitionAccessor;
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
            Column column,
            Dictionary<string, string> data,
            SqlStatement updateModel)
        {
            var now = DateTime.Now;
            var format = Format(
                column: column,
                data: data);
            var key = Key(
                context: context,
                ss: ss,
                column: column,
                data: data,
                now: now,
                format: format);
            var success = false;
            string value;
            do
            {
                var number = NextNumber(
                    context: context,
                    ss: ss,
                    column: column,
                    key: key);
                value = Value(
                    context: context,
                    data: data,
                    now: now,
                    format: format,
                    number: number);
                updateModel.SqlParamCollection = Rds.IssuesParam().Add(
                    columnBracket: $"\"{column.ColumnName}\"",
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
                                .ColumnName(column.ColumnName)
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
            Column column,
            Dictionary<string, string> data)
        {
            var format = column.AutoNumberingFormat;
            data?.ForEach(o => format = format.Replace($"[{o.Key}]", o.Value));
            return format;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Key(
            Context context,
            SiteSettings ss,
            Column column,
            Dictionary<string, string> data,
            DateTime now,
            string format)
        {
            switch (column.AutoNumberingResetType)
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
            Column column,
            string key)
        {
            if (Rds.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectAutoNumberings(
                    column: Rds.AutoNumberingsColumn()
                        .Key(),
                    where: Rds.AutoNumberingsWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(ss.SiteId)
                        .ColumnName(column.ColumnName)
                        .Key(key),
                    top: 1)) == key)
            {
                return Rds.ExecuteScalar_decimal(
                    context: context,
                    statements: Rds.SelectAutoNumberings(
                        column: Rds.AutoNumberingsColumn()
                            .Number(function: Sqls.Functions.Max),
                        where: Rds.AutoNumberingsWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId(ss.SiteId)
                            .ColumnName(column.ColumnName)
                            .Key(key))) + column.AutoNumberingStep.ToDecimal();
            }
            else
            {
                return column.AutoNumberingDefault.ToDecimal();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Value(
            Context context,
            Dictionary<string, string> data,
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
