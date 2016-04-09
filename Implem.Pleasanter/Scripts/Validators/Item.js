$(function () {
    $('#ItemForm').validate({
        rules: {
            Items_#ColumnName#: { #Validators# }
        },
        messages: {
            Items_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
