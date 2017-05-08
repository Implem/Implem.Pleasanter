$p.openTemplateDialog = function ($control) {
    var error = $p.syncSend($control, 'SitesForm');
    if (error === 0) {
        $('#TemplateDialog').dialog({
            modal: true,
            width: '520px',
            appendTo: '.main-form'
        });
    }
}