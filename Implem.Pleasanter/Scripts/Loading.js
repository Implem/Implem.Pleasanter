$p.loading = function ($control) {
    if ($control) {
        if ($control.prop('tagName') === 'BUTTON') {
            $control.prop('disabled', true).addClass('loading');
            var $icon = $control.find('.ui-icon');
            $icon
                .attr('data-css', $icon.prop('class'))
                .prop('class', 'ui-icon')
                .css('background-image', 'url(' + $('#Logo > a').attr('href') + 'images/loading.gif)');
        }
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
            .removeAttr('style')
            .prop('class', $icon.attr('data-css'));
    });
}