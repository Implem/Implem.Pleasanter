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
                    value: element.Value.ToString("yyyy/MM/dd"),
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
                    + GetOrScript();
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
                .Replace("$or(", "$OR(", StringComparison.InvariantCultureIgnoreCase);
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
                        + ':' + ('0' + d.getMinutes()).slice(-2);
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
	                var index = secondString.indexOf(firstString, start - 1);
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
		            return firstString.substring(0, length);
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
		            return firstString.length;
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
		            return firstString.toLowerCase();
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
		            return firstString.substring(start - 1, start - 1 + length);
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
		            return firstString.substring(firstString.length - length);
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
                        var reg = new RegExp(secondString, 'g');
                        return firstString.replace(reg, thirdString);
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
                    var reg = new RegExp(secondString, 'g');
                    return firstString.replace(reg, match => ++i == index ? thirdString : match);
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
		            return firstString.trim();
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
		            return firstString.toUpperCase();
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
    }
}