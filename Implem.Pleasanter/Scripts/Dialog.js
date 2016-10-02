$p.openDialog = function ($control, appendTo) {
    $($control.attr('data-selector')).dialog({
        modal: true,
        width: '420px',
        appendTo: appendTo
    });
}

$p.sendByDialog = function ($control) {
    $p.closeDialog($control);
    $p.send($control);
}

$p.closeDialog = function ($control) {
    $control.closest('.ui-dialog-content').dialog('close');
}

$p.clearDialogs = function () {
    $('body > .ui-dialog').remove();
}