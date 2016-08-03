$(function () {
    if ($('.edit-form .main-form').length === 1) {
        var value = $('.edit-form .main-form')
            .attr('action')
            .replace('_action_', $p.methodType());
        history.pushState('Edit', '', value);
    }
    if ($('#SearchResults').length === 1) {
        history.pushState('Search', '', location.href);
    }
    $(window).on('popstate', function (e) {
        var state = e.originalEvent.state;
        var location = e.originalEvent.currentTarget.location;
        switch (state) {
            case 'Edit':
                $p.ajax(location.pathname.replace('/edit', '/reload'), 'post');
                break;
            case 'Search':
                $p.ajax(location.pathname.replace('/items/search', '/items/ajaxsearch') +
                    location.search + '&reload=1', 'get');
                break;
            default:
                history.back();
                break;
        }
    });
});