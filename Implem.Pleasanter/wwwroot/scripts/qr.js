$p.showQr = function () {
    if ($('#TotpQRCode').length && !$('#qrCode').children().length) {
        var uri = $('#TotpQRCode').data('url');
        new QRCode($('#qrCode').get(0) ,
            {
                text: uri,
                width: 150,
                height: 150
            });
    };
};