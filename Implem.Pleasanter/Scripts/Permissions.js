$p.openPermissionsDialog = function ($control) {
    $p.data.PermissionsForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionsDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor'
        });
    }
}

$p.changePermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissions = mainFormData.CurrentPermissions;
    data.CurrentPermissionsAll = mainFormData.CurrentPermissionsAll;
    $p.send($control);
}

$p.setPermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    $p.setData($('#SourcePermissions'));
    $p.setData($('#SearchPermissionElements'));
    $p.send($control);
}