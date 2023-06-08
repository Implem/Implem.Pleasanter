$p.initDashboard = function() {
    let layout = $('#DashboardPartLayouts').val();
    let gridstack = GridStack.init();
    gridstack.load(JSON.parse(layout));
};

$(document).on('click', '.dashboard-timeline-titlebody', function() {
    $p.transition($(this).attr('data-url'));
});

//gridstackで現在のレイアウトを取得
$(document).on('click', '#SaveDashboardPartLayout', function () {
    let gridstack = GridStack.init();
    let layout = gridstack.save();
    $('#DashboardPartLayouts').val(JSON.stringify(layout));
    $p.post('/Dashboard/SaveLayout', { layout: JSON.stringify(layout) });
});


