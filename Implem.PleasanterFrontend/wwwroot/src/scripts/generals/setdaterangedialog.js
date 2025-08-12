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
};
$p.openSiteSetDateRangeDialog = function ($control, timepicker) {
    $control.blur();
    if (!$('#SetDateRangeDialog').hasClass('loop')) {
        $p.openSiteSettingsDialog($control, '#SetDateRangeDialog', 'auto');
        var $target = $('[id="' + $control.attr('id').replace('_DateRange', '') + '"]');
        var initValue = JSON.parse($target.val() || 'null');
        var startValue = '';
        var endValue = '';
        if (Array.isArray(initValue) && initValue.length > 0 && !/^[A-Za-z]*$/.test(initValue)) {
            var values = initValue[0].split(',');
            if (values.length > 0) {
                startValue = timepicker ? values[0] : values[0].split(' ')[0];
            }
            if (values.length > 1) {
                endValue = timepicker ? values[1] : values[1].split(' ')[0];
            }
        }
        $('#DateRangeStart').val(startValue);
        $('#DateRangeEnd').val(endValue);
    } else {
        $('#SetDateRangeDialog').removeClass('loop');
    }
};
$p.openSetDateRangeOK = function (controlId, type) {
    var sdval = $('#DateRangeStart').val();
    var edval = $('#DateRangeEnd').val();
    var setval = '';
    var dispval = '';
    switch (type) {
        case 'Today':
        case 'ThisMonth':
        case 'ThisYear':
            setval = '["' + type + '"]';
            dispval = $p.display(type);
            break;
        default:
            if (sdval || edval) {
                dispval = sdval + ' - ' + edval;
                if (type !== 'DateTimepicker' && sdval) {
                    sdval += ' 00:00:00.000';
                }
                if (type !== 'DateTimepicker' && edval) {
                    edval += ' 23:59:59.997';
                }
                setval = '["' + sdval + ',' + edval + '"]';
            }
            break;
    }
    var $control = $('[id="' + controlId + '"]');
    var $target = $('[id="' + controlId.replace('_DateRange', '') + '"]');
    $control.val(dispval);
    $p.set($target, setval);
    $p.closeSiteSetDateRangeDialog(controlId);
    if ($('#UseFilterButton').length === 0) {
        $p.send($target);
    }
};
$p.closeSiteSetDateRangeDialog = function (controlId) {
    $p.clearMessage();
    $('#SetDateRangeDialog').addClass('loop');
    $('#SetDateRangeDialog').dialog('close');
    if ($(document.activeElement).attr('id') != controlId) {
        $('#SetDateRangeDialog').removeClass('loop');
    }
};
$p.openSetDateRangeClear = function () {
    $('#DateRangeStart').val('');
    $('#DateRangeEnd').val('');
};
