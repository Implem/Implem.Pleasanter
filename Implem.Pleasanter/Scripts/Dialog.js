$p.openDialog = function ($control) {
    $($control.attr('data-selector')).dialog({
        modal: true,
        width: '420px',
        appendTo: '.main-form'
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

$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $('.ui-dialog-content:visible').dialog('close');
    });
});