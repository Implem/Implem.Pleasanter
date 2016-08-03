$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $('.ui-dialog-content:visible').dialog('close');
    });
});