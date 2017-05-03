$p.openImportSettingsDialog = function ($control) {
    $p.syncSend($control);
    $('#ImportSettingsDialog').dialog({
        modal: true,
        width: '520px'
    });
}

$p.import = function ($control) {
    var data = new FormData();
    data.append('Import', $('#Import').prop('files')[0]);
    data.append('Encoding', $('#Encoding').val());
    data.append('UpdatableImport', $('#UpdatableImport').prop('checked'));
    $p.upload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        $control.attr('data-method'),
        data,
        $control);
}