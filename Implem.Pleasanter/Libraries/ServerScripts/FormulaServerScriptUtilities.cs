using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
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
                    value: model.CreatedTime?.Value,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.UpdatedTime),
                    value: model.UpdatedTime?.Value,
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
                    value: element.Value.ToString("yyyy/MM/dd HH:mm:ss"),
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
                    value: issueModel.StartTime,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.CompletionTime),
                    value: issueModel.CompletionTime.Value,
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
                    + GetValueScript();
                return engine.Evaluate(functionScripts + formulaScript);
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
                .Replace("$value(", "$VALUE(", StringComparison.InvariantCultureIgnoreCase);
        }

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

        private static string GetConcatScript()
        {
            return @"
                function $CONCAT(firstString)
                {
                    if (firstString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
		            for (var i = 1; i < arguments.length; i++)
		            {
			            firstString = firstString.toString() + arguments[i].toString();
		            }
		            return firstString;
	            }";
        }

        private static string GetFindScript()
        {
            return @"
                function $FIND(firstString, secondString, start = 1)
                {
                    if (firstString == undefined || secondString == undefined || isNaN(start))
                    {
                        throw 'Invalid Parameter';
                    }
                    start = Number(start);
                    if (start < 1)
                    {
                        throw 'Invalid Parameter';
                    }
	                var index = secondString.toString().indexOf(firstString.toString(), start - 1);
                    if (index < 0)
                    {
                        throw 'Not Found';
                    }
		            return index + 1;
	            }";
        }

        private static string GetLeftScript()
        {
            return @"
                function $LEFT(firstString, length = 1)
                {
                    if (firstString == undefined || isNaN(length))
                    {
                        throw 'Invalid Parameter';
                    }
                    length = Number(length);
                    if (length < 0)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().substring(0, length);
	            }";
        }

        private static string GetLenScript()
        {
            return @"
                function $LEN(firstString)
                {
                    if (firstString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().length;
	            }";
        }

        private static string GetLowerScript()
        {
            return @"
                function $LOWER(firstString)
                {
                    if (firstString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().toLowerCase();
	            }";
        }

        private static string GetMidScript()
        {
            return @"
                function $MID(firstString, start, length)
                {
                    if (firstString == undefined || isNaN(start) || isNaN(length))
                    {
                        throw 'Invalid Parameter';
                    }
                    start = Number(start);
                    length = Number(length);
                    if (start < 1 || length < 0)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().substring(start - 1, start - 1 + length);
	            }";
        }

        private static string GetRightScript()
        {
            return @"
                function $RIGHT(firstString, length = 1)
                {
                    if (firstString == undefined || isNaN(length))
                    {
                        throw 'Invalid Parameter';
                    }
                    length = Number(length);
                    if (length < 0)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().substring(firstString.toString().length - length);
	            }";
        }

        private static string GetSubstituteScript()
        {
            return @"
                function $SUBSTITUTE(firstString, secondString, thirdString, index)
                {
                    if (firstString == undefined || secondString == undefined || thirdString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (index == undefined)
                    {
                        var reg = new RegExp(secondString.toString(), 'g');
                        return firstString.toString().replace(reg, thirdString.toString());
                    }
                    else if (isNaN(index))
                    {
                        throw 'Invalid Parameter';
                    }
                    index = Number(index);
                    if (index < 1)
                    {
                        throw 'Invalid Parameter';
                    }
                    var i = 0;
                    var reg = new RegExp(secondString.toString(), 'g');
                    return firstString.toString().replace(reg, match => ++i == index ? thirdString.toString() : match);
	            }";
        }

        private static string GetTrimScript()
        {
            return @"
                function $TRIM(firstString)
                {
                    if (firstString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().trim();
	            }";
        }

        private static string GetUpperScript()
        {
            return @"
                function $UPPER(firstString)
                {
                    if (firstString == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstString.toString().toUpperCase();
	            }";
        }

        private static string GetAndScript()
        {
            return @"
                function $AND(firstClause)
                {
                    if (firstClause == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(firstClause))
                    {
                        firstClause = (firstClause != 0);
                    }
                    else if (typeof firstClause != 'boolean')
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
			            firstClause = firstClause && arguments[i];
		            }
		            return firstClause;
	            }";
        }

        private static string GetIfScript()
        {
            return @"
                function $IF(firstClause, retValue1, retValue2 = false)
                {
                    if (firstClause == undefined || retValue1 == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(firstClause))
                    {
                        firstClause = (firstClause != 0);
                    }
                    else if (typeof firstClause != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
		            return firstClause ? retValue1 : retValue2;
	            }";
        }

        private static string GetNotScript()
        {
            return @"
                function $NOT(firstClause)
                {
                    if (firstClause == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(firstClause))
                    {
                        firstClause = (firstClause != 0);
                    }
                    else if (typeof firstClause != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
		            return !firstClause;
	            }";
        }

        private static string GetOrScript()
        {
            return @"
                function $OR(firstClause)
                {
                    if (firstClause == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(firstClause))
                    {
                        firstClause = (firstClause != 0);
                    }
                    else if (typeof firstClause != 'boolean')
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
			            firstClause = firstClause || arguments[i];
		            }
		            return firstClause;
	            }";
        }

        private static string GetReplaceScript()
        {
            return @"
                function $REPLACE(firstString, start, length, secondString)
                {
                    if (firstString == undefined || secondString == undefined
                        || isNaN(start) || isNaN(length))
                    {
                        throw 'Invalid Parameter';
                    }
                    start = Number(start);
                    length = Number(length);
                    if (start < 1 || length < 0)
                    {
                        throw 'Invalid Parameter';
                    }
                    return firstString.toString().substring(0, start - 1)
                        + secondString.toString()
                        + firstString.toString().substring(start - 1 + length);
	            }";
        }

        private static string GetSearchScript()
        {
            return @"
                function $SEARCH(firstString, secondString, start = 1)
                {
                    if (firstString == undefined || secondString == undefined || isNaN(start))
                    {
                        throw 'Invalid Parameter';
                    }
                    start = Number(start);
                    if (start < 1 || start > secondString.length)
                    {
                        throw 'Invalid Parameter';
                    }
                    var index = secondString.toLowerCase().indexOf(firstString.toLowerCase(), start - 1);
                    if (index < 0)
                    {
                        throw 'Not Found';
                    }
                    return index + 1;
	            }";
        }

        private static string GetIfsScript()
        {
            return @"
                function $IFS(firstClause, retValue1)
                {
                    if (firstClause == undefined || retValue1 == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (!isNaN(firstClause))
                    {
                        firstClause = (firstClause != 0);
                    }
                    else if (typeof firstClause != 'boolean')
                    {
                        throw 'Invalid Parameter';
                    }
                    var retValue = firstClause ? retValue1 : '';
                    for (var i = 2; i < arguments.length; i = i + 2)
		            {
			            if (!isNaN(arguments[i]))
			            {
				            arguments[i] = (arguments[i] != 0);
			            }
			            else if (typeof arguments[i] != 'boolean')
			            {
				            throw 'Invalid Parameter';
			            }
			            retValue = arguments[i] ? (arguments[i + 1] == undefined ? '' : arguments[i + 1]) : retValue;
		            }
		            return retValue;
	            }";
        }

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

        private static string GetIsNumberScript()
        {
            return @"
                function $ISNUMBER(number)
                {
                    if (number == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (typeof number === 'string' || number instanceof String)
                    {
                        return false;
                    }
                    return !isNaN(number);
	            }";
        }

        private static string GetIsOddScript()
        {
            return @"
                function $ISODD(number)
                {
                    if (number == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(number))
                    {
                        return $DAYS(number, '1/2/2000') % 2 == 0;
                    }
                    number = Number(number);
                    return Math.trunc(number) % 2 == 0;
	            }";
        }

        private static string GetIsTextScript()
        {
            return @"
                function $ISTEXT(text)
                {
                    if (text == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    return typeof text === 'string' || text instanceof String;
	            }";
        }

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

        private static string GetOddScript()
        {
            return @"
                function $ODD(number)
                {
                    if (number == undefined)
                    {
                        throw 'Invalid Parameter';
                    }
                    if (isNaN(number))
                    {
                        var result = DAYS(number, '1/2/2000');
                    }
                    var result = Math.ceil(Number(number));
                    return result % 2 == 0 ? result + (result > 0 ? 1 : -1) : result;
	            }";
        }

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
                        date = Number(date);
                        date = new Date(1900, 0, date > 59 ? date - 1 : date);
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
                    if (number1 == undefined || arguments.length > 255)
                    {
                        throw 'Invalid Parameter';
                    }
                    let minValue = number1;
                    for (let i = 1; i < arguments.length; i++)
                    {
                        if (arguments[i] !== null && arguments[i] !== '' && arguments[i] < minValue)
                        {
                            minValue = arguments[i];
                        }
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
                    if (number1 == undefined || arguments.length > 255)
                    {
                        throw 'Invalid Parameter';
                    }
                    let maxValue = number1;
                    for (let i = 1; i < arguments.length; i++)
                    {
                        if (arguments[i] !== null && arguments[i] !== '' && arguments[i] > maxValue)
                        {
                            maxValue = arguments[i];
                        }
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
                    if(timeRegex.test(text)) {
                        let hour = Number(text.substring(0, 2)),
                        minute = Number(text.substring(3, 5));
                        return  Number(((hour + minutes/60) / 24).toFixed(1));
                    } else {
                        if (isNaN(Number(text))) {
                            throw 'Invalid Parameter';
                        }
                        return Number(text);
                    }
                }";
        }
    }
}