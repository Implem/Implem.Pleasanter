using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Dynamic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public static class FormulaServerScriptUtilities
    {
        public static object Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            string formulaScript)
        {
            var data = ServerScriptUtilities.Values(
                context: context,
                ss: ss,
                model: itemModel,
                isFormulaServerScript: true);
            var Model = new ExpandoObject();
            data?.ForEach(datam => ((IDictionary<string, object>)Model)[datam.Name] = datam.Value);
            formulaScript = ParseIgnoreCase(formulaScript);
            using (var engine = new ScriptEngine(debug: false))
            {
                engine.AddHostObject("model", Model);
                engine.AddHostObject("context", context);
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
                    + GetEOMonthScript()
                    + GetIsBlankScript()
                    + GetIsErrorScript()
                    + GetIfErrorScript()
                    + GetDateTimeScript();
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
                .Replace("$eomonth(", "$EOMONTH(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$isblank(", "$ISBLANK(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$iserror(", "$ISERROR(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$iferror(", "$IFERROR(", StringComparison.InvariantCultureIgnoreCase)
                .Replace("$datetime(", "$DATETIME(", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetText(object value, string format, Context context)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }
            if (long.TryParse(value.ToString(), out long longValue))
            {
                return longValue.ToString(
                    format: format,
                    provider: context.CultureInfoCurrency(context.Language));
            }
            if (double.TryParse(value.ToString(), out double doubleValue))
            {
                return doubleValue.ToString(
                    format: format,
                    provider: context.CultureInfoCurrency(context.Language));
            }
            if (DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
            {
                return dateTimeValue.ToString(
                    format: format,
                    provider: context.CultureInfoCurrency(context.Language));
            }
            return value.ToString();
        }

        private static string GetDateScript()
        {
            return @"
                function $DATE(year, month, day)
                {
                    if (arguments.length != 3) {
                        return 'Invalid Parameter';
                    }
                    year = (year === undefined || year === '' || year === '0') ? 0 : year;
                    month = (month === undefined || month === '' || month === '0') ? 0 : month;
                    day = (day === undefined || day === '' || day === '0') ? 0 : day;
                    if (isNaN(year) || isNaN(month) || isNaN(day))
                    {
                        return '#NUM!';
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
                        return '#NUM!';
                    }
                    return date.getFullYear()
                        + '/' + ('0' + (date.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + date.getDate()).slice(-2);
                }";
        }

        private static string GetDateDifScript()
        {
            return @"
                function $DATEDIF(startDate, endDate, unit)
                {
                    if (arguments.length != 3) {
                        return 'Invalid Parameter';
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
                        return '#NUM!';
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
                        return '#VALUE!';
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
                        return '#VALUE!';
                    }
                    switch(unit)
                    {
                        case 'Y':
                            return endDate.getFullYear() - startDate.getFullYear()
                                - (endDate.getMonth() > startDate.getMonth()
                                    ? 0
                                    : endDate.getMonth() < startDate.getMonth()
                                        ? 1
                                        : endDate.getDate() >= startDate.getDate() ? 0 : 1);
                        case 'M':
                            return endDate.getMonth() - startDate.getMonth()
                                + 12 * (endDate.getFullYear() - startDate.getFullYear())
                                - (endDate.getDate() >= startDate.getDate() ? 0 : 1);
                        case 'D':
                            startDate = startDate.getTime() - (startDate.getTimezoneOffset() == -402 ? -24124000 : startDate.getTimezoneOffset() * 60 * 1000);
                            endDate = endDate.getTime() - (endDate.getTimezoneOffset() == -402 ? -24124000 : endDate.getTimezoneOffset() * 60 * 1000);
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
                                + (endDate.getMonth() >= startDate.getMonth() ? 0 : 12)
                                - (endDate.getDate() >= startDate.getDate() ? 0 : 1);
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
                            startDate = startDate.getTime() - (startDate.getTimezoneOffset() == -402 ? -24124000 : startDate.getTimezoneOffset() * 60 * 1000);
                            endDate = endDate.getTime() - (endDate.getTimezoneOffset() == -402 ? -24124000 : endDate.getTimezoneOffset() * 60 * 1000);
                            var diff = (endDate - startDate) / (1000 * 3600 * 24);
                            return ((startDate <= endDate && endDate <= -2203915325000)
                                || (endDate >= startDate && startDate >= -2203915324000))
                                ? diff
                                : (startDate < endDate ? diff - 1 : diff);
                        default:
                            return '#NUM!';
                    }
                }";
        }

        private static string GetDayScript()
        {
            return @"
                function $DAY(date)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
                    }
                    if (date === undefined || date === '' || date == 0)
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
                        return '#VALUE!';
                    }
                    return date.getDate();
                }";
        }

        private static string GetDaysScript()
        {
            return @"
                function $DAYS(startDate, endDate)
                {
                   if (arguments.length !== 2) {
                        return 'Invalid Parameter';
                    }
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
                        return '#VALUE!';
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
                        return '#VALUE!';
                    }
                    startDate = startDate.getTime() - (startDate.getTimezoneOffset() == -402 ? -24124000 : startDate.getTimezoneOffset() * 60 * 1000);
                    endDate = endDate.getTime() - (endDate.getTimezoneOffset() == -402 ? -24124000 : endDate.getTimezoneOffset() * 60 * 1000);
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

        private static string GetHourScript()
        {
            return @"
                function $HOUR(date)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
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
                        return '#NUM!';
                    }
                    return date.getHours();
                }";
        }

        private static string GetMinuteScript()
        {
            return @"
                function $MINUTE(date)
                {
                   if (arguments.length === 0) {
                        return 'Invalid Parameter';
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
                                    return '#VALUE!';
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
                        return '#VALUE!';
                    }
                    return date.getMinutes();
                }";
        }

        private static string GetMonthScript()
        {
            return @"
                function $MONTH(date)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
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
                        return '#VALUE!';
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
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
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
                        return '#VALUE!';
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
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
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
                        return '#VALUE!';
                    }
                    return date.getFullYear();
                }";
        }

        private static string GetConcatScript()
        {
            return @"
                function $CONCAT(firstString)
                {
                    if (arguments.length === 0)
                    {
                        return 'Invalid Parameter';
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

        private static string GetFindScript()
        {
            return @"
                function $FIND(findText, withinText, startNum = 1)
                {
                    if (arguments.length > 3 || arguments.length < 2) {
                        return 'Invalid Parameter';
                    }
                    if(isNaN(startNum) || Number(startNum) < 1) {
                        return '#VALUE!';
                    }
                    findText = (findText == undefined) ? '' : findText;
                    withinText = (withinText == undefined ) ? '' : withinText; 
                    startNum = Number(startNum);
                    if(findText === '' && (withinText.toString().length + 1) >= startNum) {
                        return startNum;
                    }
                    if((findText === '' && withinText === '' && startNum > 1)
                        || (findText === '' && (withinText.toString().length + 1) < startNum)) {
                            return '#VALUE!';
                    }
                    var index = withinText.toString().indexOf(findText.toString(), startNum - 1);
                    if (index < 0)
                    {
                        return '#VALUE!';
                    }
                    return index + 1;
                }";
        }

        private static string GetLeftScript()
        {
            return @"
                function $LEFT(text, numChars = 1)
                {
                    if (arguments.length > 2 || arguments.length < 1) {
                        return 'Invalid Parameter';
                    }
                    numChars = Number(numChars);
                    return text.toString().substring(0, numChars);
                }";
        }

        private static string GetLenScript()
        {
            return @"
                function $LEN(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    return text.toString().length;
                }";
        }

        private static string GetLowerScript()
        {
            return @"
                function $LOWER(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    return text.toString().toLowerCase();
                }";
        }

        private static string GetMidScript()
        {
            return @"
                function $MID(text, startNum, numChars)
                {
                    if (arguments.length !== 3) {
                        return 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    if (isNaN(startNum) || isNaN(numChars))
                    {
                        return '#VALUE!';
                    }
                    startNum = Number(startNum);
                    numChars = Number(numChars);
                    if (startNum < 1 || numChars < 0)
                    {
                        return '#VALUE!';
                    }
                    return text.toString().substring(startNum - 1, startNum - 1 + numChars);
                }";
        }

        private static string GetRightScript()
        {
            return @"
                function $RIGHT(text, numChars = 1)
                {
                    if (arguments.length > 2 || arguments.length < 1) {
                        return 'Invalid Parameter';
                    }    
                    text = (text == undefined) ? '' : text;
                    if (isNaN(numChars) || Number(numChars) < 0)
                    {
                        return '#VALUE!';
                    }    
                    numChars = Number(numChars);
                    return text.toString().substring(text.toString().length - numChars);
                }";
        }

        private static string GetSubstituteScript()
        {
            return @"
                function $SUBSTITUTE(text, oldText, newtext, instanceNum)
                {
                    if (arguments.length > 4 || arguments.length < 3) {
                        return 'Invalid Parameter';
                    }     
                    if (Number(instanceNum) < 1)
                    {
                        return '#VALUE!';
                    }
                    text = (text == undefined) ? '' : text;
                    oldText = (oldText == undefined) ? '' : oldText;
                    newtext = (newtext == undefined) ? '' : newtext;
                    let reg = new RegExp(oldText.toString(), 'g');
                    if (instanceNum == undefined && arguments.length == 3)
                    {
                        return text.toString().replace(reg, newtext.toString());
                    }
                    instanceNum = Number(instanceNum);
                    let i = 0;
                    return text.toString().replace(reg, match => ++i == instanceNum ? newtext.toString() : match);
                }";
        }

        private static string GetTrimScript()
        {
            return @"
                function $TRIM(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    return text.toString().trim();
                }";
        }

        private static string GetUpperScript()
        {
            return @"
                function $UPPER(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    return text.toString().toUpperCase();
                }";
        }

        private static string GetAndScript()
        {
            return @"
                function $AND(firstClause)
                {
                    if (arguments.length == 0) {
                        return 'Invalid Parameter';
                    }
                    if(firstClause == 0 || firstClause === 'false') {
                        return false;
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
                            firstClause =  Boolean(arguments[i]);
                        }
                    }
                    firstClause =
                        firstClause == '0' || firstClause == 'false'
                            ? false
                            : firstClause == 'true'
                            ? true
                            : firstClause;
                    if (firstClause === undefined || firstClause === '' || isNaN(firstClause)) {
                        return '#VALUE!';
                    }
                    return Boolean(firstClause);
                }";
        }

        private static string GetIfScript()
        {
            return @"
                function $IF(expression, valueIfTrue, valueIfFalse = false)
                {
                    if (arguments.length > 3 || arguments.length < 2) {
                        return 'Invalid Parameter';
                    }
                    expression = (expression === undefined || expression === '') ? false  : expression;
                    valueIfTrue = (valueIfTrue === undefined) ? 0 : valueIfTrue;
                    valueIfFalse = (valueIfFalse === undefined) ? 0 : valueIfFalse;
                    if(typeof valueIfTrue === 'string' && valueIfTrue.length === 2 
                        && valueIfTrue.substring(0,1).charCodeAt() === 34 && valueIfTrue.substring(1,2).charCodeAt() == 34) 
                    {
                        valueIfTrue = '';
                    }
                    if(typeof valueIfFalse === 'string' && valueIfFalse.length === 2
                        && valueIfFalse.substring(0,1).charCodeAt() === 34 && valueIfFalse.substring(1,2).charCodeAt() == 34)
                    {
                        valueIfFalse = '';
                    }    
                    if (typeof expression === 'boolean')
                    {
                        return expression ? valueIfTrue : valueIfFalse;
                    }
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                        return expression ? valueIfTrue : valueIfFalse;
                    }
                    expression = ($VALUE(expression) != 0);
                    return expression ? valueIfTrue : valueIfFalse;
                }";
        }

        private static string GetNotScript()
        {
            return @"
                function $NOT(expression)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
                    }
                    expression = (expression == undefined) ? '' : expression;
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        return '#VALUE!';
                    }
                    return !expression;
                }";
        }

        private static string GetOrScript()
        {
            return @"
                function $OR(expression)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
                    }
                    for (let i = 1; i < arguments.length; i++) {        
                        if (arguments[i] === undefined || arguments[i].toString().trim() === '') {
                            continue;
                        }
                        arguments[i] = arguments[i] === 'true'  ? true : arguments[i];
                        if (typeof arguments[i] === 'boolean' || !isNaN(arguments[i])) { 
                            if (arguments[i] === true || Boolean(Number(arguments[i]))) {
                                return true;
                            }            
                            expression = Boolean(arguments[0]);
                        }
                    }
                    expression =
                        (!isNaN(expression) && Boolean(Number(expression))) || expression === 'true'
                            ? true
                            : expression == 'false' || expression == '0'
                            ? false
                            : expression;
                    if (expression === undefined || expression === '' || isNaN(expression)) {
                        return '#VALUE!';
                    }
                    return Boolean(expression);
                }";
        }

        private static string GetReplaceScript()
        {
            return @"
                function $REPLACE(oldText, startNum, numChars, newText)
                {
                    if (arguments.length !== 4)
                    {
                        return 'Invalid Parameter';
                    }
                    startNum = (startNum === undefined) ? 0 : startNum;
                    numChars = (numChars === undefined) ? 0 : numChars;
                    if (isNaN(startNum) || isNaN(numChars))
                    {
                        return 'Invalid Parameter';
                    }
                    startNum = Number(startNum);
                    numChars = Number(numChars);
                    if (startNum < 1 || numChars < 0)
                    {
                        return 'Invalid Parameter';
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

        private static string GetSearchScript()
        {
            return @"
                function $SEARCH(findText, withinText, start = 1)
                {
                    if (arguments.length < 2 || arguments.length > 3)
                    {
                        return 'Invalid Parameter';
                    }
                    if (start === '' || isNaN(start))
                    {
                        return '#VALUE!';
                    }
                    start = Number(start);
                    if (start < 1 || start > withinText.toString().length)
                    {
                        return '#VALUE!';
                    }
                    let index = withinText.toString().toLowerCase().indexOf(findText.toString().toLowerCase(), start - 1);
                    if (index < 0)
                    {
                        return 'Not Found';
                    }
                    return index + 1;
	            }";
        }

        private static string GetIfsScript()
        {
            return @"
                function $IFS(logicalTest, valueIfTrue)
                {
                    if (arguments.length === 0 || arguments.length % 2 !== 0) {
                        return 'Invalid Parameter';
                    }  
                    for (let i = 0; i < arguments.length; i = i + 2)
                    {
                        logicalTest = (arguments[i] === '' || arguments[i] === undefined ) ? false : arguments[i] 
                        logicalTest = (logicalTest === 'false' || logicalTest == '0') ? false : (logicalTest === 'true') ? true : logicalTest;
                        valueIfTrue = arguments[i+1] === undefined ? 0 : arguments[i+1];
                        if (!isNaN(logicalTest) || typeof logicalTest === 'boolean')
                        {       
                            if(Boolean(logicalTest)) {
                                retValue = $VALUE(valueIfTrue);
                                return (retValue == 'Invalid Parameter' || retValue == '#VALUE!') ? valueIfTrue : retValue;
                            }
                            logicalTest = Boolean(logicalTest);
                        }
                        else if(Boolean($VALUE(logicalTest))) {
                            retValue = $VALUE(valueIfTrue);
                            return (retValue == 'Invalid Parameter' || retValue == '#VALUE!') ? valueIfTrue : retValue;
                        }
                    }
                    if(logicalTest === false) {
                        return '#N/A';
                    }
	            }";
        }

        private static string GetIsEvenScript()
        {
            return @"
                function $ISEVEN(number)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    number = ( number == undefined  ||  number === '') ? 0 : number;
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
                function $ISNUMBER(value)
                {
                    if (arguments.length === 0)
                    {
                        return 'Invalid Parameter';
                    }
                    if(value === '' || value === undefined || typeof value === 'boolean') 
                    {
                        return false;
                    }
                    if(typeof value === 'number' || !isNaN(value))
                    {
                        return true;
                    }
                    return false;
	            }";
        }

        private static string GetIsOddScript()
        {
            return @"
                function $ISODD(number)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    number = ( number == undefined  ||  number === '') ? 0 : number;
                    if (isNaN(number) && typeof number === 'string')
                    {
                        return $DAYS(number, '1/2/2000') % 2 === 0;
                    }
                    number = Number(number);
                    return Math.trunc(number) % 2 !== 0;
	            }";
        }

        private static string GetIsTextScript()
        {
            return @"
                function $ISTEXT(text)
                {
                    if (arguments.length === 0) {
                        return 'Invalid Parameter';
                    }
                    if (text === ''
                        || text === undefined 
                        || typeof text === 'boolean' 
                        || typeof text === 'number' 
                        || !isNaN(text)
                        || !isNaN((new Date(text)).getTime())) 
                    {
                        return false;
                    }
                    return true;
	            }";
        }

        private static string GetModScript()
        {
            return @"
                function $MOD(number, divisor)
                {
                    if (arguments.length !== 2) {
                        return 'Invalid Parameter';
                    }
                    divisor = (divisor === undefined || divisor === '') ? 0
                        : (typeof divisor === 'boolean' && divisor) ? 1 
                        : (typeof divisor === 'boolean' && !divisor) ? 0
                        : divisor;
                    if(divisor == 0 || divisor == undefined) {
                        return '#DIV/0!';
                    }
                    number = (number === undefined || number === '') ? 0
                        : (typeof number === 'boolean' && number) ? 1 
                        : (typeof number === 'boolean' && !number) ? 0
                        : number;
                    number = $VALUE(number);
                    divisor = $VALUE(divisor);
                    return Math.abs(number) % divisor * (divisor > 0 ? 1 : -1);
	            }";
        }

        private static string GetOddScript()
        {
            return @"
                function $ODD(number)
                {
                    if (arguments.length === 0)
                    {
                        return 'Invalid Parameter';
                    } 
                    number = (number === undefined || number === '' || typeof number === 'boolean') ? 1 : number;
                    if(number === 1) {
                        return 1;
                    }
                    if (isNaN(number) && typeof number === 'string')
                    {   
                        number = $VALUE(number);
                    }
                    let result = Math.ceil(Number(number));
                    return (result % 2 === 0) ? result + (result > 0 ? 1 : -1) : result;
	            }";
        }

        private static string GetAverageScript()
        {
            return @"
                function $AVERAGE()
                {
                    if (arguments.length == 0 || arguments.length > 255)
                    {
                        return 'Invalid Parameter';
                    }
                    let sum = 0, averageCount = 0;
                    for (let i = 0; i < arguments.length; i++)
                    {
                        if (arguments[i] !== '' && arguments[i] !== undefined)
                        {
                            if(!isNaN(Number(arguments[i])) && typeof arguments[i] !== 'boolean') {
                                sum += Number(arguments[i]);
                                averageCount++;
                            }
                            else if(typeof arguments[i] === 'boolean') {
                                continue;
                            }
                            else {
                                try {
                                    sum += $VALUE(arguments[i]);
                                    averageCount++;
                                } 
                                catch {
                                    //No thing to do
                                }
                            }
                        }
                    }
                    if (averageCount === 0)
                    {
                        return '#DIV/0!';
                    }
                    return sum / averageCount;
	            }";
        }

        private static string GetWeekdayScript()
        {
            return @"
                function $WEEKDAY(date, returnType = 1)
                {
                    if (arguments.length < 1) {
                        return 'Invalid Parameter';
                    }
                    date = (date == undefined) ? 0 : date;
                    if(date === '' && (returnType === '' || Number(returnType) == 0)) {
                        return '#NUM!';
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
                        return '#NUM!';
                    }
                    returnType = Number(returnType)
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
                            return '#NUM!';
                    }
                }";
        }

        private static string GetMinScript()
        {
            return @"
                function $MIN(number1)
                {
                    if (arguments.length == 0 || arguments.length > 255)
                    {
                        return 'Invalid Parameter';
                    }
                    let minValue = Number.POSITIVE_INFINITY;
                    for (let i = 0; i < arguments.length; i++)
                    {
                        if (arguments[i] !== '' && !isNaN(arguments[i]) && Number(arguments[i]) < minValue)
                        {
                            minValue = Number(arguments[i]);
                        }
                    }
                    return minValue == Number.POSITIVE_INFINITY ? 0 : minValue;
                }";
        }

        private static string GetMaxScript()
        {
            return @"
                function $MAX(number1)
                {
                    if (arguments.length == 0 || arguments.length > 255)
                    {
                        return 'Invalid Parameter';
                    }
                    let maxValue = Number.NEGATIVE_INFINITY;
                    for (let i = 0; i < arguments.length; i++)
                    {
                        if (arguments[i] !== '' && !isNaN(arguments[i]) && Number(arguments[i]) > maxValue)
                        {
                            maxValue = Number(arguments[i]);
                        }
                    }
                    return maxValue == Number.NEGATIVE_INFINITY ? 0 : maxValue;
                }";
        }

        private static string GetRoundScript()
        {
            return @"
                function $ROUND(number, numDigits)
                {
                    if (arguments.length !== 2) {
                        return 'Invalid Parameter';
                    }
                    number = (number === undefined || number === '') ? 0
                            : (typeof number === 'boolean' && number) ? 1 
                            : (typeof number === 'boolean' && !number) ? 0 
                            : isNaN(Number(number)) ?  $VALUE(number)
                            : number;
                    numDigits = (numDigits === undefined || numDigits === '') ? 0 
                        : (typeof numDigits === 'boolean' && numDigits) ? 1 
                        : (typeof numDigits === 'boolean' && !numDigits) ? 0 
                        : isNaN(Number(numDigits)) ?  $VALUE(numDigits)
                        : numDigits;
                    if (isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        return '#VALUE!';
                    }
                    if (number == 0 && !isNaN(Number(numDigits))) {
                        return 0;
                    }
                    number = Number(number);
                    numDigits = Number(numDigits);
                    numDigits = numDigits > 30 ? 30 : numDigits;
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
                    return Number(result);
                }";
        }

        private static string GetRoundUpScript()
        {
            return @"
                function $ROUNDUP(number, numDigits)
                {
                    if (arguments.length !== 2) {
                        return 'Invalid Parameter';
                    }
                    number = (number === undefined || number === '') ? 0
                            : (typeof number === 'boolean' && number) ? 1 
                            : (typeof number === 'boolean' && !number) ? 0 
                            : isNaN(Number(number)) ?  $VALUE(number)
                            : number;
                    numDigits = (numDigits === undefined || numDigits === '') ? 0 
                        : (typeof numDigits === 'boolean' && numDigits) ? 1 
                        : (typeof numDigits === 'boolean' && !numDigits) ? 0 
                        : isNaN(Number(numDigits)) ?  $VALUE(numDigits)
                        : numDigits;
                    if (isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        return '#VALUE!';
                    }
                    if (number == 0 && !isNaN(Number(numDigits))) {
                        return 0;
                    }
                    number = Number(number);
                    numDigits = Number(numDigits);
                    numDigits = numDigits > 30 ? 30 : numDigits;
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
                    return Number(result);
                }";
        }

        private static string GetRoundDownScript()
        {
            return @"
                function $ROUNDDOWN(number, numDigits)
                {
                    if (arguments.length !== 2) {
                        return 'Invalid Parameter';
                    }
                    number =  (number === undefined || number === '') ? 0
                            : (typeof number === 'boolean' && number) ? 1 
                            : (typeof number === 'boolean' && !number) ? 0 
                            : isNaN(Number(number)) ?  $VALUE(number)
                            : number;
                    numDigits = (numDigits === undefined || numDigits === '') ? 0 
                        : (typeof numDigits === 'boolean' && numDigits) ? 1 
                        : (typeof numDigits === 'boolean' && !numDigits) ? 0 
                        : isNaN(Number(numDigits)) ?  $VALUE(numDigits)
                        : numDigits;
                    if (isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        return '#VALUE!';
                    }
                    if (number == 0 && !isNaN(Number(numDigits))) {
                        return 0;
                    }
                    number = Number(number);
                    numDigits = Number(numDigits);
                    numDigits = numDigits > 30 ? 30 : numDigits;
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
                    return Number(result);
                }";
        }

        private static string GetTruncScript()
        {
            return @"
                function $TRUNC(number, numDigits)
                {
                    if (arguments.length > 2 || arguments.length  < 1) {
                        return 'Invalid Parameter';
                    }
                    number = (number === undefined || number === '') ? 0
                            : (typeof number === 'boolean' && number) ? 1 
                            : (typeof number === 'boolean' && !number) ? 0 
                            : isNaN(Number(number)) ?  $VALUE(number)
                            : number;
                    numDigits = (numDigits === undefined || numDigits === '') ? 0 
                        : (typeof numDigits === 'boolean' && numDigits) ? 1 
                        : (typeof numDigits === 'boolean' && !numDigits) ? 0 
                        : isNaN(Number(numDigits)) ?  $VALUE(numDigits)
                        : numDigits;    
                    if (isNaN(Number(number)) || isNaN(Number(numDigits)))
                    {
                        return '#VALUE!';
                    }
                    if (number === 0 && !isNaN(Number(numDigits))) {
                        return 0;
                    }
    
                    number = Number(number);
                    numDigits = Number(numDigits);
                    numDigits = numDigits > 30 ? 30 : numDigits;
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

        private static string GetAscScript()
        {
            return @"
                function $ASC(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    if(text === undefined || text === '') {
                        return '';
                    }
                    let kanaMap = {
                        'ガ': 'ｶﾞ', 'ギ': 'ｷﾞ', 'グ': 'ｸﾞ', 'ゲ': 'ｹﾞ', 'ゴ': 'ｺﾞ', 'ザ': 'ｻﾞ', 'ジ': 'ｼﾞ', 'ズ': 'ｽﾞ', 'ゼ': 'ｾﾞ', 'ゾ': 'ｿﾞ', 'ダ': 'ﾀﾞ', 
                        'ヂ': 'ﾁﾞ', 'ヅ': 'ﾂﾞ', 'デ': 'ﾃﾞ', 'ド': 'ﾄﾞ', 'バ': 'ﾊﾞ', 'ビ': 'ﾋﾞ', 'ブ': 'ﾌﾞ', 'ベ': 'ﾍﾞ', 'ボ': 'ﾎﾞ', 'パ': 'ﾊﾟ', 'ピ': 'ﾋﾟ', 
                        'プ': 'ﾌﾟ', 'ペ': 'ﾍﾟ', 'ポ': 'ﾎﾟ', 'ヴ': 'ｳﾞ', 'ヷ': 'ﾜﾞ', 'ヺ': 'ｦﾞ', 'ア': 'ｱ', 'イ': 'ｲ', 'ウ': 'ｳ', 'エ': 'ｴ', 'オ': 'ｵ', 
                        'カ': 'ｶ', 'キ': 'ｷ', 'ク': 'ｸ', 'ケ': 'ｹ', 'コ': 'ｺ', 'サ': 'ｻ', 'シ': 'ｼ', 'ス': 'ｽ', 'セ': 'ｾ', 'ソ': 'ｿ', 'タ': 'ﾀ', 'チ': 'ﾁ', 
                        'ツ': 'ﾂ', 'テ': 'ﾃ', 'ト': 'ﾄ', 'ナ': 'ﾅ', 'ニ': 'ﾆ', 'ヌ': 'ﾇ', 'ネ': 'ﾈ', 'ノ': 'ﾉ', 'ハ': 'ﾊ', 'ヒ': 'ﾋ', 'フ': 'ﾌ', 'ヘ': 'ﾍ', 
                        'ホ': 'ﾎ', 'マ': 'ﾏ', 'ミ': 'ﾐ', 'ム': 'ﾑ', 'メ': 'ﾒ', 'モ': 'ﾓ', 'ヤ': 'ﾔ', 'ユ': 'ﾕ', 'ヨ': 'ﾖ', 'ラ': 'ﾗ', 'リ': 'ﾘ', 'ル': 'ﾙ', 
                        'レ': 'ﾚ', 'ロ': 'ﾛ', 'ワ': 'ﾜ', 'ヲ': 'ｦ', 'ン': 'ﾝ', 'ァ': 'ｧ', 'ィ': 'ｨ', 'ゥ': 'ｩ', 'ェ': 'ｪ', 'ォ': 'ｫ', 'ッ': 'ｯ', 'ャ': 'ｬ', 
                        'ュ': 'ｭ', 'ョ': 'ｮ', '！': '!', '”': String.fromCharCode(34), '＃': '#', '＄': '$', '％': '%', '＆': '&', '’': String.fromCharCode(39),
                        '（': '(', '）': ')', '＊': '*', '＋': '+', '，': ',', '－': '-', '．': '.', '／': '/', '：': ':', '；': ';', '＜': '<', '＝': '=', 
                        '＞': '>', '？': '?', '＠': '@', '［': '[', '￥': '￥', '］': ']', '＾': '^', '＿': '_', '‘': '`', '｛': '{', '｜': '|', '｝': '}', '～': '~' 
                    };
                    let reg = new RegExp(`(${Object.keys(kanaMap).join('|')})`, 'g');
                    return text
                        .toString()
                        .replace(reg, function (match) {
                            return kanaMap[match];
                        })
                        .replace(/[Ａ-Ｚａ-ｚ０-９]/g, function (s) {
                            return String.fromCharCode(s.charCodeAt(0) - 0xfee0);
                        });
                }";
        }

        private static string GetJisScript()
        {
            return @"
                function $JIS(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    if(text === undefined || text === '') {
                        return '';
                    }
                    let halfKanaMap = {
                        'ｶﾞ': 'ガ', 'ｷﾞ': 'ギ', 'ｸﾞ': 'グ', 'ｹﾞ': 'ゲ', 'ｺﾞ': 'ゴ', 'ｻﾞ': 'ザ', 'ｼﾞ': 'ジ', 'ｽﾞ': 'ズ', 'ｾﾞ': 'ゼ', 'ｿﾞ': 'ゾ', 'ﾀﾞ': 'ダ', 'ﾁﾞ': 'ヂ', 
                        'ﾂﾞ': 'ヅ', 'ﾃﾞ': 'デ', 'ﾄﾞ': 'ド', 'ﾊﾞ': 'バ', 'ﾋﾞ': 'ビ', 'ﾌﾞ': 'ブ', 'ﾍﾞ': 'ベ', 'ﾎﾞ': 'ボ', 'ﾊﾟ': 'パ', 'ﾋﾟ': 'ピ', 'ﾌﾟ': 'プ', 
                        'ﾍﾟ': 'ペ', 'ﾎﾟ': 'ポ', 'ｳﾞ': 'ヴ', 'ﾜﾞ': 'ヷ', 'ｦﾞ': 'ヺ', 'ｱ': 'ア', 'ｲ': 'イ', 'ｳ': 'ウ', 'ｴ': 'エ', 'ｵ': 'オ', 'ｶ': 'カ', 'ｷ': 'キ', 
                        'ｸ': 'ク', 'ｹ': 'ケ', 'ｺ': 'コ', 'ｻ': 'サ', 'ｼ': 'シ', 'ｽ': 'ス', 'ｾ': 'セ', 'ｿ': 'ソ', 'ﾀ': 'タ', 'ﾁ': 'チ', 'ﾂ': 'ツ', 'ﾃ': 'テ', 'ﾄ': 'ト',
                        'ﾅ': 'ナ', 'ﾆ': 'ニ', 'ﾇ': 'ヌ', 'ﾈ': 'ネ', 'ﾉ': 'ノ', 'ﾊ': 'ハ', 'ﾋ': 'ヒ', 'ﾌ': 'フ', 'ﾍ': 'ヘ', 'ﾎ': 'ホ', 'ﾏ': 'マ', 'ﾐ': 'ミ', 'ﾑ': 'ム', 
                        'ﾒ': 'メ', 'ﾓ': 'モ', 'ﾔ': 'ヤ', 'ﾕ': 'ユ', 'ﾖ': 'ヨ', 'ﾗ': 'ラ', 'ﾘ': 'リ', 'ﾙ': 'ル', 'ﾚ': 'レ', 'ﾛ': 'ロ', 'ﾜ': 'ワ', 'ｦ': 'ヲ', 'ﾝ': 'ン', 
                        'ｧ': 'ァ', 'ｨ': 'ィ', 'ｩ': 'ゥ', 'ｪ': 'ェ', 'ｫ': 'ォ', 'ｯ': 'ッ', 'ｬ': 'ャ', 'ｭ': 'ュ', 'ｮ': 'ョ', 
                    };    
                    let specialCharMaping = {
                        '!': '！', '#': '＃', '$': '＄', '%': '％', '&': '＆', '\'': '’', '(': '（', ')': '）', '*': '＊', '+': '＋', ',': '，', '-': '－', 
                        '.': '．', '/': '／', ':': '：', ';': '；', '<': '＜', '=': '＝', '>': '＞', '?': '？', '@': '＠', '[': '［', '\\': '￥', ']': '］',
                        '^': '＾', '_': '＿', '`': '‘', '{': '｛', '|': '｜', '}': '｝', '~': '～' 
                    };
                    specialCharMaping[String.fromCharCode(34)] = '”';
                    let specialCharRegexPattern = new RegExp(`[${Object.keys(specialCharMaping).join('')}]`, 'g');
                    let kanaMapRegxPattern = new RegExp(`(${Object.keys(halfKanaMap).join('|')})`, 'g');
                    return text
                        .toString()
                        .replace(kanaMapRegxPattern, function (matKana) {
                                return halfKanaMap[matKana];
                        })
                        .replace (/ﾞ/g, '゛')
                        .replace (/ﾟ/g, '゜')
                        .replace(specialCharRegexPattern, function (matchSpc) {
                            return specialCharMaping[matchSpc];
                        })   
                        .replace(/[A-Za-z0-9]/g, function (s) {
                            return String.fromCharCode(s.charCodeAt(0) + 0xfee0);
                    });
                }";
        }

        private static string GetValueScript()
        {
            return @"
                function $VALUE(text)
                {
                    if (arguments.length !== 1) {
                        return 'Invalid Parameter';
                    }
                    if(typeof text === 'boolean' || text === undefined || text === '') {
                        return '#VALUE!';
                    }    
                    if (!isNaN(text)) {
                        return Number(text);
                    } 
                    text = text
                            .toString()
                            .replace(/[０-９]/g, function (s) {
                                return String.fromCharCode(s.charCodeAt(0) - 0xfee0);
                            });
                    if (!isNaN(text)) {
                        return Number(text);
                    }
                    return '#VALUE!';
                }";
        }

        private static string GetTextScript()
        {
            return @"
                function $TEXT(value, format)
                {
                    if (arguments.length != 2 || value === undefined || format === undefined)
                    {
                        return 'Invalid Parameter';
                    }
                    if ((value.toString().toLowerCase() === 'true' || value.toString().toLowerCase() === 'false')
                        && (format === '' || !isNaN(format)))
                    {
                        return value;
                    }
                    var reg = new RegExp('^0+$', 'g');
                    if (reg.test(format))
                    {
                        return FormulaServerScriptUtilities.GetText(value, '#' + format, context);
                    }
                    if (format.toString().toLowerCase() === 'true'
                        || format.toString().toLowerCase() === 'false'
                        || !isNaN(Date.parse(format)))
                    {
                        return '#VALUE!';
                    }
                    return FormulaServerScriptUtilities.GetText(value, format, context);
                }";
        }

        private static string GetAbsScript()
        {
            return @"
                function $ABS(number)
                {
                    if (arguments.length === 0 || number === undefined || number === '')
                    {
                        return 'Invalid Parameter';
                    }
                    if (number.toString().toLowerCase() === 'true')
                    {
                        number = true;
                    }
                    else if (number.toString().toLowerCase() === 'false')
                    {
                        number = false;
                    }
                    if (isNaN(number))
                    {
                        return '#VALUE!';
                    }
                    else
                    {
                        return Math.abs(Number(number));
                    }
	            }";
        }

        private static string GetPowerScript()
        {
            return @"
                function $POWER(number, power)
                {
                    if (arguments.length != 2 || number === undefined || power === undefined || number === '' || power === '')
                    {
                        return 'Invalid Parameter';
                    }
                    if (number.toString().toLowerCase() === 'true')
                    {
                        number = true;
                    }
                    else if (number.toString().toLowerCase() === 'false')
                    {
                        number = false;
                    }
                    if (isNaN(number))
                    {
                        return '#VALUE!';
                    }
                    if (power.toString().toLowerCase() === 'true')
                    {
                        power = true;
                    }
                    else if (power.toString().toLowerCase() === 'false')
                    {
                        power = false;
                    }
                    if (isNaN(power))
                    {
                        return '#VALUE!';
                    }
                    if (((Number(number) == 0 || !number) && (Number(power) == 0 || !power)) || (Number(number) < 0 && Number(power) % 1 !== 0))
                    {
                        return '#NUM!';
                    }
                    var result = Math.pow(Number(number), Number(power));
                    if (result == Number.POSITIVE_INFINITY || result == Number.NEGATIVE_INFINITY)
                    {
                        return '#NUM!';
                    }
                    return result;
	            }";
        }

        private static string GetRandScript()
        {
            return @"
                function $RAND()
                {
                    return Math.random();
	            }";
        }

        private static string GetSqrtScript()
        {
            return @"
                function $SQRT(number)
                {
                    if (arguments.length === 0 || number === undefined || number === '')
                    {
                        return 'Invalid Parameter';
                    }
                    if (number.toString().toLowerCase() === 'true')
                    {
                        number = true;
                    }
                    else if (number.toString().toLowerCase() === 'false')
                    {
                        number = false;
                    }
                    if (isNaN(number))
                    {
                        return '#VALUE!';
                    }
                    else
                    {
                        if (Number(number) < 0)
                        {
                            return '#NUM!';
                        }
                        return Math.sqrt(Number(number));
                    }
	            }";
        }

        private static string GetEOMonthScript()
        {
            return @"
                function $EOMONTH(start_date, months)
                {
                    if (arguments.length != 2 || months === undefined || start_date === undefined)
                    {
                        return 'Invalid Parameter';
                    }
                    if (start_date === '')
                    {
                        start_date = 1;
                    }
                    var datePart = start_date.split(' ')[0].toString().split('/');
                    if (isNaN(start_date))
                    {
                        if (isNaN(Date.parse(start_date)))
                        {
                            return '#VALUE!';
                        }
                        else
                        {
                            start_date = new Date(Date.parse(start_date));
                        }
                    }
                    else
                    {
                        return '#VALUE!';
                    }
                    if (isNaN(months))
                    {
                        if (isNaN(Date.parse(months)))
                        {
                            return '#VALUE!';
                        }
                        else
                        {
                            months = $DAYS(months, '1900/01/01') + 1;
                        }
                    }
                    else
                    {
                        months = Number(months);
                    }
                    if (isNaN(start_date.getTime()) || start_date.getFullYear() < 1900 || start_date.getFullYear() > 9999)
                    {
                        return 'Invalid Parameter';
                    }
                    if (datePart.length == 3 && datePart[2] != start_date.getDate())
                    {
                        return '#VALUE!';
                    }
                    var d = new Date(start_date.getFullYear(), start_date.getMonth() + Number(months) + 1, 0);
                    if (isNaN(d.getTime()) || d.getFullYear() < 1900 || d.getFullYear() > 9999)
                    {
                        return '#NUM!';
                    }
                    return d.getFullYear()
                        + '/' + ('0' + (d.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + d.getDate()).slice(-2);
                }";
        }

        private static string GetIsBlankScript()
        {
            return @"
                function $ISBLANK(value)
                {
                    if (arguments.length != 1)
                    {
                        return 'Invalid Parameter';
                    }
                    return value === undefined || value === '';
                }";
        }

        private static string GetIsErrorScript()
        {
            return @"
                function $ISERROR(value)
                {
                    if (arguments.length != 1)
                    {
                        return 'Invalid Parameter';
                    }
                    return value == '#N/A'
                        || value == '#VALUE!'
                        || value == '#REF!'
                        || value == '#DIV/0!'
                        || value == '#NUM!'
                        || value == '#NAME?'
                        || value == '#NULL!'
                        || value == 'Invalid Parameter'
                        || !Number.isFinite(value);
                }";
        }

        private static string GetIfErrorScript()
        {
            return @"
                function $IFERROR(value, value_if_error)
                {
                    if (arguments.length != 2)
                    {
                        return 'Invalid Parameter';
                    }
                    return value === undefined || value === ''
                        ? 0
                        : ($ISERROR(value) ? value_if_error : value);
                }";
        }

        private static string GetDateTimeScript()
        {
            return @"
                function $DATETIME(year, month, day, hour, minute, second)
                {
                    if (arguments.length != 6)
                    {
                        return 'Invalid Parameter';
                    }
                    year = (year === undefined || year === '' || year === '0') ? 0 : year;
                    month = (month === undefined || month === '' || month === '0') ? 0 : month;
                    day = (day === undefined || day === '' || day === '0') ? 0 : day;
                    hour = (hour === undefined || hour === '' || hour === '0') ? 0 : hour;
                    minute = (minute === undefined || minute === '' || minute === '0') ? 0 : minute;
                    second = (second === undefined || second === '' || second === '0') ? 0 : second;
                    if (isNaN(year) || isNaN(month) || isNaN(day) || isNaN(hour) || isNaN(minute) || isNaN(second))
                    {
                        return '#NUM!';
                    }
                    year = Number(year);
                    month = Number(month);
                    day = Number(day);
                    hour = Number(hour);
                    minute = Number(minute);
                    second = Number(second);
                    if (year === 0 && month === 1 && day === 0 && hour === 0 && minute === 0 && second === 0)
                    {
                        return '1900/01/00 00:00:00';
                    }    
                    if (year >= 0 && year < 1900)
                    {
                        year = 1900 + year;
                    }
                    var date = new Date(year, month - 1, day, hour, minute, second);
                    if (isNaN(date.getTime()) || date.getFullYear() < 1900 || date.getFullYear() > 9999)
                    {
                        return '#NUM!';
                    }
                    return date.getFullYear()
                        + '/' + ('0' + (date.getMonth() + 1)).slice(-2)
                        + '/' + ('0' + date.getDate()).slice(-2)
                        + ' ' + ('0' + (date.getHours())).slice(-2)
                        + ':' + ('0' + (date.getMinutes())).slice(-2)
                        + ':' + ('0' + date.getSeconds()).slice(-2);
                }";
        }
    }
}