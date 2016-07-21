$(function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('data-id');
            var $element = getHoveredItem($('.nav-site:not([data-id="' + siteId + '"])'));
            if ($element && $element.attr('data-type') === 'Sites') {
                ui.item.hide();
                var data = getFormData($(this));
                data['SiteId'] = siteId;
                data['DestinationId'] = $element.attr('data-id');
                requestByForm(getForm($('#MoveSiteMenu')), $('#MoveSiteMenu'));
            }
        },
        update: function () {
            getFormData($(this))['Data'] = $('.nav-sites.sortable li')
                .map(function () { return $(this).attr('data-id'); })
                .get()
                .join(',');
            requestByForm(getForm($('#SortSiteMenu')), $('#SortSiteMenu'));
        }
    });
});