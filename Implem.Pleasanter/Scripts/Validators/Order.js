$(function () {
    $('#OrderForm').validate({
        rules: {
            Orders_#ColumnName#: { #Validators# }
        },
        messages: {
            Orders_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
