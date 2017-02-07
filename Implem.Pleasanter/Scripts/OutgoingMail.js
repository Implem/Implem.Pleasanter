$p.openOutgoingMailDialog = function ($control) {
    if ($('#OutgoingMails_Title').length === 0) {
        $p.syncSend($control, 'OutgoingMailsForm');
    }
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: 'auto',
        dialogClass: 'outgoing-mail'
    });
}

$p.openOutgoingMailReplyDialog = function ($control) {
    $p.getData($('#OutgoingMailsForm')).OutgoingMails_OutgoingMailId = $control.attr('data-id');
    $p.syncSend($control, 'OutgoingMailsForm');
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: 'auto',
        dialogClass: 'outgoing-mail'
    });
}

$p.sendMail = function ($control) {
    $p.getData($('#OutgoingMailForm')).Ver = $('._Ver')[0].innerHTML;
    $p.send($control);
}

$p.initOutgoingMailDialog = function () {
    if ($('#OutgoingMails_Reply').val() !== '1') {
        $('#OutgoingMails_Title').val($('#HeaderTitle').text());
        $('#OutgoingMails_Body').val($('#' + $p.tableName() + '_Body').val() +
            '\n\n' + $('#OutgoingMails_Location').val());
    }
    $p.addMailAddress($('#OutgoingMails_To'), $('#To').val());
    $p.addMailAddress($('#OutgoingMails_Cc'), $('#Cc').val());
    $p.addMailAddress($('#OutgoingMails_Bcc'), $('#Bcc').val());
}

$p.addMailAddress = function ($control, defaultMailAddresses) {
    var mailAddresses = defaultMailAddresses !== undefined
        ? defaultMailAddresses
        : $('#OutgoingMails_MailAddresses').find('.ui-selected').map(function () {
            return unescape($(this).text());
        }).get().join(';');
    if (mailAddresses) {
        mailAddresses.split(';').forEach(function (mailAddress) {
            if (mailAddress) {
                $p.addBasket($control, mailAddress);
            }
        });
    }
    $p.setData($control);
}