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
        $('#MailEditorTabsContainer').tabs('option', 'active', 0);
    }
});