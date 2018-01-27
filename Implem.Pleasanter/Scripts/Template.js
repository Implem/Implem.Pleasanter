$p.templates = function ($control) {
    $p.send($control, 'SitesForm');
}

$p.setTemplate = function () {
    var selector = '.template-selectable .control-selectable';
    $('#TemplateTabsContainer:not(.applied)').tabs({
        beforeActivate: function (event, ui) {
            $p.setTemplateData(ui.newPanel.find(selector));
        }
    }).addClass('applied');
    $(selector).selectable({
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
            $p.setTemplateData($(this));
        }
    });
}

$p.setTemplateData = function ($control) {
    var selected = $control.find('li.ui-selected');
    var show = selected.length === 1;
    $p.setData($control);
    $p.send($control);
    $('#OpenSiteTitleDialog')
        .removeClass('hidden')
        .toggle(show);
    $('#SiteTitle').val(show
        ? selected[0].innerText
        : '');
    $('#TemplateId').val(show
        ? selected[0].getAttribute('data-value')
        : '');
}

$p.setTemplateViewer = function () {
    $('.template-tab-container').tabs().addClass('applied');
}

$p.openSiteTitleDialog = function ($control) {
    $('#SiteTitleDialog').dialog({
        modal: true,
        width: '370px',
        appendTo: '#Application'
    });
}