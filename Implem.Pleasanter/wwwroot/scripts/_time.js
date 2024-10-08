﻿$p.dateAdd = function (type, num, date) {
    switch (type) {
        case 'd':
            return new Date(new Date(date).setDate(date.getDate() + num));
    }
}

$p.dateDiff = function (type, date1, date2) {
    switch (type) {
        case 'd':
            return parseInt((date1 - date2) / 86400000);
    }
}

$p.shortDate = function (date) {
    if (date === undefined) date = new Date();
    return new Date($p.shortDateString(date));
}

$p.beginningMonth = function (date) {
    if (date === undefined) date = new Date();
    return new Date(date.getFullYear() + '/' + (date.getMonth() + 1) + '/1');
}

$p.shortDateString = function (date) {
    if (date === undefined) date = new Date();
    return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
}

$p.dateTimeString = function (date) {
    if (date === undefined) date = new Date();
    return $p.shortDateString(date) +
        (date.getHours() + date.getMinutes() !== 0
            ? ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2)
            : '');
}

$p.dateTimeFormatString = function (date, dateFormat) {
    if (date === undefined) date = new Date();
    switch (dateFormat) {
        case 'MM/dd/yyyy':
             return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
            break;
        case 'yyyy/MM/dd':
             return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
            break;
        case 'yyyy.MM.dd.':
            return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate() + '. ';
            break;
        case 'yyyy.MM.dd':
             return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate();
            break;
        case 'MM/dd/yyyy HH:mm':
             return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2);
            break;
        case 'yyyy/MM/dd HH:mm':
            return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2);
            break;
        case 'yyyy.MM.dd. HH:mm':
            return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()
                + '. ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2);
            break;
        case 'yyyy.MM.dd HH:mm':
            return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2);
            break;
        case 'MM/dd/yyyy HH:mm:ss':
             return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
            break;
        case 'yyyy/MM/dd HH:mm:ss':
            return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
            break;
        case 'yyyy.MM.dd. HH:mm:ss':
             return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()
                + '. ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
            break;
        case 'yyyy.MM.dd HH:mm:ss':
            return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
        default:
            (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
                + ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
    }
}

$p.transferedDate = function (format, datetimeString) {
    return new Date(moment(datetimeString, format.toUpperCase()).format('YYYY/MM/DD'));
}