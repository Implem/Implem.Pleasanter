$p.initDashboard = function () {
    let layout = $('#DashbordLayouts').val();
    let gridstack = GridStack.init();
    gridstack.load(JSON.parse(layout));
};