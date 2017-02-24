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
    $p.getData($control).PermissionDestination = $p.getData($('.main-form')).PermissionDestination;
    $p.send($control);
}