$p.setValidationError = function ($form) {
    $form.find('.ui-tabs li').each(function () {
        $control = $('#' + $(this).attr('aria-controls')).find('input.error:first');
        if ($control.length === 1) {
            $(this).closest('.ui-tabs').tabs('option', 'active', $(this).index());
            $control.focus();
        }
    });
}