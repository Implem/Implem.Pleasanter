$p.openSetDateRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#SetDateRangeDialog').dialog({
            autoOpen: false,
            modal: true,
            height: 'auto',
            width: '700px',
            resizable: false,
            position: { of: window },
            close: function () {
                $(this).find('.datepicker').datetimepicker('remove');
            }
        });
        $('#SetDateRangeDialog').dialog('open');
    }
}
$p.openSiteSetDateRangeDialog = function ($control, timepicker) {
    $control.blur();
    $p.openSiteSettingsDialog($control, '#SetDateRangeDialog', 'auto');
    $target = $('[id="' + $control.attr('id').replace('_DateRange', '') + '"]');
    var initValue = JSON.parse($target.val() || 'null');
    var startValue = '';
    var endValue = '';
    if (Array.isArray(initValue) && initValue.length > 0) {
        var values = initValue[0].split(',');
        if (values.length > 0) {
            startValue = timepicker ? values[0] : values[0].split(' ')[0];
        }
        if (values.length > 1) {
            endValue = timepicker ? values[1] : values[1].split(' ')[0];
        }
    }
    $('#dateRangeStart').val(startValue);
    $('#dateRangeEnd').val(endValue);
}
$p.openSetDateRangeOK = function ($controlID, type) {
    var sdval = $('#dateRangeStart').val();
    var edval = $('#dateRangeEnd').val();
    var setval = '["' + type + '"]';
    var dispval = $p.display(type);
    switch (type) {
        case 'Today':
        case 'ThisMonth':
        case 'ThisYear':
            break;
        default:
            if (sdval || edval) {
                dispval = sdval + ' - ' + edval;
                if (type !== 'DateTimepicker' && sdval) { sdval += ' 00:00:00.000'; }
                if (type !== 'DateTimepicker' && edval) { edval += ' 23:59:59.997'; }
                setval = '["' + sdval + ',' + edval + '"]';
            }
            break;
    }
    $control = $('[id="' + $controlID + '"]');
    $target = $('[id="' + $controlID.replace('_DateRange', '') + '"]');
    $control.val(dispval);
    $p.set($target, setval);
    $('#SetDateRangeDialog').dialog('close');
    $p.send($target);
}
$p.openSetDateRangeClear = function () {
    $('#dateRangeStart').val('');
    $('#dateRangeEnd').val('');
}