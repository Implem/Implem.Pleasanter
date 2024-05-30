$p.dateAdd = function (type, num, date) {
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

$p.dateTimeFormatString = function (date, language) {
    if (date === undefined) date = new Date();
    var dateTime;
    switch (language) {
        case undefined:
        case 'en':
            dateTime = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
            break;
        case 'de':
            dateTime = date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate();
            break;
        case 'ko':
            dateTime = date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate() + '.';
            break;
        default:
            dateTime =  date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
    }
    return dateTime +
        (date.getHours() + date.getMinutes() !== 0
            ? ' ' + ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2)
            : '');
}

$p.transferedDate = function (format, datetimeString) {
    // 指定された日付書式でDate型オブジェクトを生成出来ない場合があるため、YYYY/MM/DD形式の日付書式に変換してDate型オブジェクトを生成します
    return new Date(moment(datetimeString, format.toUpperCase()).format('YYYY/MM/DD'));
}