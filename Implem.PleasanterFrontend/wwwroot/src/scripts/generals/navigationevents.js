$(function () {
    $(window).on('popstate', function (e) {
        if (e.originalEvent.currentTarget.location.pathname !== $('#BaseUrl').val() + $('#Id').val()
            || e.originalEvent.state === "History" || urlParams()["ver"]) {
            $p.ajax(e.originalEvent.currentTarget.location, 'post');
        }
    });
    $p.setSwitchTargets();
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $control.appendTo('body');
    }

    function urlParams() {
        var urlParam = location.search.substring(1);
        var paramArray = [];
        if (urlParam) {
            var param = urlParam.split('&');
            for (i = 0; i < param.length; i++) {
                var paramItem = param[i].split('=');
                paramArray[paramItem[0]] = paramItem[1];
            }
        }
        return paramArray;
    }
});