$p.validateSites = function () {
    $('#SiteForm').validate({
        ignore: '',
        rules: {
            Sites_Title: { required:true }
        },
        messages: {
            Sites_Title: { required: $('#Sites_Title').attr('data-validate-required') }
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
