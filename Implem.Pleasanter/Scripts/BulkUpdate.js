$p.openBulkUpdateSelectorDialog = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#BulkUpdateSelectorDialog').dialog({
            modal: true,
            width: '520px',
            resizable: false
        });
    }
}

$p.bulkUpdate = function () {
    var maindata = $p.getData($('.main-form'));
    var data = $p.getData($('#BulkUpdateSelectorForm'));
    var key = $('#ReferenceType').val() + '_' + $('#BulkUpdateColumnName').val();
    data.GridCheckAll = maindata.GridCheckAll;
    data.GridUnCheckedItems = maindata.GridUnCheckedItems;
    data.GridCheckedItems = maindata.GridCheckedItems;
    $p.setData($('#' + key));
    $p.send($('#BulkUpdate'));
}