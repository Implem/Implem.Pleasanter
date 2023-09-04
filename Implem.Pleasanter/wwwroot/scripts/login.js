$p.showPassword = function () {
    if ($('#Users_Password').attr('type') === 'password') {
        $('#Users_Password').attr('type', 'text');
        $('#show_password').removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        $('#Users_Password').attr('type', 'password');
        $('#show_password').removeClass('fa-eye-slash').addClass('fa-eye');
    }
}