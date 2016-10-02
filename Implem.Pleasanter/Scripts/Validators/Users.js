$p.validateUsers = function () {
    $('#UserForm').validate({
        ignore: '',
        rules: {
            Users_LoginId: { required:true },
            Users_Password: { required:true },
            Users_PasswordValidate: { required:true,equalTo:'#Users_Password' },
            Users_LastName: { required:true },
            Users_FirstName: { required:true }
        },
        messages: {
            Users_LoginId: { required:$p.display('ValidateRequired') },
            Users_Password: { required:$p.display('ValidateRequired') },
            Users_PasswordValidate: { required:$p.display('ValidateRequired'),equalTo:$p.display('ValidateEqualTo') },
            Users_LastName: { required:$p.display('ValidateRequired') },
            Users_FirstName: { required:$p.display('ValidateRequired') }
        }
    });
    $('#ChangePasswordForm').validate({
        ignore: '',
        rules: {
            Users_OldPassword: { required:true },
            Users_ChangedPassword: { required:true },
            Users_ChangedPasswordValidator: { required:true,equalTo:'#Users_ChangedPassword' }
        },
        messages: {
            Users_OldPassword: { required:$p.display('ValidateRequired') },
            Users_ChangedPassword: { required:$p.display('ValidateRequired') },
            Users_ChangedPasswordValidator: { required:$p.display('ValidateRequired'),equalTo:$p.display('ValidateEqualTo') }
        }
    });
    $('#ResetPasswordForm').validate({
        ignore: '',
        rules: {
            Users_AfterResetPassword: { required:true },
            Users_AfterResetPasswordValidator: { required:true,equalTo:'#Users_AfterResetPassword' }
        },
        messages: {
            Users_AfterResetPassword: { required:$p.display('ValidateRequired') },
            Users_AfterResetPasswordValidator: { required:$p.display('ValidateRequired'),equalTo:$p.display('ValidateEqualTo') }
        }
    });
    $('#DemoForm').validate({
        ignore: '',
        rules: {
            Users_DemoMailAddress: { required:true,email:true }
        },
        messages: {
            Users_DemoMailAddress: { required:$p.display('ValidateRequired'),email:$p.display('ValidateMail') }
        }
    });
}
$p.validateUsers();
