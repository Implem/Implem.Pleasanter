$p.openSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    var error = $p.syncSend($control);
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
    if (!$('#SetNumericRangeDialog').hasClass('loop')) {
        $p.openSiteSettingsDialog($control, '#SetNumericRangeDialog', 'auto');
        $target = $('[id="' + $control.attr('id').replace('_NumericRange', '') + '"]');
        var initValue = JSON.parse($target.val() || 'null');
        var startValue = '';
        var endValue = '';
        if (Array.isArray(initValue) && initValue.length > 0) {
            var values = initValue[0].split(',');
            if (values.length > 0) {
                startValue = values[0];
            }
            if (values.length > 1) {
                endValue = values[1];
            }
        }
        $('#NumericRangeStart').val(startValue);
        $('#NumericRangeEnd').val(endValue);
    }
    else {
        $('#SetNumericRangeDialog').removeClass('loop');
    }
}
$p.openSetNumericRangeOK = function ($controlID) {
    $start = $('#NumericRangeStart');
    $end = $('#NumericRangeEnd');
    $start.validate();
    $end.validate();
    if (!$start.valid() || !$end.valid()) {
        $p.setErrorMessage('ValidationError');
        return false;
    }
    $control = $('[id="' + $controlID + '"]');
    $target = $('[id="' + $controlID.replace('_NumericRange', '') + '"]');
    var sdval = $('#NumericRangeStart').val();
    var edval = $('#NumericRangeEnd').val();
    var setval = '';
    var dispval = '';
    if (sdval || edval) {
        dispval = sdval + ' - ' + edval;
        setval = '["' + sdval + ',' + edval + '"]';
    }
    $control.val(dispval);
    $p.set($target, setval);
    $p.closeSiteSetNumericRangeDialog($controlID);
    if ($('#UseFilterButton').length === 0) {
        $p.send($target);
    }
}
$p.closeSiteSetNumericRangeDialog = function ($controlID) {
    $p.clearMessage();
    $('#SetNumericRangeDialog').addClass('loop');
    $('#SetNumericRangeDialog').dialog('close');
    if ($(document.activeElement).attr('id') != $controlID) {
        $('#SetNumericRangeDialog').removeClass('loop');
    }
}
$p.openSetNumericRangeClear = function ($control) {
    $('#NumericRangeStart').val('');
    $('#NumericRangeEnd').val('');
    $p.clearMessage();
}