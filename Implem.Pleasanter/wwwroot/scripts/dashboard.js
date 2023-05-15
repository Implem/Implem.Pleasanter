$p.initDashboard = function() {
    let layout = $('#DashboardLayouts').val();
    let gridstack = GridStack.init();
    gridstack.load(JSON.parse(layout));
};

$(document).on('click', '.timeline-titlebody', function() {
    $p.transition($(this).attr('data-url'));
});

//gridstackで現在のレイアウトを取得
$(document).on('click', '#SaveDashboardLayout', function () {
    let gridstack = GridStack.init();
    let layout = gridstack.save();
    $('#DashboardLayouts').val(JSON.stringify(layout));
    $p.post('/Dashboard/SaveLayout', { layout: JSON.stringify(layout) });
});