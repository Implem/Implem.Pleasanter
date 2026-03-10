$p.setSiteMenu = function (sortable = true) {
    var sortableBool = sortable === true || sortable === 'true' ? true : false;
    if (sortableBool) {
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
                if (!sortable) {
                    location.reload();
                    return;
                }
                $p.getData($(this)).Data = $p.toJson($('.nav-sites.sortable li'));
                $p.send($('#SortSiteMenu'));
            }
        });
    } else {
        $('.nav-site').draggable({
            helper: 'clone',
            delay: 150,
            stop: function (event) {
                var siteId = $(event.target).data('value');
                var $element = $p.hoverd($('.nav-site:not([data-value="' + siteId + '"])'));
                if ($element) {
                    $(event.target).hide();
                    var data = $p.getData($('.main-form'));
                    data.SiteId = siteId;
                    data.DestinationId = $element.attr('data-value');
                    $p.send($('#MoveSiteMenu'));
                }
            }
        });
    }
};

$p.openLinkDialog = function () {
    $('#LinkDialog').dialog({
        modal: true,
        width: '420px',
        appendTo: '.main-form',
        resizable: false
    });
};
