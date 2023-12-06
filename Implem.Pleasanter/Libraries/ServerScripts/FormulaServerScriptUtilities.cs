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

        public static object Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            string formulaScript)
        {
            var data = ServerScriptUtilities.Values(
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
                .Replace("$value(", "$VALUE(", StringComparison.InvariantCultureIgnoreCase);
        }

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

        private static string GetDayScript()
        {
            return @"
                function $DAY(date)
                {
                    if (arguments.length === 0) {
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

        private static string GetDaysScript()
        {
            return @"
                function $DAYS(startDate, endDate)
                {
                   if (arguments.length !== 2) {
                        throw 'Invalid Parameter';
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

        private static string GetHourScript()
        {
            return @"
                function $HOUR(date)
                {
                    if (arguments.length === 0) {
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

        private static string GetMinuteScript()
        {
            return @"
                function $MINUTE(date)
                {
                   if (arguments.length === 0) {
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

        private static string GetMonthScript()
        {
            return @"
                function $MONTH(date)
                {
                    if (arguments.length === 0) {
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

        private static string GetFindScript()
        {
            return @"
                function $FIND(findText, withinText, startNum = 1)
                {
                    if (arguments.length > 3 || arguments.length < 2) {
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

        private static string GetLeftScript()
        {
            return @"
                function $LEFT(text, numChars = 1)
                {
                    if (arguments.length > 2 || arguments.length < 1) {
                        throw 'Invalid Parameter';
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
                        throw 'Invalid Parameter';
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
                        throw 'Invalid Parameter';
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
                        throw 'Invalid Parameter';
                    }
                    text = (text == undefined) ? '' : text;
                    if (isNaN(startNum) || isNaN(numChars))
                    {
                        throw '#VALUE!';
                    }
                    startNum = Number(startNum);
                    numChars = Number(numChars);
                    if (startNum < 1 || numChars < 0)
                    {
                        throw '#VALUE!';
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
                        throw 'Invalid Parameter';
                    }    
                    text = (text == undefined) ? '' : text;
                    if (isNaN(numChars) || Number(numChars) < 0)
                    {
                        throw '#VALUE!';
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
                        throw 'Invalid Parameter';
                    }     
                    if (Number(instanceNum) < 1)
                    {
                        throw '#VALUE!';
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
                        throw 'Invalid Parameter';
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
                        throw 'Invalid Parameter';
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
                        throw 'Invalid Parameter';
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
                        throw '#VALUE!';
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
                        throw 'Invalid Parameter';
                    }
                    expression = (expression == undefined) ? '' : expression;
                    valueIfTrue = (valueIfTrue == undefined) ? '' : valueIfTrue;
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        throw '#VALUE!';
                    }
                    return expression ? valueIfTrue : valueIfFalse;
                }";
        }

        private static string GetNotScript()
        {
            return @"
                function $NOT(expression)
                {
                    if (arguments.length === 0) {
                        throw 'Invalid Parameter';
                    }
                    expression = (expression == undefined) ? '' : expression;
                    if (!isNaN(expression))
                    {
                        expression = (expression != 0);
                    }
                    else if (typeof expression != 'boolean')
                    {
                        throw '#VALUE!';
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
                        throw 'Invalid Parameter';
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
                            expression =  false;
                        }
                    }
                    expression =
                        (!isNaN(expression) && Boolean(Number(expression))) || expression === 'true'
                            ? true
                            : expression == 'false' || expression == '0'
                            ? false
                            : expression;
                    if (expression === undefined || expression === '' || isNaN(expression)) {
                        throw '#VALUE!';
                    }
                    return Boolean(expression);
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
    }
}