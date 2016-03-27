$(function () {
    if ($('.edit-form .main-form').length === 1) {
        history.pushState(
            $('.edit-form .main-form').attr('action').replace('_action_', 'reload'),
            null,
            location.href);
    }
    $(window).on('popstate', function (e) {
        var state = e.originalEvent.state;
        if (state) {
            request(state, 'post');
        } else {
            history.back();
        }
    });
});