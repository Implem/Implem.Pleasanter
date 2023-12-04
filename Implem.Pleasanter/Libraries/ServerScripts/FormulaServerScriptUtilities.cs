using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public static class FormulaServerScriptUtilities
    {
        private static (string, object) ReadNameValue(string columnName, object value)
        {
            return (columnName, value);
        }

        private static (string, object) ReadNameValue(
            Context context, SiteSettings ss, string columnName, object value, List<string> mine)
        {
            return (
                columnName,
                ss?.ColumnHash.Get(columnName)?.CanRead(
                    context: context,
                    ss: ss,
                    mine: mine,
                    noCache: true) == true
                        ? value
                        : null);
        }

        public static IEnumerable<(string Name, object Value)> Values(
            Context context, SiteSettings ss, BaseItemModel model)
        {
            var mine = model?.Mine(context: context);
            var values = new List<(string, object)>
            {
                ReadNameValue(
                    columnName: nameof(model.ReadOnly),
                    value: model.ReadOnly),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.SiteId),
                    value: model.SiteId,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Title),
                    value: model.Title?.Value,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Body),
                    value: model.Body,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Ver),
                    value: model.Ver,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Creator),
                    value: model.Creator.Id,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Updator),
                    value: model.Updator.Id,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.CreatedTime),
                    value: model.CreatedTime?.Value.ToControl(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(
                            context: context,
                            columnName: nameof(model.CreatedTime))),
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.UpdatedTime),
                    value: model.UpdatedTime?.Value.ToControl(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(
                            context: context,
                            columnName: nameof(model.UpdatedTime))),
                    mine: mine)
            };
            values.AddRange(model
                .ClassHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .NumHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.Value
                        ?? (ss?.GetColumn(
                            context: context,
                            columnName: element.Key)
                                ?.Nullable == true
                                    ? (decimal?)null
                                    : 0),
                    mine: mine)));
            values.AddRange(model
                .DateHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.ToControl(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(
                            context: context,
                            columnName: element.Key)),
                    mine: mine)));
            values.AddRange(model
                .DescriptionHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .CheckHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .AttachmentsHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.ToJson(),
                    mine: mine)));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: "Comments",
                value: model.Comments?.ToJson(),
                mine: mine));
            if (model is IssueModel issueModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.IssueId),
                    value: issueModel.IssueId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.StartTime),
                    value: issueModel.StartTime.ToControl(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(
                            context: context,
                            columnName: nameof(IssueModel.StartTime))),
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.CompletionTime),
                    value: issueModel.CompletionTime.Value
                        .AddDifferenceOfDates(
                            format: ss.GetColumn(
                                context: context,
                                columnName: nameof(IssueModel.CompletionTime))?.EditorFormat,
                            minus: true)
                        .ToControl(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: nameof(IssueModel.CompletionTime))),
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.WorkValue),
                    value: issueModel.WorkValue.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.ProgressRate),
                    value: issueModel.ProgressRate.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.RemainingWorkValue),
                    value: issueModel.RemainingWorkValue.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Status),
                    value: issueModel.Status?.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Manager),
                    value: issueModel.Manager.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Owner),
                    value: issueModel.Owner.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Locked),
                    value: issueModel.Locked,
                    mine: mine));
            }
            if (model is ResultModel resultModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.ResultId),
                    value: resultModel.ResultId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Status),
                    value: resultModel.Status?.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Manager),
                    value: resultModel.Manager.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Owner),
                    value: resultModel.Owner.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Locked),
                    value: resultModel.Locked,
                    mine: mine));
            }
            return values.ToArray();
        }

        public static object Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            string formulaScript)
        {
            var data = Values(
                context: context,
                ss: ss,
                model: itemModel);
            var Model = new ExpandoObject();
            data?.ForEach(datam => ((IDictionary<string, object>)Model)[datam.Name] = datam.Value);
            formulaScript = ParseIgnoreCase(formulaScript);
            using (var engine = new ScriptEngine(debug: false))
            {
                engine.AddHostObject("model", Model);
                engine.AddHostType(typeof(FormulaServerScriptUtilities));
                var functionScripts = GetDateScript()
                    + GetDateDifScript()
                    + GetDayScript()
                    + GetDaysScript()
                    + GetHourScript()
                    + GetMinuteScript()
                    + GetMonthScript()
                    + GetNowScript(context: context)
                    + GetSecondScript()
                    + GetTodayScript(context: context)
                    + GetYearScript()
                    + GetConcatScript()
                    + GetFindScript()
                    + GetLeftScript()
                    + GetLenScript()
                    + GetLowerScript()
                    + GetMidScript()
                    + GetRightScript()
                    + GetSubstituteScript()
                    + GetTrimScript()
                    + GetUpperScript()
                    + GetAndScript()
                    + GetIfScript()
                    + GetNotScript()
                    + GetOrScript()
                    + GetWeekdayScript()
                    + GetReplaceScript()
                    + GetSearchScript()
                    + GetIfsScript()
                    + GetIsEvenScript()
                    + GetIsNumberScript()
                    + GetIsOddScript()
                    + GetIsTextScript()
                    + GetModScript()
                    + GetOddScript()
                    + GetAverageScript()
                    + GetMinScript()
                    + GetMaxScript()
                    + GetRoundScript()
                    + GetRoundUpScript()
                    + GetRoundDownScript()
                    + GetTruncScript()
                    + GetAscScript()
                    + GetJisScript()
                    + GetValueScript()
                    + GetTextScript()
                    + GetAbsScript()
                    + GetPowerScript()
                    + GetRandScript()
                    + GetSqrtScript()
                    + GetEOMonthScript();
                var value = engine.Evaluate(functionScripts + formulaScript);
                return value == Undefined.Value ? string.Empty : value;
            }
        }

        private static string ParseIgnoreCase(string script)
        {
            return script.Replace("$date(", "$DATE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$datedif(", "$DATEDIF(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$day(", "$DAY(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$days(", "$DAYS(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$hour(", "$HOUR(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$minute(", "$MINUTE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$month(", "$MONTH(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$now(", "$NOW(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$second(", "$SECOND(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$today(", "$TODAY(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$year(", "$YEAR(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$concat(", "$CONCAT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$find(", "$FIND(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$left(", "$LEFT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$len(", "$LEN(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$lower(", "$LOWER(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$mid(", "$MID(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$right(", "$RIGHT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$substitute(", "$SUBSTITUTE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$trim(", "$TRIM(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$upper(", "$UPPER(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$and(", "$AND(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$if(", "$IF(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$not(", "$NOT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$or(", "$OR(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$weekday(", "$WEEKDAY(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$replace(", "$REPLACE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$search(", "$SEARCH(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$ifs(", "$IFS(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$iseven(", "$ISEVEN(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$isnumber(", "$ISNUMBER(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$isodd(", "$ISODD(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$istext(", "$ISTEXT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$mod(", "$MOD(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$odd(", "$ODD(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$average(", "$AVERAGE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$min(", "$MIN(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$max(", "$MAX(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$round(", "$ROUND(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$roundup(", "$ROUNDUP(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$rounddown(", "$ROUNDDOWN(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$trunc(", "$TRUNC(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$asc(", "$ASC(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$jis(", "$JIS(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$value(", "$VALUE(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$text(", "$TEXT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$abs(", "$ABS(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$power(", "$POWER(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$rand(", "$RAND(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$sqrt(", "$SQRT(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$eomonth(", "$EOMONTH(", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetText(object value, string format)
        {
            if (long.TryParse(value.ToString(), out long longValue))
            {
                return longValue.ToString(format);
            }
            if (double.TryParse(value.ToString(), out double doubleValue))
            {
                return doubleValue.ToString(format);
            }
            return DateTime.Parse(value.ToString()).ToString(format
                .Replace("Y", "y")
                .Replace("D", "d")
                .Replace("AM/PM", "tt", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Returns a serial number representing the specified date
        /// </summary>
        /// <remarks>
        /// Syntax: $DATE(year, month, day)
        /// </remarks>
        private static string GetDateScript()
        {
            return @"
                function $DATE(year, month, day)
                {
                    if (isNaN(year) || isNaN(month) || isNaN(day))
                    {
                        throw 'Invalid Parameter';
                    }
                    year = Number(year);
                    month = Number(month);
                    day = Number(day);
                    if (year >= 0 && year < 1900)
                    {
                        year = 1900 + year;
                    }
                    var date = new Date(year, month - 1, day);
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getFullYear()
                        + '/' + ('0' + (date.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + date.getDate()).slice(-2);
                }";
        }

        /// <summary>
        /// Calculates the number of days, months, or years between two dates.
        /// </summary>
        /// <remarks>
        /// Syntax: $DATEDIF(firstDate, secondDate, unit)
        /// </remarks>
        private static string GetDateDifScript()
        {
            return @"
                function $DATEDIF(firstDate, secondDate, unit)
                {
                    if (firstDate == undefined || secondDate == undefined || unit == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(firstDate))
                    {
                        firstDate = new Date(Date.parse(firstDate));
                    }
                    else
                    {
                        firstDate = Number(firstDate);
                        firstDate = new Date(1900, 0, firstDate > 59 ? firstDate - 1 : firstDate);
                    }
                    if (isNaN(firstDate.getTime()) || firstDate.getFullYear() < 1900 || firstDate.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(secondDate))
                    {
                        secondDate = new Date(Date.parse(secondDate));
                    }
                    else
                    {
                        secondDate = Number(secondDate);
                        secondDate = new Date(1900, 0, secondDate > 59 ? secondDate - 1 : secondDate);
                    }
                    if (isNaN(secondDate.getTime()) || secondDate.getFullYear() < 1900 || secondDate.getFullYear() > 9999 || firstDate.getTime() > secondDate.getTime())
                    {
                        throw 'Invalid Parameter';
                    }
                    switch(unit)
                    {
                        case 'Y':
                            return secondDate.getFullYear() - firstDate.getFullYear();
                        case 'M':
                            return secondDate.getMonth() - firstDate.getMonth() + 12 * (secondDate.getFullYear() - firstDate.getFullYear());
                        case 'D':
                            firstDate = firstDate.getTime();
                            secondDate = secondDate.getTime();
                            var diff = (secondDate - firstDate) / (1000 * 3600 * 24);
                            return ((firstDate <= secondDate && secondDate <= -2203915325000)
                                || (secondDate >= firstDate && firstDate >= -2203915324000))
                                ? diff
                                : (firstDate < secondDate ? diff - 1 : diff);
                        case 'MD':
                            return secondDate.getDate() - firstDate.getDate()
                                + (secondDate.getDate() >= firstDate.getDate() ? 0 : new Date(secondDate.setDate(0)).getDate());
                        case 'YM':
                            return secondDate.getMonth() - firstDate.getMonth()
                                + (secondDate.getMonth() >= firstDate.getMonth() ? 0 : 12);
                        case 'YD':
                            if ((secondDate.getMonth() == firstDate.getMonth() && secondDate.getDate() >= firstDate.getDate())
                                || (secondDate.getMonth() > firstDate.getMonth()))
                            {
                                if (secondDate.getFullYear() > firstDate.getFullYear())
                                {
                                    secondDate.setYear(firstDate.getFullYear());
                                }
                            }
                            else
                            {
                                if (secondDate.getFullYear() - firstDate.getFullYear() > 1)
                                {
                                    secondDate.setYear(firstDate.getFullYear() + 1);
                                }
                            }
                            firstDate = firstDate.getTime();
                            secondDate = secondDate.getTime();
                            var diff = (secondDate - firstDate) / (1000 * 3600 * 24);
                            return ((firstDate <= secondDate && secondDate <= -2203915325000)
                                || (secondDate >= firstDate && firstDate >= -2203915324000))
                                ? diff
                                : (firstDate < secondDate ? diff - 1 : diff);
                        default:
                            throw 'Invalid Parameter';
                    }
                }";
        }

        /// <summary>
        /// Returns the day of a date, represented by a date, serial number.
        /// The day is given as an integer ranging from 1 to 31.
        /// </summary>
        /// <remarks>
        /// Syntax: $DAY(date)
        /// </remarks>
        private static string GetDayScript()
        {
            return @"
                function $DAY(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        date = new Date(Date.parse(date));
                    }
                    else
                    {
                        date = Number(date);
                        date = new Date(1900, 0, date > 59 ? date - 1 : date);
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getDate();
                }";
        }

        /// <summary>
        /// Returns the number of days between two dates.
        /// </summary>
        /// <remarks>
        /// Syntax: $DAYS(firstDate, secondDate)
        /// </remarks>
        private static string GetDaysScript()
        {
            return @"
                function $DAYS(firstDate, secondDate)
                {
                    if (firstDate == undefined || secondDate == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(firstDate))
                    {
                        firstDate = new Date(Date.parse(firstDate));
                    }
                    else
                    {
                        firstDate = Number(firstDate);
                        firstDate = new Date(1900, 0, firstDate > 59 ? firstDate - 1 : firstDate);
                    }
                    if (isNaN(firstDate.getTime()) || firstDate.getFullYear() < 1900 || firstDate.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(secondDate))
                    {
                        secondDate = new Date(Date.parse(secondDate));
                    }
                    else
                    {
                        secondDate = Number(secondDate);
                        secondDate = new Date(1900, 0, secondDate > 59 ? secondDate - 1 : secondDate);
                    }
                    if (isNaN(secondDate.getTime()) || secondDate.getFullYear() < 1900 || secondDate.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    firstDate = firstDate.getTime();
                    secondDate = secondDate.getTime();
                    var diff = (firstDate - secondDate) / (1000 * 3600 * 24);
                    return ((secondDate <= firstDate && firstDate <= -2203915325000)
                        || (firstDate >= secondDate && secondDate >= -2203915324000))
                        || ((firstDate <= secondDate && secondDate <= -2203915325000)
                        || (secondDate >= firstDate && firstDate >= -2203915324000))
                        ? diff
                        : (firstDate > secondDate
                            ? diff + 1
                            : (firstDate < secondDate ? diff - 1 : diff));
                }";
        }

        /// <summary>
        /// Returns the hour of a time value.
        /// The hour is given as an integer, ranging from 0 to 23.
        /// </summary>
        /// <remarks>
        /// Syntax: $HOUR(date)
        /// </remarks>
        private static string GetHourScript()
        {
            return @"
                function $HOUR(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        var originDate = date;
                        date = new Date(Date.parse(date));
                        if (isNaN(date.getTime()))
                        {
                            date = new Date(Date.parse('1900/01/01 ' + originDate));
                        }
                    }
                    else
                    {
                        date = Number(date);
                        if (date % 1 === 0)
                        {
                            date = new Date(1900, 0, date > 59 ? date - 1 : date);
                        }
                        else if (date % 1 !== 0)
                        {
                            return Math.floor((date * 24).toFixed(2) % 24);
                        }
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getHours();
                }";
        }

        /// <summary>
        /// Returns the minutes of a time value. The minute is given as an integer, ranging from 0 to 59.
        /// </summary>
        /// <remarks>
        /// Syntax: $MINUTE(date)
        /// </remarks>
        private static string GetMinuteScript()
        {
            return @"
                function $MINUTE(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        var originDate = date;
                        date = new Date(Date.parse(date));
                        if (isNaN(date.getTime()))
                        {
                            var reg = new RegExp('^[0-9]{1,2}:[0-9]{1,4}:[0-9]{1,2}$', 'g');
                            if (reg.test(originDate))
                            {
                                var times = originDate.split(':');
                                if (Number(times[0]) > 23 || Number(times[2]) > 59)
                                {
                                    throw 'Invalid Parameter';
                                }
                                return Number(times[1]) % 60;
                            }
                            date = new Date(Date.parse('1900/01/01 ' + originDate));
                        }
                    }
                    else
                    {
                        date = Number(date);
                        if (date % 1 === 0)
                        {
                            date = new Date(1900, 0, date > 59 ? date - 1 : date);
                        }
                        else if (date % 1 !== 0)
                        {
                            return Math.floor((date * 24 * 60).toFixed(2) % 60);
                        }
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getMinutes();
                }";
        }

        /// <summary>
        /// Returns the month of a date represented by a serial number.
        /// The month is given as an integer, ranging from 1 (January) to 12 (December).
        /// </summary>
        /// <remarks>
        /// Syntax: $MONTH(date)
        /// </remarks>
        private static string GetMonthScript()
        {
            return @"
                function $MONTH(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        date = new Date(Date.parse(date));
                    }
                    else
                    {
                        date = Number(date);
                        date = new Date(1900, 0, date > 59 ? date - 1 : date);
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getMonth() + 1;
                }";
        }

        /// <summary>
        /// Returns the serial number of the current date and time
        /// </summary>
        /// <remarks>
        /// Syntax: $NOW()
        /// </remarks>
        private static string GetNowScript(Context context)
        {
            return @"
                function $NOW()
                {
                    var d = new Date();
                    d.setMinutes(d.getMinutes() + d.getTimezoneOffset() + " + context.TimeZoneInfo.BaseUtcOffset.Hours * 60 + @");
                    return d.getFullYear()
                        + '/' + ('0' + (d.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + d.getDate()).slice(-2)
                        + ' ' + ('0' + d.getHours()).slice(-2)
                        + ':' + ('0' + d.getMinutes()).slice(-2)
                        + ':' + ('0' + d.getSeconds()).slice(-2);
                }";
        }

        /// <summary>
        /// Returns the seconds of a time value. The second is given as an integer in the range 0 (zero) to 59.
        /// </summary>
        /// <remarks>
        /// Syntax: $SECOND(date)
        /// </remarks>
        private static string GetSecondScript()
        {
            return @"
                function $SECOND(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        var originDate = date;
                        date = new Date(Date.parse(date));
                        if (isNaN(date.getTime()))
                        {
                            date = new Date(Date.parse('1900/01/01 ' + originDate));
                        }
                    }
                    else
                    {
                        date = Number(date);
                        if (date % 1 === 0)
                        {
                            date = new Date(1900, 0, date > 59 ? date - 1 : date);
                        }
                        else if (date % 1 !== 0)
                        {
                            return Math.floor((date * 24 * 60 * 60).toFixed(2) % 60);
                        }
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getSeconds();
                }";
        }

        /// <summary>
        /// Returns the serial number of the current date.
        /// </summary>
        /// <remarks>
        /// Syntax: $TODAY()
        /// </remarks>
        private static string GetTodayScript(Context context)
        {
            return @"
                function $TODAY()
                {
                    var d = new Date();
                    d.setMinutes(d.getMinutes() + d.getTimezoneOffset() + " + context.TimeZoneInfo.BaseUtcOffset.Hours * 60 + @");
                    return d.getFullYear()
                        + '/' + ('0' + (d.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + d.getDate()).slice(-2);
                }";
        }

        /// <summary>
        /// Returns the year corresponding to a date. The year is returned as an integer in the range 1900-9999.
        /// </summary>
        /// <remarks>
        /// Syntax: $YEAR(date)
        /// </remarks>
        private static string GetYearScript()
        {
            return @"
                function $YEAR(date)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        date = new Date(Date.parse(date));
                    }
                    else
                    {
                        date = Number(date);
                        date = new Date(1900, 0, date > 59 ? date - 1 : date);
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    return date.getFullYear();
                }";
        }

        /// <summary>
        /// Combines the text from multiple ranges and/or strings.
        /// </summary>
        /// <remarks>
        /// Syntax: $CONCAT(firstString1, [firstString2],…)
        /// </remarks>
        private static string GetConcatScript()
        {
            return @"
                function $CONCAT(firstString)
                {
                    if (arguments.length === 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    let result = firstString === undefined ? '' : firstString;
                    for (var i = 1; i < arguments.length; i++)
                    {
                        if(arguments[i] !== undefined && arguments[i] !== '')
                        {
                            result = result.toString() + arguments[i].toString();
                        }
                    }
                    return result;
                }";
        }

        /// <summary>
        /// Finds the first occurrence of a string within another string. Uppercase and lowercase letters are case sensitive
        /// </summary>
        /// <remarks>
        /// Syntax: $FIND(findText, withinText, start = 1)
        /// </remarks>
        private static string GetFindScript()
        {
            return @"
                function $FIND(findText, withinText, startNum = 1)
                {
                    if (findText == undefined || withinText == undefined || isNaN(startNum))
                    {
                        throw 'Invalid Parameter';
                    }
                    startNum = Number(startNum);
                    if (startNum < 1)
                    {
                        throw 'Invalid Parameter';
                    }
                    var index = withinText.toString().indexOf(findText.toString(), startNum - 1);
                    if (index < 0)
                    {
                        throw 'Not Found';
                    }
                    return index + 1;
                }";
        }

        /// <summary>
        /// LEFT returns the first character or characters in a text string, based on the number of characters you specify.
        /// </summary>
        /// <remarks>
        /// Syntax: $LEFT(text, numChars)
        /// </remarks>
        private static string GetLeftScript()
        {
            return @"
                function $LEFT(text, numChars = 1)
                {
                    if (text == undefined || isNaN(numChars))
                    {
                        throw 'Invalid Parameter';
                    }
                    numChars = Number(numChars);
                    if (numChars < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().substring(0, numChars);
                }";
        }

        /// <summary>
        /// LEN returns the number of characters in a text string.
        /// </summary>
        /// <remarks>
        /// Syntax: $LEN(text)
        /// </remarks>
        private static string GetLenScript()
        {
            return @"
                function $LEN(text)
                {
                    if (text == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().length;
                }";
        }

        /// <summary>
        /// Converts all uppercase letters in a text string to lowercase.
        /// </summary>
        /// <remarks>
        /// Syntax: $LOWER(text)
        /// </remarks>
        private static string GetLowerScript()
        {
            return @"
                function $LOWER(text)
                {
                    if (text == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().toLowerCase();
                }";
        }

        /// <summary>
        /// MID returns a specific number of characters from a text string, starting at the position you specify, based on the number of characters you specify.
        /// </summary>
        /// <remarks>
        /// Syntax: $MID(text, startNum, numChars)
        /// </remarks>
        private static string GetMidScript()
        {
            return @"
                function $MID(text, startNum, numChars)
                {
                    if (text == undefined || isNaN(startNum) || isNaN(numChars))
                    {
                        throw 'Invalid Parameter';
                    }
                    startNum = Number(startNum);
                    numChars = Number(numChars);
                    if (startNum < 1 || numChars < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().substring(startNum - 1, startNum - 1 + numChars);
                }";
        }

        /// <summary>
        /// RIGHT returns the last character or characters in a text string, based on the number of characters you specify.
        /// </summary>
        /// <remarks>
        /// Syntax: $RIGHT(text,[numChars])
        /// </remarks>
        private static string GetRightScript()
        {
            return @"
                function $RIGHT(text, numChars = 1)
                {
                    if (text == undefined || isNaN(numChars))
                    {
                        throw 'Invalid Parameter';
                    }
                    numChars = Number(numChars);
                    if (numChars < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().substring(text.toString().length - numChars);
                }";
        }

        /// <summary>
        /// Substitutes newtext for oldText in a text string. 
        /// </summary>
        /// <remarks>
        /// Syntax: $SUBSTITUTE(text, oldText, newtext, [instanceNum])
        /// </remarks>
        private static string GetSubstituteScript()
        {
            return @"
                function $SUBSTITUTE(text, oldText, newtext, instanceNum)
                {
                    if (text == undefined || oldText == undefined || newtext == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    let reg = new RegExp(oldText.toString(), 'g');
                    if (instanceNum == undefined)
                    {
                        return text.toString().replace(reg, newtext.toString());
                    }
                    else if (isNaN(instanceNum))
                    {
                        throw 'Invalid Parameter';
                    }
                    instanceNum = Number(instanceNum);
                    if (instanceNum < 1)
                    {
                        throw 'Invalid Parameter';
                    }
                    let i = 0;
                    return text.toString().replace(reg, match => ++i == instanceNum ? newtext.toString() : match);
                }";
        }

        /// <summary>
        /// Removes all spaces from text except for single spaces between words.
        /// </summary>
        /// <remarks>
        /// Syntax: $TRIM(text)
        /// </remarks>
        private static string GetTrimScript()
        {
            return @"
                function $TRIM(text)
                {
                    if (text == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().trim();
                }";
        }

        /// <summary>
        /// Converts text to uppercase.
        /// </summary>
        /// <remarks>
        /// Syntax: $UPPER(text)
        /// </remarks>
        private static string GetUpperScript()
        {
            return @"
                function $UPPER(text)
                {
                    if (text == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    return text.toString().toUpperCase();
                }";
        }

        /// <summary>
        /// Use the AND function, one of the logical functions, to determine if all conditions in a test are TRUE.
        /// </summary>
        /// <remarks>
        /// Syntax: $AND(firstClause, [secondClause], ...)
        /// </remarks>
        private static string GetAndScript()
        {
            return @"
                function $AND(firstClause)
                {
                    
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    for (let i = 1; i < arguments.length; i++) {
                        if (arguments[i] === undefined || arguments[i].toString().trim() === '') {
                            continue;
                        }
                        arguments[i] =
                            arguments[i] == '0' || arguments[i] == 'false'
                                ? false
                                : arguments[i];
                        if (typeof arguments[i] === 'boolean' || !isNaN(arguments[i])) {
                            if (arguments[i] === false) {
                                return false;
                            }
                            firstClause =  Boolean(firstClause);
                        }
                    }
                    firstClause =
                        firstClause == '0' || firstClause == 'false'
                            ? false
                            : firstClause == 'true'
                            ? true
                            : firstClause;
                    if (firstClause === undefined || firstClause === '' || isNaN(firstClause)) {
                        throw 'Invalid Parameter';
                    }
                    return Boolean(firstClause);
                }";
        }

        /// <summary>
        /// Returns the specified value depending on the result of a logical expression(TRUE or FALSE)
        /// </summary>
        /// <remarks>
        /// Syntax: $IF(expression, valueIfTrue, valueIfFalse)
        /// </remarks>
        private static string GetIfScript()
        {
            return @"
                function $IF(expression, valueIfTrue, valueIfFalse = false)
                {
                    if (expression == undefined || valueIfTrue == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
                    return expression ? valueIfTrue : valueIfFalse;
                }";
        }

        /// <summary>
        /// Returns TRUE if the argument is FALSE, otherwise returns FALSE
        /// </summary>
        /// <remarks>
        /// Syntax: $NOT(expression)
        /// </remarks>
        private static string GetNotScript()
        {
            return @"
                function $NOT(expression)
                {
                    if (expression == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
                    return !expression;
                }";
        }

        /// <summary>
        /// Returns TRUE if either argument is TRUE. Returns FALSE if all arguments are FALSE
        /// </summary>
        /// <remarks>
        /// Syntax: $OR(expression, [expression2], ...))
        /// </remarks>
        private static string GetOrScript()
        {
            return @"
                function $OR(expression)
                {
                    if (expression == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
                    for (var i = 1; i < arguments.length; i++)
                    {
                        if (!isNaN(arguments[i]))
                        {
                            arguments[i] = (arguments[i] != 0);
                        }
                        else if (typeof arguments[i] != 'boolean')
                        {
                            throw 'Invalid Parameter';
                        }
                        expression = expression || arguments[i];
                    }
                    return expression;
                }";
        }

        /// <summary>
        /// Replaces part of a text string, based on the number of characters you specify, with a different text string.
        /// </summary>
        /// <remarks>
        /// Syntax: REPLACE(oldText, startNum, numChars, newText)
        /// </remarks>
        private static string GetReplaceScript()
        {
            return @"
                function $REPLACE(oldText, startNum, numChars, newText)
                {
                    if (arguments.length !== 4)
                    {
                        throw 'Invalid Parameter';
                    }
                    startNum = (startNum === undefined) ? 0 : startNum;
                    numChars = (numChars === undefined) ? 0 : numChars;
                    if (isNaN(startNum) || isNaN(numChars))
                    {
                        throw 'Invalid Parameter';
                    }
                    startNum = Number(startNum);
                    numChars = Number(numChars);
                    if (startNum < 1 || numChars < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    if(oldText === undefined || oldText === '') {
                        return newText === undefined ? '' : newText;
                    }
                    if(oldText === undefined || newText === undefined) {
                        return '';
                    }
                    return oldText.toString().substring(0, startNum - 1)
                        + newText.toString()
                        + oldText.toString().substring(startNum - 1 + numChars);
                }";
        }

        /// <summary>
        /// Returns the character number of the first occurrence of a string. Uppercase and lowercase letters are not sensitive
        /// </summary>
        /// <remarks>
        /// Syntax: SEARCH(findText, withinText, [startNum])
        /// </remarks>
        private static string GetSearchScript()
        {
            return @"
                function $SEARCH(findText, withinText, start = 1)
                {
                    if(arguments.length === 0 ) {
                        throw 'Invalid Parameter';
                    }
                    if (findText == undefined && withinText == undefined && start == 1)
                    {
                        return 1;
                    }
                    start = Number(start);
                    if (start < 1 || start > withinText.toString().length) {
                        throw 'Invalid Parameter';
                    }
                    let index = withinText.toString().toLowerCase().indexOf(findText.toString().toLowerCase(), start - 1);
                    if (index < 0) {
                        throw 'Not Found';
                    }
                    return index + 1;
	            }";
        }


        /// <summary>
        /// Checks if one or more conditions are met and returns the value corresponding to the first true condition
        /// </summary>
        /// <remarks>
        /// Syntax: IFS(firstClause, retValue1, [logicalClause2, retValue2, logicalClause3, retValue3])
        /// </remarks>
        private static string GetIfsScript()
        {
            return @"
                function $IFS(firstClause, retValue1)
                {
                    if (arguments.length === 0 || arguments.length % 2 !== 0) {
                        throw 'Invalid Parameter';
                    }            
                    for (let i = 0; i < arguments.length; i = i + 2)
                    {
                        logicalTest = arguments[i],
                        valueIfTrue = arguments[i+1];
                        if (logicalTest === '')
                        {
                            throw 'Invalid Parameter';
                        }
                        logicalTest = (logicalTest === 'false') ? false : (logicalTest === 'true') ? true : logicalTest;
                        if (!isNaN(logicalTest) || typeof logicalTest === 'boolean')
                        {       
                            if(Boolean(logicalTest)) {
                                return valueIfTrue === undefined ? 0 : valueIfTrue;
                            }        
                            logicalTest = Boolean(logicalTest);
                        }
                    }
                    if((logicalTest === undefined && logicalTest === undefined) || logicalTest === false) {
                        throw 'Invalid Parameter';
                    }
                    if(logicalTest && (valueIfTrue === undefined)) {
                        return 0;
                    }
	            }";
        }

        /// <summary>
        /// Returns TRUE if number is even, or FALSE if number is odd.
        /// </summary>
        /// <remarks>
        /// Syntax: ISEVEN(number)
        /// </remarks>
        private static string GetIsEvenScript()
        {
            return @"
                function $ISEVEN(number)
                {
                    if (number == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(number))
                    {
                        return $DAYS(number, '1/1/2000') % 2 == 0;
                    }
                    number = Number(number);
                    return Math.trunc(number) % 2 == 0;
	            }";
        }
        /// <summary>
        /// Value refers to a number.
        /// </summary>
        /// <remarks>
        /// Syntax: ISNUMBER(value)
        /// </remarks>
        private static string GetIsNumberScript()
        {
            return @"
                function $ISNUMBER(value)
                {
                    if (arguments.length === 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    if(!isNaN(value) && typeof value !== 'string') 
                    {
                         return true; 
                    }
                    if(typeof value == 'string') 
                    {
                        if(value == '') 
                        {
                             return false; 
                        }
                        if(/^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/.test(value.substring(0,10).trim())) 
                        {
                             return true; 
                        }
                    }
                    return false;
	            }";
        }

        /// <summary>
        /// Returns TRUE if number is odd, or FALSE if number is even.
        /// </summary>
        /// <remarks>
        /// If number is nonnumeric, ISODD will throw Invalid Parametererror Exception.
        /// Syntax: ISODD(number)
        /// </remarks>
        private static string GetIsOddScript()
        {
            return @"
                function $ISODD(number)
                {
                    if (arguments.length === 0 || number === '')
                    {
                        throw 'Invalid Parameter';
                    }
                    if (number === undefined)
                    {
                        return false;
                    }
                    if (isNaN(number) && typeof number === 'string')
                    {
                        if(/^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/.test(number.substring(0,10).trim())) { 
                            return $DAYS(number, '1/2/2000') % 2 === 0;
                        }
                        throw 'Invalid Parameter';
                    }
                    number = Number(number);
                    return Math.trunc(number) % 2 !== 0;
	            }";
        }

        /// <summary>
        /// Returns TRUE if the cell content is a string
        /// </summary>
        /// <remarks>
        /// Syntax: ISTEXT(text)
        /// </remarks>
        private static string GetIsTextScript()
        {
            return @"
                function $ISTEXT(text)
                {
                    if (arguments.length === 0)
                    {
                        throw 'Invalid Parameter';
                    }     
                    if (text === undefined)
                    {
                        return false;
                    }
                    if(typeof text === 'string' || text instanceof String) {
                        if(/^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/.test(text.substring(0,10).trim())) {
                            return false;
                        }
                        return true;
                    }
                    if (text === undefined)
                    {
                        return false;
                    }
                    return typeof text === 'string' || text instanceof String;
	            }";
        }

        /// <summary>
        /// Returns the remainder after number is divided by divisor. The result has the same sign as divisor
        /// </summary>
        /// <remarks>
        /// Syntax: MOD(number, divisor)
        /// </remarks>
        private static string GetModScript()
        {
            return @"
                function $MOD(number, divisor)
                {
                    if (number == undefined || divisor == undefined || isNaN(number) || isNaN(divisor))
                    {
                        throw 'Invalid Parameter';
                    }
                    return Math.abs(number) % divisor * (divisor > 0 ? 1 : -1);
	            }";
        }

        /// <summary>
        /// Returns number rounded up to the nearest odd integer.
        /// </summary>
        /// <remarks>
        /// Syntax: ODD(number)
        /// </remarks>
        private static string GetOddScript()
        {
            return @"
                function $ODD(number)
                {
                    if (arguments.length === 0)
                    {
                        throw 'Invalid Parameter';
                    } 
                    if (number === undefined || number === 0)
                    {
                        return 1;
                    }
                    if (isNaN(number) && typeof number === 'string')
                    {
                        if(/^(\d{4})(\/|-)(\d{1,2})(\/|-)(\d{1,2})$/.test(number.substring(0,10).trim())) {
                            number = $DAYS(number, '01/01/1900');
                        }
                    }
                    let result = Math.ceil(Number(number));
                    return (result % 2 === 0) ? result + (result > 0 ? 1 : -1) : result;
	            }";
        }

        /// <summary>
        /// Returns the average (arithmetic mean) of the arguments. 
        /// </summary>
        /// <remarks>
        /// Syntax: AVERAGE(number1, [number2], ...)
        /// </remarks>
        private static string GetAverageScript()
        {
            return @"
                function $AVERAGE(number1)
                {
                    if (number1 == undefined || isNaN(number1))
                    {
                        throw 'Invalid Parameter';
                    }
                    number1 = Number(number1);
                    var total = number1;
		            for (var i = 1; i < arguments.length; i++)
		            {
                        if (arguments[i] == undefined || isNaN(arguments[i]))
                        {
                            throw 'Invalid Parameter';
                        }
			            total += Number(arguments[i]);
		            }
		            return total / arguments.length;
	            }";
        }

        /// <summary>
        /// Returns the day of the week corresponding to a date.
        /// The day is given as an integer, ranging from 1 (Sunday) to 7 (Saturday), by default.
        /// </summary>
        /// <remarks>
        /// Syntax: WEEKDAY(date, [returnType])
        /// </remarks>
        private static string GetWeekdayScript()
        {
            return @"
                function $WEEKDAY(date, returnType = 1)
                {
                    if (date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(date))
                    {
                        date = new Date(Date.parse(date));
                    }
                    else
                    {
                        if(Number(date) === 0 || Number(date) === 1) {
                            date = Number(date) + 7;
                        }
                        date = new Date(1900, 0, Number(date -1));
                    }
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    switch (returnType)
                    {
                        case 1:
                        case 17:
                            return date.getDay() + 1;
                        case 2:
                        case 11:
                            return date.getDay() == 0 ? 7 : date.getDay();
                        case 3:
                            return date.getDay() == 0 ? 6 : date.getDay() - 1;
                        case 12:
                            return (date.getDay() + 6) % 7 == 0 ? 7 : (date.getDay() + 6) % 7;
                        case 13:
                            return (date.getDay() + 5) % 7 == 0 ? 7 : (date.getDay() + 5) % 7;
                        case 14:
                            return (date.getDay() + 4) % 7 == 0 ? 7 : (date.getDay() + 4) % 7;
                        case 15:
                            return (date.getDay() + 3) % 7 == 0 ? 7 : (date.getDay() + 3) % 7;
                        case 16:
                            return (date.getDay() + 2) % 7 == 0 ? 7 : (date.getDay() + 2) % 7;
                        default:
                            throw 'Invalid Parameter';
                    }
                }";
        }

        /// <summary>
        /// Returns the smallest number in a set of values.
        /// </summary>
        /// <remarks>
        /// Syntax: MIN(number1, [number2], ...)
        /// </remarks>
        private static string GetMinScript()
        {
            return @"
                function $MIN(number1)
                {
                    if (arguments.length == 0 || arguments.length > 255)
                    {
                        throw 'Invalid Parameter';
                    }
                    let minValue = arguments[0];
                    for (let i = 1; i < arguments.length; i++)
                    {
                        if (arguments[i] !== null
                            && arguments[i] !== ''
                            && (minValue === undefined || arguments[i] < minValue))
                        {
                            minValue = arguments[i];
                        }
                    }
                    if (minValue === undefined)
                    {
                        minValue = 0;
                    }
                    return !isNaN(Number(minValue)) ? minValue : 0;
                }";
        }

        /// <summary>
        /// Returns the largest value in a set of values.<br/>
        /// </summary>
        /// <remarks>
        /// Syntax: MAX(number1, [number2], ...)
        /// </remarks>
        private static string GetMaxScript()
        {
            return @"
                function $MAX(number1)
                {
                    if (arguments.length == 0 || arguments.length > 255)
                    {
                        throw 'Invalid Parameter';
                    }
                    let maxValue = arguments[0];
                    for (let i = 1; i < arguments.length; i++)
                    {
                        if (arguments[i] !== null
                            && arguments[i] !== ''
                            && (maxValue === undefined || arguments[i] > maxValue))
                        {
                            maxValue = arguments[i];
                        }
                    }
                    if (maxValue === undefined)
                    {
                        maxValue = 0;
                    }
                    return !isNaN(Number(maxValue)) ? maxValue : 0;
                }";
        }

        /// <summary>
        /// Rounds a number to a specified number of digits.
        /// </summary>
        /// <remarks>
        /// Syntax: ROUND(number,numDigits)
        /// </remarks>
        private static string GetRoundScript()
        {
            return @"
                function $ROUND(number, numDigits)
                {
                    if (number === '' || numDigits === '' || isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        throw 'Invalid Parameter';
                    }
                    let negative = 1,
                    result;
                    if (number < 0)
                    {
                        negative = -1;
                        number = number * -1;
                    }
                    if (numDigits === 0)
                    {
                        result = Math.round(number) * negative;
                    }
                    else if (numDigits > 0)
                    {
                        let multiplier = Math.pow(10, numDigits),
                            n = parseFloat(
                                (number * multiplier).toFixed(numDigits + 1)
                            );
                        result =
                            (Math.round(n) / multiplier).toFixed(numDigits) *
                            negative;
                    }
                    else if (numDigits < 0)
                    {
                        let divider = Math.pow(10, -numDigits);
                        result = Math.round(number / divider) * divider * negative;
                    }
                    return result == 0 ? 0 : result;
                }";
        }

        /// <summary>
        /// Rounds a number up, away from 0 (zero).
        /// </summary>
        /// <remarks>
        /// Syntax: ROUNDUP(number,numDigits)
        /// </remarks>
        private static string GetRoundUpScript()
        {
            return @"
                function $ROUNDUP(number, numDigits)
                {
                    if (number === '' || numDigits === '' || isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        throw 'Invalid Parameter';
                    }
                    let negative = 1,
                    result;
                    if (number < 0)
                    {
                        negative = -1;
                        number = number * -1;
                    }
                    if (numDigits === 0)
                    {
                        result = Math.ceil(number) * negative;
                    }
                    else if (numDigits > 0)
                    {
                        let multiplier = Math.pow(10, numDigits),
                            n = parseFloat(
                                (number * multiplier).toFixed(numDigits + 1)
                            );
                        result =
                            (Math.ceil(n) / multiplier).toFixed(numDigits) *
                            negative;
                    }
                    else if (numDigits < 0)
                    {
                        let divider = Math.pow(10, -numDigits);
                        result = Math.ceil(number / divider) * divider * negative;
                    }
                    return result == 0 ? 0 : result;
                }";
        }

        /// <summary>
        /// Rounds a number down, toward zero.
        /// </summary>
        /// <remarks>
        /// Syntax: ROUNDDOWN(number,numDigits)
        /// </remarks>
        private static string GetRoundDownScript()
        {
            return @"
                function $ROUNDDOWN(number, numDigits)
                {
                    if (number === '' || numDigits === '' || isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        throw 'Invalid Parameter';
                    }
                    let negative = 1,
                    result;
                    if (number < 0)
                    {
                        negative = -1;
                        number = number * -1;
                    }
                    if (numDigits === 0)
                    {
                        result = Math.floor(number) * negative;
                    }
                    else if (numDigits > 0)
                    {
                        let multiplier = Math.pow(10, numDigits),
                            n = parseFloat(
                                (number * multiplier).toFixed(numDigits + 1)
                            );
                        result =
                            (Math.floor(n) / multiplier).toFixed(numDigits) *
                            negative;
                    }
                    else if (numDigits < 0)
                    {
                        let divider = Math.pow(10, -numDigits);
                        result = Math.floor(number / divider) * divider * negative;
                    }
                    return result == 0 ? 0 : result;
                }";
        }

        /// <summary>
        /// Truncates a number to an integer by removing the fractional part of the number.
        /// </summary>
        /// <remarks>
        /// Syntax: TRUNC(number, [numDigits])
        /// </remarks>
        private static string GetTruncScript()
        {
            return @"
                function $TRUNC(number, numDigits)
                {
                    if (numDigits == undefined)
                    {
                        numDigits = 0;
                    }
                    if (number == undefined ||
                        number === '' ||
                        numDigits === '' ||
                        isNaN(Number(number)) ||
                        isNaN(Number(numDigits)))
                    {
                        throw 'Invalid Parameter';
                    }
                    if (numDigits >= 0)
                    {
                        let multiplier = Math.pow(10, numDigits);
                        return Math.trunc(number * multiplier) / multiplier;
                    }
                    else if (numDigits < 0)
                    {
                        let divider = Math.pow(10, -numDigits);
                        return Math.trunc(number / divider) * divider;
                    }
                }";
        }

        /// <summary>
        /// Converts full-width alphanumeric kana characters (2 bytes) to half-width alphanumeric kana characters (1 byte)
        /// </summary>
        /// <remarks>
        /// Syntax: ASC(text)
        /// </remarks>
        private static string GetAscScript()
        {
            return @"
                function $ASC(text)
                {
                    return text.toString().replace(/[Ａ-Ｚａ-ｚ０-９]/g, function (s) {
                        return String.fromCharCode(s.charCodeAt(0) - 0xfee0);
                    });
                }";
        }

        /// <summary>
        /// Converts half-width alphanumeric kana characters (1 byte) to full-width alphanumeric kana characters (2 bytes)
        /// </summary>
        /// <remarks>
        /// Syntax: JIS(text)
        /// </remarks>
        private static string GetJisScript()
        {
            return @"
                function $JIS(text)
                {
                    return text.toString().replace(/[A-Za-z0-9]/g, function (s) {
                        return String.fromCharCode(s.charCodeAt(0) + 0xfee0);
                    });
                }";
        }

        /// <summary>
        /// Converts numbers entered as strings to numbers
        /// </summary>
        /// <remarks>
        /// Syntax: VALUE(text)
        /// </remarks>
        private static string GetValueScript()
        {
            return @"
                function $VALUE(text)
                {
                    if (text == undefined || text === '')
                    {
                        throw 'Invalid Parameter';
                    }
                    let timeRegex = /^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$/;
                    if (timeRegex.test(text))
                    {
                        let hour = Number(text.substring(0, 2)),
                        minute = Number(text.substring(3, 5));
                        return Number(((hour + minute/60) / 24).toFixed(1));
                    }
                    else
                    {
                        text = text.toString().replace(/[$,]/g, '');
                        if (isNaN(Number(text)))
                        {
                            throw 'Invalid Parameter';
                        }
                        return Number(text);
                    }
                }";
        }

        /// <summary>
        /// Converts numbers to text by applying formatting to it with format codes.
        /// </summary>
        /// <remarks>
        /// Syntax: TEXT(value, format)
        /// </remarks>
        private static string GetTextScript()
        {
            return @"
                function $TEXT(value, format)
                {
                    return FormulaServerScriptUtilities.GetText(value, format);
                }";
        }

        /// <summary>
        /// Returns the absolute value of a number.
        /// </summary>
        /// <remarks>
        /// Syntax: $ABS(number)
        /// </remarks>
        private static string GetAbsScript()
        {
            return @"
                function $ABS(number)
                {
                    if (arguments.length === 0 || number === '' || number === undefined || isNaN(Number(number)))
                    {
                        throw 'Invalid Parameter';
                    }
                    return Math.abs(Number(number));
	            }";
        }

        /// <summary>
        /// Returns the result of a number raised to a power.
        /// </summary>
        /// <remarks>
        /// Syntax: $POWER(number, power)
        /// </remarks>
        private static string GetPowerScript()
        {
            return @"
                function $POWER(number, power)
                {
                    if (arguments.length === 0
                        || number === '' || number === undefined || isNaN(Number(number))
                        || power === '' || power === undefined || isNaN(Number(power)))
                    {
                        throw 'Invalid Parameter';
                    }
                    return Math.pow(Number(number), Number(power));
	            }";
        }

        /// <summary>
        /// Returns an evenly distributed random real number greater than or equal to 0 and less than 1.
        /// </summary>
        /// <remarks>
        /// Syntax: $RAND()
        /// </remarks>
        private static string GetRandScript()
        {
            return @"
                function $RAND()
                {
                    return Math.random();
	            }";
        }

        /// <summary>
        /// Returns a positive square root.
        /// </summary>
        /// <remarks>
        /// Syntax: $SQRT(number)
        /// </remarks>
        private static string GetSqrtScript()
        {
            return @"
                function $SQRT(number)
                {
                    if (arguments.length === 0 || number === '' || number === undefined || isNaN(Number(number)) || Number(number) < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    return Math.sqrt(Number(number));
	            }";
        }

        /// <summary>
        /// Returns the serial number for the last day of the month that is the indicated number of months before or after start_date.
        /// </summary>
        /// <remarks>
        /// Syntax: $EOMONTH(start_date, months)
        /// </remarks>
        private static string GetEOMonthScript()
        {
            return @"
                function $EOMONTH(start_date, months)
                {
                    if (arguments.length === 0 || months === '' || months === undefined || isNaN(Number(months)) || start_date == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(start_date))
                    {
                        start_date = new Date(Date.parse(start_date));
                    }
                    else
                    {
                        start_date = Number(start_date);
                        start_date = new Date(1900, 0, start_date > 59 ? start_date - 1 : start_date);
                    }
                    if (isNaN(start_date.getTime()) || start_date.getFullYear() < 1900 || start_date.getFullYear() > 9999)
                    {
                        throw 'Invalid Parameter';
                    }
                    var d = new Date(start_date.getFullYear(), start_date.getMonth() + Number(months) + 1, 0);
                    return d.getFullYear()
                        + '/' + ('0' + (d.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + d.getDate()).slice(-2);
                }";
        }
    }
}