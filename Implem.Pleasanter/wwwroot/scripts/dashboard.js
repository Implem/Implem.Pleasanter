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

$(document).on('click', '.dashboard-timeline-item', function() {
    $p.transition($(this).attr('data-url'));
});

$p.addDashboardPartAccessControl = function () {
    $('#SourceDashboardPartAccessControl li.ui-selected').appendTo('#CurrentDashboardPartAccessControl');
    $p.setData($('#CurrentDashboardPartAccessControl'));
}

$p.deleteDashboardPartAccessControl = function () {
    $('#CurrentDashboardPartAccessControl li.ui-selected').appendTo('#SourceDashboardPartAccessControl');
}