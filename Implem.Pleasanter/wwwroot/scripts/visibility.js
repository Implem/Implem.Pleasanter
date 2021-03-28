﻿$p.hideField = function (target, options) {
    $field = $(target + 'Field');
    if (options.Hide && !$field.hasClass('hidden')) {
        $field.addClass('hidden');
    } else if (options.Hide !== true && $field.hasClass('hidden')) {
        $field.removeClass('hidden');
    }
}