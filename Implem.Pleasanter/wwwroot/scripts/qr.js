$p.showQr = function () {
    if ($('#GoogleAuthenticatorQRCode').length) {
        var uri = $('#GoogleAuthenticatorQRCode').data('url');
        new QRCode($('#qrCode').get(0) ,
            {
                text: uri,
                width: 150,
                height: 150
            });
    };
};