$p.openOutgoingMailDialog = function ($control) {
    var error = 0;
    if ($('#OutgoingMails_Title').length === 0) {
        var data = $p.getData($('#OutgoingMailsForm'));
        data.Controller = $('#Controller').val();
        data.Id = $('#Id').val();
        error = $p.syncSend($control, 'OutgoingMailsForm');
    }
    if (error === 0) {
        $('#OutgoingMailDialog').dialog({
            modal: true,
            width: '90%',
            height: 'auto',
            dialogClass: 'outgoing-mail'
        });
    }
}

$p.openOutgoingMailReplyDialog = function ($control) {
    $p.getData($('#OutgoingMailsForm')).OutgoingMails_OutgoingMailId = $control.attr('data-id');
    var error = $p.syncSend($control, 'OutgoingMailsForm');
    if (error === 0) {
        $('#OutgoingMailDialog').dialog({
            modal: true,
            width: '90%',
            height: 'auto',
            dialogClass: 'outgoing-mail'
        });
    }
}

$p.sendMail = function ($control) {
    var data = $p.getData($('#OutgoingMailForm'));
    data.Ver = $('._Ver')[0].innerHTML;
    data.Controller = $('#Controller').val();
    data.Id = $('#Id').val();
    $p.send($control);
}

$p.initOutgoingMailDialog = function () {
    var body = $('#' + $p.tableName() + '_Body').val();
    body = body !== undefined ? body + '\n\n' : '';
    if ($('#OutgoingMails_Reply').val() !== '1') {
        $('#OutgoingMails_Title').val($('#HeaderTitle').text());
        $('#OutgoingMails_Body').val(body + $('#OutgoingMails_Location').val());
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