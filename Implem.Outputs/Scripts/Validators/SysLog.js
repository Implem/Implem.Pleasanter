$(function () {
    $('#SysLogForm').validate({
        rules: {
            SysLogs_#ColumnName#: { #Validators# }
        },
        messages: {
            SysLogs_#ColumnName#: { #ValidatorMessages# }
        }
    });
});
