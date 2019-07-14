$p.openImportSettingsDialog = function ($control) {
    $('#ImportSettingsDialog').dialog({
        modal: true,
        width: '520px'
    });
}

$p.import = function ($control) {
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('Encoding', $('#Encoding').val());
    data.append('UpdatableImport', $('#UpdatableImport').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}