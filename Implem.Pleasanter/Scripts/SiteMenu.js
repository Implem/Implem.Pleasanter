$p.setSiteMenu = function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('value');
            var $element = $p.hoverd($('.nav-site:not([value="' + siteId + '"])'));
            if ($element) {
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

$p.openTemplateDialog = function ($control) {
    var error = $p.syncSend($control, 'SitesForm');
    if (error === 0) {
        $p.setTemplates();
        $('#TemplateDialog').dialog({
            modal: true,
            width: '370px',
            appendTo: '#Application'
        });
    }
}

$p.setTemplates = function () {
    $('#TemplateSelector').selectable({
        selected: function (event, ui) {
            $(ui.selected)
                .addClass("ui-selected")
                .siblings()
                .removeClass("ui-selected")
                .each(function (key, value) {
                    $(value).find('*').removeClass("ui-selected");
                });
        },
        stop: function () {
            var $control = $(this);
            $('#SiteTitle').val($control.find('li.ui-selected').text());
            $p.setData($control);
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