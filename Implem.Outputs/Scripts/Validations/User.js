$(function () {
    $('#UserForm').validate({
        rules: {
            Users_LoginId: { required:true },
            Users_Password: { required:true },
            Users_PasswordValidate: { required:true,equalTo:'#Users_Password' },
            Users_LastName: { required:true },
            Users_FirstName: { required:true }
        },
        messages: {
            Users_LoginId: { required: $('#Users_LoginId').attr('data-validate-required') },
            Users_Password: { required: $('#Users_Password').attr('data-validate-required') },
            Users_PasswordValidate: { required: $('#Users_PasswordValidate').attr('data-validate-required'),equalTo: $('#Users_PasswordValidate').attr('data-validate-equalTo') },
            Users_LastName: { required: $('#Users_LastName').attr('data-validate-required') },
            Users_FirstName: { required: $('#Users_FirstName').attr('data-validate-required') }
        }
    });
    $('#ChangePasswordForm').validate({
        rules: {
            Users_OldPassword: { required:true },
            Users_ChangedPassword: { required:true },
            Users_ChangedPasswordValidation: { required:true,equalTo:'#Users_ChangedPassword' }
        },
        messages: {
            Users_OldPassword: { required: $('#Users_OldPassword').attr('data-validate-required') },
            Users_ChangedPassword: { required: $('#Users_ChangedPassword').attr('data-validate-required') },
            Users_ChangedPasswordValidation: { required: $('#Users_ChangedPasswordValidation').attr('data-validate-required'),equalTo: $('#Users_ChangedPasswordValidation').attr('data-validate-equalTo') }
        }
    });
    $('#ResetPasswordForm').validate({
        rules: {
            Users_AfterResetPassword: { required:true },
            Users_AfterResetPasswordValidation: { required:true,equalTo:'#Users_AfterResetPassword' }
        },
        messages: {
            Users_AfterResetPassword: { required: $('#Users_AfterResetPassword').attr('data-validate-required') },
            Users_AfterResetPasswordValidation: { required: $('#Users_AfterResetPasswordValidation').attr('data-validate-required'),equalTo: $('#Users_AfterResetPasswordValidation').attr('data-validate-equalTo') }
        }
    });
});
