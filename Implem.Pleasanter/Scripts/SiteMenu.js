$p.setSiteMenu = function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('value');
            var $element = $p.hoverd($('.nav-site:not([value="' + siteId + '"])'));
            if ($element && $element.attr('data-type') === 'Sites') {
                ui.item.hide();
                var data = $p.getData($('.main-form'));
                data.SiteId = siteId;
                data.DestinationId = $element.attr('value');
                $p.send($('#MoveSiteMenu'));
            }
        },
        update: function () {
            $p.getData($(this)).Data = $p.toJson($('.nav-sites.sortable li'));
            $p.send($('#SortSiteMenu'));
        }
    });
}