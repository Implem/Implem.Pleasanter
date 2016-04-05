$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $(this).prev().find('.ui-dialog-content').dialog('close');
    });
    func.clearDialogs = function () {
        $('body > .ui-dialog').remove();
    }
});