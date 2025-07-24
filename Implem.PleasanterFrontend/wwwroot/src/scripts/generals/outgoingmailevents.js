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

    function showMailEditor() {
        $('#MailEditorTabsContainer').tabs('option', 'active', 0);
    }
});