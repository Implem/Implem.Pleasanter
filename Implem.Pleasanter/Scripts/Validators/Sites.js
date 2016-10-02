$p.validateSites = function () {
    $('#SiteForm').validate({
        ignore: '',
        rules: {
            Sites_Title: { required:true }
        },
        messages: {
            Sites_Title: { required:$p.display('ValidateRequired') }
        }
    });
    $('#NotificationForm').validate({
        rules: {
            NotificationAddress: { required: true }
        },
        messages: {
            NotificationAddress: { required: $p.display('ValidateRequired') }
        }
    });

}
$p.validateSites();
