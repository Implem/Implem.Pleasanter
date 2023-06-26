$p.initDashboard = function() {
    let layout = $('#DashboardPartLayouts').val();
    $p.gridstackInstance = GridStack.init({
        column: 20,
        cellHeight: 16
    });
    $p.gridstackInstance.load(JSON.parse(layout));
};

//現在のレイアウトを保存する
$p.UpdateDashboardPartLayouts = function () {
    let layouts = $p.gridstackInstance.save();
    layouts.forEach(item => item.content = "");
    $p.set($('#DashboardPartLayouts'), JSON.stringify(layouts))
    $p.send($("#UpdateDashboardPartLayouts"));
};

$(document).on('click', '.dashboard-timeline-titlebody', function() {
    $p.transition($(this).attr('data-url'));
});
