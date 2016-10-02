$p.validateOutgoingMails = function () {
    $('#OutgoingMailForm').validate({
        ignore: '',
        rules: {
            OutgoingMails_Title: { required:true },
            OutgoingMails_Body: { required:true }
        },
        messages: {
            OutgoingMails_Title: { required:$p.display('ValidateRequired') },
            OutgoingMails_Body: { required:$p.display('ValidateRequired') }
        }
    });
}
$p.validateOutgoingMails();
