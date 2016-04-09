$(function () {
    $('#BinaryForm').validate({
        rules: {
            Binaries_#ColumnName#: { #Validators# }
        },
        messages: {
            Binaries_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
