$p.initDashboard = function () {
    const isMobile = navigator.userAgent.includes('Mobile');
    let layout = $('#DashboardPartLayouts').val();
    $p.gridstackInstance = GridStack.init({
        column: 20,
        cellHeight: 16,
        oneColumnSize: 1024,
        draggable: { cancel: ".no-drag" },
        disableDrag: isMobile,
    });
    $p.gridstackInstance.load(JSON.parse(layout));

    $p.gridstackInstance.on('resizestop dragstop', function (event, el) {
        var node = el.gridstackNode;
        var layouts = {
            id:node.id,
            x: node.x,
            y: node.y,
            w:node.w,
            h:node.h,
            content: ""
        }
        $p.set($('#DashboardPartLayout'), JSON.stringify(layouts))
        $p.send($('#DashboardPartLayout'));
    })
};

//現在のレイアウトを保存する
$p.updateDashboardPartLayouts = function () {
    let layouts = $p.gridstackInstance.save();
    layouts.forEach(item => item.content = "");
    $p.set($('#DashboardPartLayouts'), JSON.stringify(layouts))
    $p.send($("#UpdateDashboardPartLayouts"));
};

$(document).on('click', '.dashboard-timeline-item', function () {
    $p.transition($(this).attr('data-url'));
});

$p.addDashboardPartAccessControl = function () {
    $('#SourceDashboardPartAccessControl li.ui-selected').appendTo('#CurrentDashboardPartAccessControl');
    $p.setData($('#CurrentDashboardPartAccessControl'));
}

$p.deleteDashboardPartAccessControl = function () {
    $('#CurrentDashboardPartAccessControl li.ui-selected').appendTo('#SourceDashboardPartAccessControl');
}

function refreshDashboardPart(partId) {
    var roadElement = $('<span />').addClass('material-symbols-outlined dashboard-part-road').text('progress_activity');
    var $control = $('[id="DashboardPart_' + partId + '"]');
    $control.html(roadElement);
    var data = {
        dashboardPartId: partId
    }
    $p.ajax('DashboardPart', 'get', data, $control, true);
}

$p.setDashboardAsync = function () {
    $('[id^="DashboardAsync_"]').each(function (index, value) {
        var partId = value.id.substring(value.id.indexOf('_') + 1);
        refreshDashboardPart(partId);
    })
}

$(document).on('mouseenter', '.grid-stack-item:not(.grid-stack-placeholder)', function () {
    var partId = $(this).find('[id^="DashboardPart_"]').attr('id').substring($(this).find('[id^="DashboardPart_"]').attr('id').indexOf('_'));
    $("#DashboardRefresh" + partId).css('opacity', '1');
});

$(document).on('mouseleave', '.grid-stack-item:not(.grid-stack-placeholder)', function () {
    var partId = $(this).find('[id^="DashboardPart_"]').attr('id').substring($(this).find('[id^="DashboardPart_"]').attr('id').indexOf('_'));
    $("#DashboardRefresh" + partId).css('opacity', '0');
});

$(function () {
    var partElement = $(".grid-stack-item-content").get()
    $($(partElement)).each(function (index, value) {
        var partId = $(this).children().attr('id').substring($(this).children().attr('id').indexOf('_') + 1);
        var buttonElement = $('<button />')
            .attr('id', 'DashboardRefresh_' + partId)
            .attr('type', 'button').on('click', function () { refreshDashboardPart(partId) })
            .addClass('dashboard-part-refresh')
            .append($('<span />')
                .addClass('material-symbols-outlined')
                .text('refresh'));
        $(this).parent('.grid-stack-item').append(buttonElement);
    })
});