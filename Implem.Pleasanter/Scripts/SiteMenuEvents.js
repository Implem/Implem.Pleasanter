$(function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('data-id');
            var $element = $p.hoverd($('.nav-site:not([data-id="' + siteId + '"])'));
            if ($element && $element.attr('data-type') === 'Sites') {
                ui.item.hide();
                var data = $p.getData($('.main-form'));
                data.SiteId = siteId;
                data.DestinationId = $element.attr('data-id');
                $p.send($('#MoveSiteMenu'));
            }
        },
        update: function () {
            $p.getData($('.main-form')).Data = $('.nav-sites.sortable li')
                .map(function () { return $(this).attr('data-id'); })
                .get()
                .join(',');
            $p.send($('#SortSiteMenu'));
        }
    });
});