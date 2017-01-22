$p.getFormId = function ($control) {
    return $control.closest('form').attr('id');
}

$p.clear = function ($control) {
    var controlId = $control.attr('id');
    var data = $p.getData($control);
    switch ($control.prop('tagName')) {
        case 'INPUT':
            switch ($control.prop('type')) {
                case 'checkbox':
                    $control.prop('checked', false);
                    break;
                case 'text':
                    $control.val('');
                    break;
            }
            break;
        case 'SELECT':
            if ($control.attr('multiple')) {
                $control
                    .addClass('no-postback')
                    .multiselect("uncheckAll")
                    .removeClass('no-postback');
            } else {
                $control.val('');
            }
            break;
    }
    $p.clearData(controlId, data);
}

$p.outsideDialog = function ($control) {
    var $dialog = $('.ui-dialog:visible');
    return $dialog.length !== 0 &&
        $control.closest($('#' + $dialog.attr('aria-describedby'))).length === 0;
}

$p.syncSend = function ($control, formId) {
    return $p.send($control, formId, false);
}

$p.send = function ($control, formId, async) {
    if ($p.outsideDialog($control)) return false;
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