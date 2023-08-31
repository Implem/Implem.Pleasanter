$p.openBulkUpdateSelectorDialog = function ($control) {
    $p.data.BulkUpdateSelectorForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#BulkUpdateSelectorDialog').dialog({
            modal: true,
            width: '800px',
            resizable: false
        });
    }
}

$p.bulkUpdate = function () {
    var main_data = $p.getData($('.main-form'));
    var data = $p.getData($('#BulkUpdateSelectorForm'));
    var key = $('#ReferenceType').val() + '_' + $('#BulkUpdateColumnName').val();
    for (let key in main_data) {
        data[key] = main_data[key];
    }
    $p.setData($('#' + key));
    $p.send($('#BulkUpdate'));
}