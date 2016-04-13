$(function () {
    if ($('.edit-form .main-form').length === 1) {
        var value = $('.edit-form .main-form')
            .attr('action')
            .replace('_action_', $('#MethodType').val());
        history.pushState('/reload', '', value);
    }
    $(window).on('popstate', function (e) {
        var state = e.originalEvent.state;
        if (state) {
            request(location.pathname.replace(/\/edit$/i, state), 'post');
        } else {
            history.back();
        }
    });
});