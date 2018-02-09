$p.setSiteMenu = function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('data-value');
            var $element = $p.hoverd($('.nav-site:not([data-value="' + siteId + '"])'));
            if ($element) {
                ui.item.hide();
                var data = $p.getData($('.main-form'));
                data.SiteId = siteId;
                data.DestinationId = $element.attr('data-value');
                $p.send($('#MoveSiteMenu'));
            }
        },
        update: function () {
            $p.getData($(this)).Data = $p.toJson($('.nav-sites.sortable li'));
            $p.send($('#SortSiteMenu'));
        }
    });
}

$p.openLinkDialog = function () {
    $('#LinkDialog').dialog({
        modal: true,
        width: '420px',
        appendTo: '.main-form'
    });
}