var $p = {
    data: {},
    events: {},
    ex: {}
};

$p.initDashboard = function () {
    let layout = $('#dashbordlayout').val();
    let gridstack = GridStack.init();
    gridstack.load(JSON.parse(layout));
};