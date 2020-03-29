$p.openSetDateRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    error = $p.send($control);
    if (error === 0) {
        $('#SetDateRangeDialog').dialog({
            autoOpen: false,
            modal: true,
            height: 'auto',
            width: '700px',
            resizable: false,
            position: { of: window }
        });
        $('#SetDateRangeDialog').dialog("open");
    }
}
$p.openSiteSetDateRangeDialog = function ($control, timepicker) {
    $control.blur();
    $p.openSiteSettingsDialog($control, '#SetDateRangeDialog', 'auto');
    $target = $('#' + $control.attr('id').replace("_DateRange", ""));
    var initValue = JSON.parse($target.val() || "null");
    var startValue = "";
    var endValue = "";
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
$p.openSetDateRangeOK = function ($controlID, timepicker) {
    var sdval = $('#dateRangeStart').val();
    var edval = $('#dateRangeEnd').val();
    var setval = "";
    var dispval = "";
    if (sdval || edval) {
        dispval = sdval + "-" + edval;
        if (!timepicker && sdval) { sdval += " 00:00:00.000"; }
        if (!timepicker && edval) { edval += " 23:59:59.997"; }
        setval = '["' + sdval + ',' + edval + '"]';
    }
    $control = $('#' + $controlID);
    $target = $('#' + $controlID.replace("_DateRange", ""));
    $control.val(dispval);
    $p.set($target, setval);
    $('#SetDateRangeDialog').dialog("close");
    $p.send($target);
}
$p.openSetDateRangeClear = function () {
    $('#dateRangeStart').val("");
    $('#dateRangeEnd').val("");
}