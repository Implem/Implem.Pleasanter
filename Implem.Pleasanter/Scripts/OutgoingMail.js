$p.openOutgoingMailDialog = function ($control) {
    if ($('#OutgoingMails_Title').length === 0) {
        $p.send($control, 'OutgoingMailsForm', false);
    }
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: 'auto',
        dialogClass: 'outgoing-mail'
    });
}

$p.openOutgoingMailReplyDialog = function ($control) {
    $p.getData('OutgoingMailsForm').OutgoingMails_OutgoingMailId = $control.attr('data-id');
    $p.send($control, 'OutgoingMailsForm', false);
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: 'auto',
        dialogClass: 'outgoing-mail'
    });
}

$p.sendMail = function ($control) {
    $p.getData('OutgoingMailForm').Ver = $('._Ver')[0].innerHTML;
    $p.send($control);
}

$p.initOutgoingMailDialog = function () {
    if ($('#OutgoingMails_Reply').val() !== '1') {
        $('#OutgoingMails_Title').val($('#HeaderTitle').text());
        $('#OutgoingMails_Body').val($('#' + $p.tableName() + '_Body').val() +
            '\n\n' + $('#OutgoingMails_Location').val());
    }
    $p.addMailAddress($('#OutgoingMails_To'), $('#MailToDefault').val());
    $p.addMailAddress($('#OutgoingMails_Cc'), $('#MailCcDefault').val());
    $p.addMailAddress($('#OutgoingMails_Bcc'), $('#MailBccDefault').val());
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
                $control.append(
                    $('<li/>')
                        .addClass('ui-widget-content ui-selectee')
                        .append($('<span/>').text(mailAddress))
                        .append($('<span/>')
                            .addClass('ui-icon ui-icon-close button-delete-address')));
            }
        });
    }
    $p.setMailAddressData($control);
}

$p.setMailAddressData = function ($control) {
    $p.getData('OutgoingMailForm')[$control.attr('id')] =
        $control.find('li').map(function () {
            return unescape($(this).text());
        }).get().join(';');
}