$p.openExportSelectorDialog = function ($control) {
    error = $p.send($control);
    if (error === 0) {
        $('#ExportSelectorDialog').dialog({
            modal: true,
            width: '420px',
            resizable: false
        });
    }
}

$p.export = function () {
    var data = $p.getData($('.main-form'));
    var exp = JSON.parse($('#ExportId').val());
    if (exp.mailNotify === true) {
        data["ExportId"] = exp.id;
        $p.send($("#DoExport"),"MainForm");
    } else {
        location.href = $('.main-form').attr('action').replace('_action_', 'export')
            + '?id=' + exp.id
            + '&GridCheckAll=' + data.GridCheckAll
            + '&GridUnCheckedItems=' + data.GridUnCheckedItems
            + '&GridCheckedItems=' + data.GridCheckedItems;
    }
    $p.closeDialog($('#ExportSelectorDialog'));
}

$p.exportCrosstab = function () {
    location.href = $('.main-form').attr('action').replace('_action_', 'exportcrosstab');
}