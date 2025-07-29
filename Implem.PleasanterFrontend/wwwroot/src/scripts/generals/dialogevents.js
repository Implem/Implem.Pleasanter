$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $p.clearMessage();
        $('.ui-dialog-content:visible:last').dialog('close');
    });
});
