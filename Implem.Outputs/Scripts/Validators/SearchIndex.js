$(function () {
    $('#SearchIndexForm').validate({
        rules: {
            SearchIndexes_#ColumnName#: { #Validators# }
        },
        messages: {
            SearchIndexes_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
