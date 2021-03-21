$p.hideField = function (target, options) {
    $field = $(target + 'Field');
    if (options.Hide && !$field.hasClass('hidden')) {
        $field.addClass('hidden');
    } else if ($field.hasClass('hidden')) {
        $field.removeClass('hidden');
    }
}