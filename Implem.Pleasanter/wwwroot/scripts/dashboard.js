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

$p.setDashboardRefresh = function () {
    function refreshDashboardPart() {
        var partId = $(this).attr('id').substring($(this).attr('id').indexOf('_'));
        var roadElement = $('<span />').addClass('material-symbols-outlined dashboard-part-road').text('progress_activity');
        $('[id="DashboardPart' + partId + '"]').html(roadElement);
        var data = {
            dashboardPartId: $('#DashboardPart' + partId).attr('data-id')
        }
    }

    $('[id^="DashboardPart_"]').each(function (index, value) {
        var buttonElement = $('<button />')
            .attr('id', 'DashboardRefresh' + value.id.substring(value.id.indexOf('_')))
            .attr('type', 'button').on('click', refreshDashboardPart)
            .addClass('dashboard-part-refresh')
            .append($('<span />')
                .addClass('material-symbols-outlined')
                .text('refresh'));
        $(value).prepend(buttonElement);
    })

    $(document).on('mouseenter', '.grid-stack-item-content', function () {
        var partId = $(this).children().attr('id').substring($(this).children().attr('id').indexOf('_'));
        $("#DashboardRefresh" + partId).css('opacity', '1');
    });

    $(document).on('mouseleave', '.grid-stack-item-content', function () {
        var partId = $(this).children().attr('id').substring($(this).children().attr('id').indexOf('_'));
        $("#DashboardRefresh" + partId).css('opacity', '0');
    });

}