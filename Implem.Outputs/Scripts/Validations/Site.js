$(function () {
    $('#SiteForm').validate({
        rules: {
            Sites_Title: { required:true }
        },
        messages: {
            Sites_Title: { required: $('#Sites_Title').attr('data-validate-required') }
        }
    });
});
