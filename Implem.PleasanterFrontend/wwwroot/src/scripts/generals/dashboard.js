$p.initDashboard = function () {
    const isMobile = navigator.userAgent.includes('Mobile');
    let layout = $('#DashboardPartLayouts').val();
    $p.gridstackInstance = GridStack.init({
        column: 20,
        cellHeight: 16,
        oneColumnSize: 1024,
        draggable: { cancel: '.no-drag' },
        disableDrag: isMobile
    });
    $p.gridstackInstance.load(JSON.parse(layout));
    $p.setDashboardPartRefreshButtons();

    $p.gridstackInstance.on('resizestop dragstop', function (event, el) {
        let layouts = $p.gridstackInstance.save();
        layouts.forEach(item => (item.content = ''));
        $p.set($('#DashboardPartLayout'), JSON.stringify(layouts));
        $p.send($('#DashboardPartLayout'));
    });
};

//現在のレイアウトを保存する
$p.updateDashboardPartLayouts = function () {
    let layouts = $p.gridstackInstance.save();
    layouts.forEach(item => (item.content = ''));
    $p.set($('#DashboardPartLayouts'), JSON.stringify(layouts));
    $p.send($('#UpdateDashboardPartLayouts'));
};

$(document).on('click', '.dashboard-timeline-item', function () {
    $p.transition($(this).attr('data-url'));
});

$p.addDashboardPartAccessControl = function () {
    $('#SourceDashboardPartAccessControl li.ui-selected').appendTo(
        '#CurrentDashboardPartAccessControl'
    );
    $p.setData($('#CurrentDashboardPartAccessControl'));
};

$p.deleteDashboardPartAccessControl = function () {
    $('#CurrentDashboardPartAccessControl li.ui-selected').appendTo(
        '#SourceDashboardPartAccessControl'
    );
};

function refreshDashboardPart(partId) {
    var roadElement = $('<span />')
        .addClass('material-symbols-outlined dashboard-part-road')
        .text('progress_activity');
    var $control = $('[id="DashboardPart_' + partId + '"]');
    $control.html(roadElement);
    var data = {
        dashboardPartId: partId
    };
    $p.ajax('DashboardPart', 'get', data, $control, true);
}

$p.setDashboardAsync = function () {
    $('[id^="DashboardAsync_"]').each(function (index, value) {
        var partId = value.id.substring(value.id.indexOf('_') + 1);
        refreshDashboardPart(partId);
    });
};

$p.setDashboardPartRefreshButtons = function () {
    if (!$p.gridstackInstance) {
        return;
    }
    $($p.gridstackInstance.el)
        .children('.grid-stack-item')
        .each(function () {
            const $gridStackItem = $(this);
            const partElementId = $gridStackItem
                .children('.grid-stack-item-content')
                .children('[id^="DashboardPart_"]')
                .attr('id');
            if (!partElementId) {
                return;
            }
            const partId = partElementId.substring(partElementId.indexOf('_') + 1);
            if (!partId) {
                return;
            }
            if ($gridStackItem.children('[id="DashboardRefresh_' + partId + '"]').length > 0) {
                return;
            }
            const $buttonElement = $('<button />')
                .attr('id', 'DashboardRefresh_' + partId)
                .attr('type', 'button')
                .on('click', function () {
                    refreshDashboardPart(partId);
                })
                .addClass('dashboard-part-refresh')
                .append($('<span />').addClass('material-symbols-outlined').text('refresh'));
            $gridStackItem.append($buttonElement);
        });
};

$(document).on('mouseenter', '.grid-stack-item:not(.grid-stack-placeholder)', function () {
    var partId = $(this)
        .find('[id^="DashboardPart_"]')
        .attr('id')
        .substring($(this).find('[id^="DashboardPart_"]').attr('id').indexOf('_'));
    $('#DashboardRefresh' + partId).css('opacity', '1');
});

$(document).on('mouseleave', '.grid-stack-item:not(.grid-stack-placeholder)', function () {
    var partId = $(this)
        .find('[id^="DashboardPart_"]')
        .attr('id')
        .substring($(this).find('[id^="DashboardPart_"]').attr('id').indexOf('_'));
    $('#DashboardRefresh' + partId).css('opacity', '0');
});
