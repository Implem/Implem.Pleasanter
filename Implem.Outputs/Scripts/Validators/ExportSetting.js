$(function () {
    $('#ExportSettingForm').validate({
        rules: {
            ExportSettings_#ColumnName#: { #Validators# }
        },
        messages: {
            ExportSettings_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
