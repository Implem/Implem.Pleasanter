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

$p.shortDateString = function (date) {
    if (date === undefined) date = new Date();
    return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
}

$p.dateTimeString = function (date) {
    if (date === undefined) date = new Date();
    return $p.shortDateString(date) +
        (date.getHours() + date.getMinutes() !== 0
            ? ' ' + date.getHours() + ':' + date.getMinutes()
            : '');
}