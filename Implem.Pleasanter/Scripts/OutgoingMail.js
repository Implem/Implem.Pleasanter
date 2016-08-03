$p.openOutgoingMailDialog = function ($control) {
    if ($('#OutgoingMails_Title').length === 0) {
        $p.ajax(
            $control.attr('data-action'),
            $control.attr('data-method'),
            $p.getData('OutgoingMailForm'),
            false);
    }
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: '650',
        dialogClass: 'outgoing-mail'
    });
    $('#OutgoingMailDialog').css('height', 'auto');
}

$p.openOutgoingMailReplyDialog = function ($control) {
    $p.getData().OutgoingMails_OutgoingMailId = $control.attr('data-id');
    $p.send($control, 'OutgoingMailsForm');
    $('#OutgoingMailDialog').dialog({
        modal: true,
        width: '90%',
        height: '650',
        dialogClass: 'outgoing-mail'
    });
    $('#OutgoingMailDialog').css('height', 'auto');
}

$p.sendMail = function ($control) {
    $p.getData('OutgoingMailForm').Ver = $('._Ver')[0].innerHTML;
    $p.send($control, 'OutgoingMailForm');
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
    $p.getData('OutgoingMailForm')[$control.attr('id')] =
        $control.find('li').map(function () {
            return unescape($(this).text());
        }).get().join(';');
}

$(function () {
    $(document).on('click', '#OutgoingMails_AddTo', function () {
        $p.addMailAddress($('#OutgoingMails_To'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddCc', function () {
        $p.addMailAddress($('#OutgoingMails_Cc'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddBcc', function () {
        $p.addMailAddress($('#OutgoingMails_Bcc'));
        showMailEditor();
    });
    $(document).on('click', '.button-delete-address', function () {
        var $control = $(this).closest('ol');
        $(this).parent().remove();
        $p.setData($control);
    });

    function showMailEditor() {
        $('#OutgoingMailDialog').find('.edit-form-tabs-max').tabs('option', 'active', 0);
    }
});