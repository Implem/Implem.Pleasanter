$p.validateOutgoingMails = function () {
    $('#OutgoingMailForm').validate({
        ignore: '',
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
$p.validateOutgoingMails();
