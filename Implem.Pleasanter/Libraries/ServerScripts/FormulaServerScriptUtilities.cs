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
                .Replace("$or(", "$OR(", StringComparison.InvariantCultureIgnoreCase);
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
                    if (arguments.length != 3) {
                        throw 'Invalid Parameter';
                    }
                    year = (year === undefined || year === '' || year === '0') ? 0 : year;
                    month = (month === undefined || month === '' || month === '0') ? 0 : month;
                    day = (day === undefined || day === '' || day === '0') ? 0 : day;
                    if (isNaN(year) || isNaN(month) || isNaN(day))
                    {
                        throw '#NUM!';
                    }
                    year = Number(year);
                    month = Number(month);
                    day = Number(day);
                    if(year === 0 && month === 1 && day === 0) {
                        return '1900/01/00';
                    }    
                    if (year >= 0 && year < 1900)
                    {
                        year = 1900 + year;
                    }
                    var date = new Date(year, month - 1, day);
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        throw '#NUM!';
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
        /// Syntax: $DATEDIF(startDate, endDate, unit)
        /// </remarks>
        private static string GetDateDifScript()
        {
            return @"
                function $DATEDIF(startDate, endDate, unit)
                {
                    if (arguments.length != 3) {
                        throw 'Invalid Parameter';
                    }
                    startDate = (startDate === undefined  || startDate === '' || startDate === '0') ? 0 : startDate;
                    endDate = (endDate === undefined || endDate === '' || endDate === '0') ? 0 : endDate;
                    unit = (unit === undefined  || unit === '' ) ? 0 : unit.toString().toUpperCase();
                    let originStartDate = startDate,
                        originEndDate = endDate;
                    if (startDate === 0 && endDate === 0)
                    {
                        if(['Y', 'M', 'D', 'MD', 'YM', 'YD'].includes(unit) ) 
                        {
                            return 0;
                        }
                        throw '#NUM!';
                    }
                    if (isNaN(startDate))
                    {
                        startDate = new Date(Date.parse(startDate));
                    }
                    else
                    {
                        startDate = Number(startDate);
                        startDate = new Date(1900, 0, startDate > 59 ? startDate - 1 : startDate);
                    }
                    if (originStartDate !== 0 && (isNaN(startDate.getTime()) || startDate.getFullYear() < 1900 || startDate.getFullYear() > 9999))
                    {
                        throw '#VALUE!';
                    }
                    if (isNaN(endDate))
                    {
                        endDate = new Date(Date.parse(endDate));
                    }
                    else
                    {
                        endDate = Number(endDate);
                        endDate = new Date(1900, 0, endDate > 59 ? endDate - 1 : endDate);
                    }
                    if (originEndDate !== 0 && (isNaN(endDate.getTime()) || endDate.getFullYear() < 1900 || endDate.getFullYear() > 9999)
                        || startDate.getTime() > endDate.getTime())
                    {
                        throw '#VALUE!';
                    }
                    switch(unit)
                    {
                        case 'Y':
                            return endDate.getFullYear() - startDate.getFullYear();
                        case 'M':
                            return endDate.getMonth() - startDate.getMonth() + 12 * (endDate.getFullYear() - startDate.getFullYear());
                        case 'D':
                            startDate = startDate.getTime();
                            endDate = endDate.getTime();
                            var diff = (endDate - startDate) / (1000 * 3600 * 24);
                            return ((startDate <= endDate && endDate <= -2203915325000)
                                || (endDate >= startDate && startDate >= -2203915324000))
                                ? diff
                                : (startDate < endDate ? diff - 1 : diff);
                        case 'MD':
                            return endDate.getDate() - startDate.getDate()
                                + (endDate.getDate() >= startDate.getDate() ? 0 : new Date(endDate.setDate(0)).getDate());
                        case 'YM':
                            return endDate.getMonth() - startDate.getMonth()
                                + (endDate.getMonth() >= startDate.getMonth() ? 0 : 12);
                        case 'YD':
                            if ((endDate.getMonth() == startDate.getMonth() && endDate.getDate() >= startDate.getDate())
                                || (endDate.getMonth() > startDate.getMonth()))
                            {
                                if (endDate.getFullYear() > startDate.getFullYear())
                                {
                                    endDate.setYear(startDate.getFullYear());
                                }
                            }
                            else
                            {
                                if (endDate.getFullYear() - startDate.getFullYear() > 1)
                                {
                                    endDate.setYear(startDate.getFullYear() + 1);
                                }
                            }
                            startDate = startDate.getTime();
                            endDate = endDate.getTime();
                            var diff = (endDate - startDate) / (1000 * 3600 * 24);
                            return ((startDate <= endDate && endDate <= -2203915325000)
                                || (endDate >= startDate && startDate >= -2203915324000))
                                ? diff
                                : (startDate < endDate ? diff - 1 : diff);
                        default:
                            throw '#NUM!';
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
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    if (date === undefined || date == '' || date == 0)
                    {
                        return 0;
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
                        throw '#VALUE!';
                    }
                    return date.getDate();
                }";
        }

        /// <summary>
        /// Returns the number of days between two dates.
        /// </summary>
        /// <remarks>
        /// Syntax: $DAYS(startDate, endDate)
        /// </remarks>
        private static string GetDaysScript()
        {
            return @"
                function $DAYS(startDate, endDate)
                {
                    startDate = (startDate === undefined || startDate === '' || startDate === '0') ? 0 : startDate;
                    endDate = (endDate === undefined || endDate === '' || startDate === '0') ? 0 : endDate;
                    let originStartDate  = startDate,
                        originEndDate = endDate;    
                    if (startDate === 0 && endDate === 0)
                    {
                        return 0;
                    }
                    if (isNaN(startDate))
                    {
                        startDate = new Date(Date.parse(startDate));
                    }
                    else
                    {
                        startDate = Number(startDate);
                        startDate = new Date(1900, 0, startDate > 59 ? startDate - 1 : startDate);
                    }
                    if (originStartDate !== 0 && (isNaN(startDate.getTime()) || startDate.getFullYear() < 1900 || startDate.getFullYear() > 9999))
                    {
                        throw '#VALUE!';
                    }
                    if (isNaN(endDate))
                    {
                        endDate = new Date(Date.parse(endDate));
                    }
                    else
                    {
                        endDate = Number(endDate);
                        endDate = new Date(1900, 0, endDate > 59 ? endDate - 1 : endDate);
                    }
                    if (originEndDate !== 0 && (isNaN(endDate.getTime()) || endDate.getFullYear() < 1900 || endDate.getFullYear() > 9999))
                    {
                        throw '#VALUE!';
                    }
                    startDate = startDate.getTime();
                    endDate = endDate.getTime();
                    let diff = (startDate - endDate) / (1000 * 3600 * 24),
                        result = ((endDate <= startDate && startDate <= -2203915325000)
                        || (startDate >= endDate && endDate >= -2203915324000))
                        || ((startDate <= endDate && endDate <= -2203915325000)
                        || (endDate >= startDate && startDate >= -2203915324000))
                        ? diff
                        : (startDate > endDate
                            ? diff + 1
                            : (startDate < endDate ? diff - 1 : diff));
                    return Math.trunc(result) ;
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
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    if(date === undefined || date === '' || date === '0') {
                        return 0;
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
                        throw '#NUM!';
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
                   if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    } 
                    if(date === undefined || date === '' || date == '0') {
                        return 0;
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
                                    throw '#VALUE!';
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
                        throw '#VALUE!';
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
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    if(date === undefined || date === '' || date == '0') {
                        return 1;
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
                        throw '#VALUE!';
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
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    if(date === undefined || date === '' || date == '0') {
                        return 0;
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
                        throw '#VALUE!';
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
                    if (arguments.length == 0) {
                        throw 'Invalid Parameter';
                    }
                    if(date === undefined || date === '' || date == '0') {
                        return 1900;
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
                        throw '#VALUE!';
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
                function $FIND(findText, withinText, startNum)
                {
                    if (arguments.length !== 3) {
                        throw 'Invalid Parameter';
                    }
                    if(isNaN(startNum) || Number(startNum) < 1) {
                        throw '#VALUE!';
                    }
                    findText = (findText == undefined) ? '' : findText;
                    withinText = (withinText == undefined ) ? '' : withinText; 
                    startNum = Number(startNum);
                    if(findText === '' && withinText === '' && startNum > 1) {
                        throw '#VALUE!';
                    }
                    var index = withinText.toString().indexOf(findText.toString(), startNum - 1);
                    if (index < 0)
                    {
                        throw '#VALUE!';
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
                    if (arguments.length !== 2) {
                        throw 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    if(isNaN(numChars) || Number(numChars) < 0) {
                        throw '#VALUE!';
                    }
                    numChars = Number(numChars);
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
                    if (arguments.length !== 1) {
                        throw 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
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
    }
}