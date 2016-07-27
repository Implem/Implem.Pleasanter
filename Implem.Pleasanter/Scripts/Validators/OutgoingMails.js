$p.form.validators.outgoingMails = function () {
    $('#OutgoingMailForm').validate({
        rules: {
            OutgoingMails_Title: { required:true },
            OutgoingMails_Body: { required:true }
        },
        messages: {
            OutgoingMails_Title: { required: $('#OutgoingMails_Title').attr('data-validate-required') },
            OutgoingMails_Body: { required: $('#OutgoingMails_Body').attr('data-validate-required') }
        }
    });
}
$p.form.validators.outgoingMails();
