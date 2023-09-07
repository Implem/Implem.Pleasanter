$p.viewMode = function ($control) {
    var url = $('.main-form').attr('action')
        .replace('_action_', $control.attr('data-action').toLowerCase());
    $p.ajax(url, 'post', $p.getData($control), $control);
    history.pushState(null, null, url);
    /*
        日付: 2023-08-15
        タスクID: 10109990
        開発者: CodLUCK QuanNA
    */
    if ($p.responsive()) {
        $p.openResponsiveMenu();
    }
    /* End タスクID: 10109990 */
}

$p.changeViewSelector = function ($control) {
    var action = $("option:selected", $control).attr('data-action');
    var url = action
        ? $('.main-form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        ViewSelector: $control.val(),
        ControlId: $control.attr('id')
    };
    $p.ajax(url, 'post', data, $control);
    history.pushState(null, null, url);
}
