$p.openDeleteSiteDialog = function () {
    $('#DeleteSiteTitle').val('');
    $('#DeleteSitePassword').val('');
    $('#DeleteSiteDialog').dialog({
        modal: true,
        width: '420px',
        height: 'auto',
        appendTo: '.main-form'
    });
}