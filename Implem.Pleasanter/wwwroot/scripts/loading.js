$p.loading = function ($control) {
    $('#LoaderContainer').css('display', 'block');
    if ($control) {
        if ($control.prop('tagName') === 'BUTTON') {
            $control.prop('disabled', true).addClass('loading');
            var $icon = $control.find('.ui-icon');
            var themeName = $p.theme();
            $icon
                .attr('data-css', $icon.prop('class'))
                .prop('class', 'ui-icon')
                .css('background-image', 'url(' + $('#ApplicationPath').val() + 'styles/plugins/themes/' + themeName + '/images/loading.gif)');
        }
    }
}

$p.loaded = function () {
    $('#LoaderContainer').css('display', 'none');
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