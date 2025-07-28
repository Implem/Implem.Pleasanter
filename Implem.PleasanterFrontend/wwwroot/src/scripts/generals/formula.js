$p.changeCalculationMethod = function ($control) {
    var action = $('option:selected', $control).attr('data-action');
    if ($('option:selected', $control).val() === 'Default') {
        $('.formula-display-error-check').addClass('hidden');
    } else {
        $('.formula-display-error-check').removeClass('hidden');
    }
    var url = action
        ? $('.main-form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        CalculationMethod: $control.val(),
        ControlId: $control.attr('id')
    };
    $p.ajax(url, 'post', data, $control);
};
