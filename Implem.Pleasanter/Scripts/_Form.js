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

$p.send = function ($eventSender, formId, async) {
    $form = formId !== undefined
        ? $('#' + formId)
        : $eventSender.closest('form');
    async = async !== undefined ? async : true;
    if ($eventSender.hasClass('validate')) {
        $form.validate();
        if (!$form.valid()) {
            $p.setValidationError($form);
            return false;
        }
    }
    var _confirm = $eventSender.attr('data-confirm');
    if (_confirm !== undefined) {
        if (!confirm($p.display(_confirm))) {
            return false;
        }
    }
    var action = $eventSender.attr('data-action');
    var method = $eventSender.attr('data-method');
    if (method !== undefined) {
        var data = $p.getData($form.attr('id'));
        if (method !== 'get') {
            data.ControlId = $eventSender.attr('id');
            $p.setMustData($form, action);
        }
        return $p.ajax(
            action !== undefined
                ? $form.attr('action').replace('_action_', action.toLowerCase())
                : location.href,
            method,
            method !== 'get' ? data : null,
            $eventSender,
            async);
    }
}

$p.setByJson = function (json, data, $eventSender) {
    if (json) {
        $.each(json, function () {
            $p.setByJsonElement(this, data, $eventSender);
        });
    }
    if (json.filter(function (d) {
        return d.Method === 'Html' || d.Method === 'ReplaceAll'
    }).length > 0) {
        $p.apply();
    }
}

$p.setByJsonElement = function (jsonElement, data, $eventSender) {
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
                $eventSender.after(value);
            }
            break;
        case 'Before':
            if ($(target).length !== 0) {
                $(target).before(value);
            } else {
                $eventSender.before(value);
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