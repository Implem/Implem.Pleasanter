$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $p.clearMessage();
        $('.ui-dialog-content:visible:last').dialog('close');
    });
    $(document).on('click', '#OpenBulkUpdateSelectorDialogCommand', function () {
        $('div.ui-multiselect-menu').css('z-index', 110); // JQueryUIのui-multiselect-menuのz-indexが固定値の為に書き換える。
    })
});