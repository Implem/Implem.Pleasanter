$p.newByLink = function ($control) {
    var data = {};
    data.LinkId = $control.attr('data-id');
    data.FromSiteId = $control.attr('data-from-site-id');
    $p.ajax($('#BaseUrl').val() + $control.attr('data-to-site-id') + '/NewByLink', 'post', data);
}