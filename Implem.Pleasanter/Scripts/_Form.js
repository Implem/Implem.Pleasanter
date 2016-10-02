$p.getFormId = function ($control) {
    return $control.closest('form').attr('id');
}

$p.setValue = function ($control, value) {
    switch ($control.prop('type')) {
        case 'checkbox':
            $control.prop('checked', value);
            break;
        case 'radio':
            $control.val([value]);
            break;
        default:
            switch ($control.prop('tagName')) {
                case 'SPAN':
                    $control.html(value);
                case 'TIME':
                    $control.html(value);
                    $control.attr('datetime', value);
                    break;
                default:
                    $control.val(value);
                    break;
            }
    }
}

$p.setMessage = function (value) {
    var $control = $('.message-dialog:visible');
    if ($control.length === 0) {
        $('#Message').html(value);
    } else {
        $control.html(value);
    }
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
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

$p.setByJson = function (json, data, $control) {
    if (json) {
        $.each(json, function () {
            $p.setByJsonElement(this, data, $control);
        });
    }
    if (json.filter(function (d) {
        return d.Method === 'Html' || d.Method === 'ReplaceAll'
    }).length > 0) {
        $p.apply();
    }
}

$p.setByJsonElement = function (jsonElement, data, $control) {
    var method = jsonElement.Method;
    var target = jsonElement.Target;
    var value = jsonElement.Value;
    switch (method) {
        case 'Html':
            $(target).html(value);
            break;
        case 'ReplaceAll':
            $(value).replaceAll(target);
            break;
        case 'Message':
            $p.setMessage(value);
            break;
        case 'Href':
            location.href = value;
            break;
        case 'PushState':
            history.pushState(target, '', value);
            break;
        case 'SetFormData':
            data[target] = value;
            break;
        case 'Append':
            $(target).append(value);
            break;
        case 'Prepend':
            $(target).prepend(value);
            break;
        case 'After':
            if ($(target).length !== 0) {
                $(target).after(value);
            } else {
                $control.after(value);
            }
            break;
        case 'Before':
            if ($(target).length !== 0) {
                $(target).before(value);
            } else {
                $control.before(value);
            }
            break;
        case 'Remove':
            $(target).remove();
            break;
        case 'Focus':
            if (target === '') {
                $('#' + data.ControlId).focus();
            } else {
                $(target).focus();
            }
            break;
        case 'SetValue':
            $p.setValue($(target), value);
            break;
        case 'ClearFormData':
            $p.clearData(target, data, value);
            break;
        case 'CloseDialog':
            $('.ui-dialog-content').dialog('close');
            break;
        case 'Trigger':
            $(target).trigger(value);
            break;
        case 'Invoke':
            $p[target]();
        case 'WindowScrollTop':
            $(window).scrollTop(value);
            break;
        case 'FocusMainForm':
            $p.focusMainForm();
            break;
        case 'Empty':
            $(target).empty();
            break;
        case 'Disabled':
            $(target).prop('disabled', value);
            break;
    }
}