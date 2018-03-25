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
    location.href = $('.main-form').attr('action').replace('_action_', 'export') + '?id=' +
        $('#ExportId').val();
    $p.closeDialog($('#ExportSelectorDialog'));
}

$p.exportCrosstab = function () {
    location.href = $('.main-form').attr('action').replace('_action_', 'exportcrosstab');
}