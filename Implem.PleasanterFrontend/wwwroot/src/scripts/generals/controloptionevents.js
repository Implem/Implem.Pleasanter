$(document).on('click', '.current-time', function () {
    var $control = $(this).prev();
    var date = new Date();
    var timeZoneOffset = $('#TimeZoneOffset').val();
    var dateTime;
    switch ($control.attr('data-format')) {
        case 'Y/m/d':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY/MM/DD');
            break;
        case 'Y.m.d':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD');
            break;
        case 'Y.m.d.':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD.');
            break;
        case 'm/d/Y':
            dateTime = moment().utcOffset(timeZoneOffset).format('MM/DD/YYYY');
            break;
        case 'Y/m/d H:i':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY/MM/DD HH:mm');
            break;
        case 'Y.m.d H:i':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD HH:mm');
            break;
        case 'Y.m.d. H:i':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD. HH:mm');
            break;
        case 'm/d/Y H:i':
            dateTime = moment().utcOffset(timeZoneOffset).format('MM/DD/YYYY HH:mm');
            break;
        case 'Y/m/d H:i:s':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY/MM/DD HH:mm:ss');
            break;
        case 'Y.m.d H:i:s':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD HH:mm:ss');
            break;
        case 'Y.m.d. H:i:s':
            dateTime = moment().utcOffset(timeZoneOffset).format('YYYY.MM.DD. HH:mm:ss');
            break;
        case 'm/d/Y H:i:s':
            dateTime = moment().utcOffset(timeZoneOffset).format('MM/DD/YYYY HH:mm:ss');
            break;
    }
    $p.set($control, dateTime);
    $control.change();
});
$(document).on('click', '.current-dept', function () {
    var $control = $(this).prev();
    $p.set($control, $p.deptId());
});
$(document).on('click', '.current-user', function () {
    var $control = $(this).prev();
    $p.set($control, $p.userId());
});
