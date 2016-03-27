function dateAdd(type, num, date) {
    switch (type) {
        case 'd':
            return new Date(date.setDate(date.getDate() + num));
    }
}

function dateDiff(type, date1, date2) {
    switch (type) {
        case 'd':
            return parseInt((date1 - date2) / 86400000);
    }
}

function shortDate(date) {
    if (date === undefined) date = new Date();
    return new Date(date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate());
}