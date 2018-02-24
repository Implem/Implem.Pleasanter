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
    var dialogs = $('.ui-dialog:visible').map(function (i, e)
    {
        return $('#' + e.getAttribute('aria-describedby'));
    });
    return dialogs.length !== 0 &&
        dialogs.filter(function (i, e)
        {
            return $control.closest(e).length === 1
        }).length === 0;
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
            $p.setErrorMessage('ValidationError');
            $("html,body").animate({ scrollTop: $('.error:first').offset().top });
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