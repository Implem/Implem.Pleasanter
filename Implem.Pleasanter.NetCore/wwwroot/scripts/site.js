$p.openDeleteSiteDialog = function () {
    $('#DeleteSiteDialog input').val('');
    $('#DeleteSiteDialog').dialog({
        modal: true,
        width: '420px',
        height: 'auto',
        appendTo: '.main-form'
    });
}