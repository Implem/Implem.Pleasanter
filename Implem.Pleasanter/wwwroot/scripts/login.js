$p.showPassword = function () {
    if ($('#Users_Password').attr('type') === 'password') {
        $('#Users_Password').attr('type', 'text');
        $('#show_password').text('visibility_off');
    } else {
        $('#Users_Password').attr('type', 'password');
        $('#show_password').text('visibility');
    }
}