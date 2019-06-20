$p.openChangePasswordDialog = function () {
    var data = $p.getData($('#ChangePasswordForm'));
    data.Users_LoginId = $('#Users_LoginId').val();
    data.Users_OldPassword = $('#Users_Password').val();
    data.ReturnUrl = $('#ReturnUrl').val();
    $('#ChangePasswordDialog').dialog({
        modal: true,
        width: '420px'
    });
}

$p.changePasswordAtLogin = function ($control) {
    var data = $p.getData($('#ChangePasswordForm'));
    data.Users_LoginId = $('#Users_LoginId').val();
    data.Users_Password = $('#Users_Password').val();
    $p.send($control);
}