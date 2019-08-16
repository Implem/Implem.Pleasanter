$p.setDateRangeDialog = function ($control, title, startLabel, endLabel, okLabel, cancelLabel, clearLabel, timepicker) {
    $control.blur();
    $target = $('#' + $control.attr('id').replace("_Display_", ""));

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
    $dlg = $('<div>')
        .attr({
            id: 'SetDateRangeDialog',
            class: 'dialog ui-dialog-content ui-widget-content',
            style: 'display:block;'
        });

    $startDate = $('<input>')
        .attr({
            id: 'dateRangeStart',
            class: 'control-textbox datepicker applied valid',
            type: 'text',
            placeholder: startLabel,
            autocomplete: 'off',
            value: startValue
        })
        .datetimepicker({
            timepicker: timepicker,
            format: timepicker ? 'Y/m/d H:i' : 'Y/m/d'
        });

    $endDate = $('<input>')
        .attr({
            id: 'dateRangeEnd',
            class: 'control-textbox datepicker applied valid',
            type: 'text',
            placeholder: endLabel,
            autocomplete: 'off',
            value: endValue
        })
        .datetimepicker({
            timepicker: timepicker,
            format: timepicker ? 'Y/m/d H:i' : 'Y/m/d'
        });

    $okButton = $('<button>')
        .attr({
            class: 'button button-icon validate ui-button ui-corner-all ui-widget applied',
            type: 'button'
        })
        .on('click', function () {
            var sdval = $startDate.val();
            var edval = $endDate.val();
            var setval = "";
            var dispval = "";
            if (sdval || edval) {
                dispval = sdval + "-" + edval;
                if (!timepicker && sdval) { sdval += " 00:00:00.000"; }
                if (!timepicker && edval) { edval += " 23:59:59.997"; }
                setval = '["'+ sdval + ',' + edval + '"]';
            }
            $control.val(dispval);
            $p.set($target, setval);
            $dlg.dialog("close");
            $p.send($target);
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
        .on('click', function () { $startDate.val(""); $endDate.val(""); })
        .append($('<span class="ui-button-icon ui-icon ui-icon-arrowrefresh-1-e">'))
        .append($('<span class="ui-button-icon-space">'))
        .append(clearLabel);

    $dlg
        .append($('<input>').css({ opacity: 0, position: 'absolute', top: 0, left: 0 }))
        .append($('<fieldset>')
            .addClass("fieldset cf")
            .append($('<div>')
                .addClass("field-normal")
                .append($('<label>')
                    .attr({ class: "field-label", for: "dateRangeStart" })
                    .text(startLabel))
                .append($('<div>')
                    .addClass("field-control")
                    .append($('<div>')
                        .addClass("container-normal")
                        .append($startDate))))
            .append($('<div>')
                .addClass("field-normal")
                .append($('<label>')
                    .attr({ class: "field-label", for: "dateRangeEnd" })
                    .text(endLabel))
                .append($('<div>')
                    .addClass("field-control")
                    .append($('<div>')
                        .addClass("container-normal")
                        .append($endDate))))
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
};