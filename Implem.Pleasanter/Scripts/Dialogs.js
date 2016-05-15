$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $('.ui-dialog-content:visible').dialog('close');
    });
    func.clearDialogs = function () {
        $('body > .ui-dialog').remove();
    }
});