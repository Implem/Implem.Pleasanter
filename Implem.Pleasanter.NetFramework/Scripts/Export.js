$p.openExportSelectorDialog = function ($control) {
    error = $p.send($control);
    if (error === 0) {
        $('#ExportSelectorDialog').dialog({
            modal: true,
            width: '420px'
        });
    }
}

$p.export = function () {
    var data = $p.getData($('.main-form'));
    location.href = $('.main-form').attr('action').replace('_action_', 'export')
        + '?id=' + $('#ExportId').val()
        + '&GridCheckAll=' + data.GridCheckAll
        + '&GridUnCheckedItems=' + data.GridUnCheckedItems
        + '&GridCheckedItems=' + data.GridCheckedItems;
    $p.closeDialog($('#ExportSelectorDialog'));
}

$p.exportCrosstab = function () {
    location.href = $('.main-form').attr('action').replace('_action_', 'exportcrosstab');
}