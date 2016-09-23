$p.openDialog = function ($control) {
    $($control.attr('data-selector')).dialog({
        modal: true,
        width: '420px'
    });
}

$p.sendByDialog = function ($control) {
    $p.closeDialog($control);
    $p.send($control, $p.getFormId($control));
}

$p.closeDialog = function ($control) {
    $control.closest('.ui-dialog-content').dialog('close');
}

$p.clearDialogs = function () {
    $('body > .ui-dialog').remove();
}