$(function () {
    $('#PermissionForm').validate({
        rules: {
            Permissions_#ColumnName#: { #Validators# }
        },
        messages: {
            Permissions_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
