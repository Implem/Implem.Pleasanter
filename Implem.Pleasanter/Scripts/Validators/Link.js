$(function () {
    $('#LinkForm').validate({
        rules: {
            Links_#ColumnName#: { #Validators# }
        },
        messages: {
            Links_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
