﻿$p.authenticatebymail = function () {
    $('#AuthenticationByMail').on('click', function () {
        $('#SecondaryAuthenticationCode').remove();
        $('#AuthenticationByMail').data('isauthenticationbymail', '1');
        $p.send($('#Login'));
        if ($('#TotpRegister').length) {
            $('#TotpRegister').remove();
        }
    });
}

$p.isAuthenticationByMail = function () {
    if ($('#AuthenticationByMail').length) {
        return '&isAuthenticationByMail=' + $('#AuthenticationByMail').data('isauthenticationbymail');
    } else {
        return '';
    }
}