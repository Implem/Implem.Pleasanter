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