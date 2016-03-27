$(function () {
    func.initDialog_OutgoingMail = function () {
        if ($('#OutgoingMails_Reply').val() !== '1') {
            $('#OutgoingMails_Title').val($('#HeaderTitle').text());
            $('#OutgoingMails_Body').val($('#' + getTableName() + '_Body').val() +
                '\n\n' + $('#OutgoingMails_Location').val());
        }
        addMailAddress($('#OutgoingMails_To'), $('#MailToDefault').val());
        addMailAddress($('#OutgoingMails_Cc'), $('#MailCcDefault').val());
        addMailAddress($('#OutgoingMails_Bcc'), $('#MailBccDefault').val());
    }
    $(document).on('click', '#OutgoingMails_AddTo', function () {
        addMailAddress($('#OutgoingMails_To'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddCc', function () {
        addMailAddress($('#OutgoingMails_Cc'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddBcc', function () {
        addMailAddress($('#OutgoingMails_Bcc'));
        showMailEditor();
    });
    $(document).on('click', '.button-delete-address', function () {
        var $control = $(this).closest('ol');
        $(this).parent().remove();
        setFormData($control);
    });

    function addMailAddress($control, defaultMailAddresses) {
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
        setFormData($control);
    }

    function showMailEditor() {
        $('#Dialog_OutgoingMail').find('.edit-form-tabs-max').tabs('option', 'active', 0);
    }

    function setFormData($control) {
        getFormData($control)[$control.attr('id')] = $control.find('li').map(function () {
            return unescape($(this).text());
        }).get().join(';');
    }
});