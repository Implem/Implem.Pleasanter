$p.openSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    error = $p.send($control);
    if (error === 0) {
        $('#SetNumericRangeDialog').dialog({
            modal: true,
            height: 'auto',
            width: '700px',
            resizable: false,
            position: { of: window }
        });
    }
}
$p.openSiteSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.openSiteSettingsDialog($control, '#SetNumericRangeDialog', 'auto');
    $target = $('#' + $control.attr('id').replace("_NumericRange", ""));
    var initValue = JSON.parse($target.val() || "null");
    var startValue = "";
    var endValue = "";
    if (Array.isArray(initValue) && initValue.length > 0) {
        var values = initValue[0].split(',');
        if (values.length > 0) {
            startValue = values[0];
        }
        if (values.length > 1) {
            endValue = values[1];
        }
    }
    $('#numericRangeStart').val(startValue);
    $('#numericRangeEnd').val(endValue);
}
$p.openSetNumericRangeOK = function ($controlID) {
    $start = $('#numericRangeStart');
    $end = $('#numericRangeEnd');
    $start.validate();
    $end.validate();
    if (!$start.valid() || !$end.valid()) {
        $p.setErrorMessage('ValidationError');
        return false;
    }
    $control = $('#' + $controlID);
    $target = $('#' + $controlID.replace("_NumericRange", ""));
    var sdval = $("#numericRangeStart").val();
    var edval = $("#numericRangeEnd").val();
    var setval = "";
    var dispval = "";
    if (sdval || edval) {
        dispval = sdval + " - " + edval;
        setval = '["'+ sdval + ',' + edval + '"]';
    }
    $control.val(dispval);
    $p.set($target, setval);
    $('#SetNumericRangeDialog').dialog("close");
    $p.send($target);
}
$p.openSetNumericRangeClear = function ($control) {
    $("#numericRangeStart").val("");
    $("#numericRangeEnd").val("");
    $p.clearMessage();
}