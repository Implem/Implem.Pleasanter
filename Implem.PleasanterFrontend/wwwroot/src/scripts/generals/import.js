$p.openImportSettingsDialog = function ($control) {
    $('#ImportSettingsDialog').dialog({
        modal: true,
        width: '520px',
        resizable: false
    });
};

$p.import = function ($control) {
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('Encoding', $('#Encoding').val());
    data.append('UpdatableImport', $('#UpdatableImport').prop('checked'));
    data.append('Key', $('#Key').val());
    data.append('ReplaceAllGroupMembers', $('#ReplaceAllGroupMembers').prop('checked'));
    data.append('MigrationMode', $('#MigrationMode').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control
    );
};

$(function () {
    $(document).on('change', '#UpdatableImport', function () {
        $('#KeyField').toggle($(this).prop('checked'));
    });
});
