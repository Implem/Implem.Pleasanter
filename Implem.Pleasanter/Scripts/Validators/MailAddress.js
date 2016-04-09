$(function () {
    $('#MailAddressForm').validate({
        rules: {
            MailAddresses_#ColumnName#: { #Validators# }
        },
        messages: {
            MailAddresses_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
