$p.openSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    error = $p.send($control);
    if (error === 0) {
        $('#SetNumericRangeDialog').dialog({
            modal: true,
            width: '420px'
        });
    }
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
    $target = $('#' + $controlID.replace("_Display_", ""));
    var sdval = $("#numericRangeStart").val();
    var edval = $("#numericRangeEnd").val();
    var setval = "";
    var dispval = "";
    if (sdval || edval) {
        dispval = sdval + "-" + edval;
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
$p.setNumericRangeDialog = function ($control, title, startLabel, endLabel, okLabel, cancelLabel, clearLabel, minNumber, maxNumber) {
    $control.blur();
    $target = $('#' + $control.attr('id').replace("_Display_", ""));
    var initValue = JSON.parse($target.val() || "null");
    var startValue = "";
    var endValue = "";
    if (Array.isArray(initValue) && initValue.length > 0) {
        var values = initValue[0].split(',');
        if (values.length > 0) {
            startValue = values[0].split(' ')[0];
        }
        if (values.length > 1) {
            endValue = values[1].split(' ')[0];
        }
    }
    $dlg = $('<div>')
        .attr({
            id: 'SetNumericRangeDialog',
            class: 'dialog ui-dialog-content ui-widget-content',
            style: 'display:block;'
        })
        .append($('<form>')
            .attr({
                id: 'numericRangeForm',
                class: 'NumericRangeDialogForm',
                novalidate: 'novalidate',
            }));
    $startNumeric = $('<input>')
        .attr({
            id: 'numericRangeStart',
            class: 'control-textbox with-unit',
            type: 'text',
            placeholder: startLabel,
            value: startValue,
            "data-validate-required": "0",
            "data-validate-number": "1",
            "data-validate-min-number": minNumber,
            "data-validate-max-number": maxNumber
        });
    $endNumeric = $('<input>')
        .attr({
            id: 'numericRangeEnd',
            class: 'control-textbox applied valid',
            type: 'text',
            placeholder: endLabel,
            autocomplete: 'off',
            value: endValue,
            "data-validate-required": "0",
            "data-validate-number": "1",
            "data-validate-min-number": minNumber,
            "data-validate-max-number": maxNumber
        });
    $okButton = $('<button>')
        .attr({
            class: 'button button-icon validate ui-button ui-corner-all ui-widget applied',
            type: 'button'
        })
        .on('click', function () {

            $start = $('#numericRangeStart');
            $end = $('#numericRangeEnd');
            $start.validate();
            $end.validate();
            if (!$start.valid() || !$end.valid()) {
                $p.setErrorMessage('ValidationError', '#NumericRangeDialogFormMessage');
                return false;
            }
            var sdval = $startNumeric.val();
            var edval = $endNumeric.val();
            var setval = "";
            var dispval = "";
            if (sdval || edval) {
                dispval = sdval + "-" + edval;
                setval = '["'+ sdval + ',' + edval + '"]';
            }
            $control.val(dispval);
            $p.set($target, setval);
            $dlg.dialog("close");
        })
        .append($('<span>')
            .addClass("ui-button-icon ui-icon ui-icon-check"))
        .append($('<span>')
            .addClass("ui-button-icon-space"))
        .append(okLabel);
    $cancelButton = $('<button>')
        .attr({
            class: 'button button-icon validate ui-button ui-corner-all ui-widget applied',
            type: 'button'
        })
        .on('click', function () { $dlg.dialog("close"); })
        .append($('<span>')
            .addClass("ui-button-icon ui-icon ui-icon-cancel"))
        .append($('<span>')
            .addClass("ui-button-icon-space"))
        .append(cancelLabel);
    $clearButton = $('<button>')
        .attr({
            class: 'button button-icon validate ui-button ui-corner-all ui-widget applied',
            type: 'button'
        })
        .on('click', function () { $startNumeric.val(""); $endNumeric.val(""); $p.clearMessage();})
        .append($('<span class="ui-button-icon ui-icon ui-icon-arrowrefresh-1-e">'))
        .append($('<span class="ui-button-icon-space">'))
        .append(clearLabel);
    $dlg
        .children('.NumericRangeDialogForm')
        .append($('<input/>').css({ opacity: 0, position: 'absolute', top: 0, left: 0 }))
        .append($('<fieldset>')
            .addClass("fieldset cf")
            .append($('<div>')
                .addClass("field-normal")
                .append($('<label>')
                    .attr({ class: "field-label", for: "numericRangeStart" })
                    .text(startLabel))
                .append($('<div>')
                    .addClass("field-control")
                    .append($('<div>')
                        .addClass("container-normal")
                        .append($startNumeric))))
            .append($('<div>')
                .addClass("field-normal")
                .append($('<label>')
                    .attr({ class: "field-label", for: "numericRangeEnd" })
                    .text(endLabel))
                .append($('<div>')
                    .addClass("field-control")
                    .append($('<div>')
                        .addClass("container-normal")
                        .append($endNumeric))))
            .append($('<p>')
                .attr({
                    id: 'NumericRangeDialogFormMessage',
                    class: 'message-dialog'
                }))
            .append($('<div>')
                .addClass("command-center")
                .append($okButton)
                .append($cancelButton)
                .append($clearButton)));
    $dlg.dialog({
        autoOpen: false,
        modal: true,
        title: title,
        height: 'auto',
        width: 'auto',
        position: { my: 'center top', at: 'center bottom', of: $control }
    });
    $dlg.dialog("open");
    $p.applyValidator();
};