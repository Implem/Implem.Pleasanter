$p.setPermissionEvents = function () {
    $p.setPaging('SourcePermissions');
}

$p.setPermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    $p.setData($('#SourcePermissions'));
    $p.setData($('#SearchPermissionElements'));
    $p.send($control);
}

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

$p.setPermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    $p.setData($('#SourcePermissionForCreating'));
    $p.send($control);
}

$p.openPermissionForCreatingDialog = function ($control) {
    $p.data.PermissionForCreatingForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionForCreatingDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor'
        });
    }
}

$p.changePermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissionForCreating = mainFormData.CurrentPermissionForCreating;
    data.CurrentPermissionForCreatingAll = mainFormData.CurrentPermissionForCreatingAll;
    $p.send($control);
}

$p.openColumnAccessControlDialog = function ($control) {
    $p.data.ColumnAccessControlForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#ColumnAccessControlDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor'
        });
    }
}

$p.changeColumnAccessControl = function ($control, type) {
    $p.setData($('#' + type + 'ColumnAccessControl'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.ColumnAccessControl = mainFormData[type + 'ColumnAccessControl'];
    data.ColumnAccessControlAll = mainFormData[type + 'ColumnAccessControlAll'];
    data.ColumnAccessControlType = type;
    $p.send($control);
}