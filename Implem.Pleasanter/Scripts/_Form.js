$p.getFormId = function ($control) {
    return $control.closest('form').attr('id');
}

$p.syncSend = function ($control, formId) {
    return $p.send($control, formId, false);
}

$p.send = function ($control, formId, async) {
    $form = formId !== undefined
        ? $('#' + formId)
        : $control.closest('form');
    async = async !== undefined ? async : true;
    if ($control.hasClass('validate')) {
        $form.validate();
        if (!$form.valid()) {
            $p.setValidationError($form);
            return false;
        }
    }
    var _confirm = $control.attr('data-confirm');
    if (_confirm !== undefined) {
        if (!confirm($p.display(_confirm))) {
            return false;
        }
    }
    var action = $control.attr('data-action');
    var method = $control.attr('data-method');
    if (method !== undefined) {
        var data = $p.getData($form);
        if (method !== 'get') {
            data.ControlId = $control.attr('id');
            $p.setMustData($form, action);
        }
        return $p.ajax(
            action !== undefined
                ? $form.attr('action').replace('_action_', action.toLowerCase())
                : location.href,
            method,
            method !== 'get' ? data : null,
            $control,
            async);
    }
}