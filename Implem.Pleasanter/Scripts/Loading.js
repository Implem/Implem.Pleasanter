$p.loading = function ($control) {
    if ($control) {
        $control.prop('disabled', true).addClass('loading');
        var $icon = $control.find('.ui-icon');
        $icon
            .attr('data-css', $icon.prop('class'))
            .prop('class', 'ui-icon')
            .css('background-image', 'url(/images/loading.gif)');
    }
}

$p.loaded = function () {
    $('button.loading').each(function () {
        var $control = $(this);
        $control
            .removeClass('loading')
            .prop('disabled', false);
        var $icon = $control.find('.ui-icon');
        $icon
            .prop('style', '')
            .prop('class', $icon.attr('data-css'));
    });
}