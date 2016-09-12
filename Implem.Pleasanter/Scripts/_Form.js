$p.getFormId = function ($control) {
    return $control.closest('form').attr('id');
}

$p.getData = function (formId) {
    formId = formId !== undefined
        ? formId
        : $('.main-form').attr('id');
    if (!(formId in $p.data)) {
        $p.data[formId] = {};
    }
    return $p.data[formId];
}

$p.getDataByInnerElement = function ($control) {
    return $p.getData($p.getFormId($control));
}

$p.setData = function ($control) {
    var controlId = $control.attr('id');
    if (!$control.hasClass('not-transport')) {
        var data = $p.getDataByInnerElement($control);
        switch ($control.prop('type')) {
            case 'checkbox':
                data[controlId] = $control.prop('checked');
                break;
            case 'radio':
                if ($control.prop('checked')) {
                    data[$control.attr('name')] = $control.val();
                }
                break;
            default:
                switch ($control.prop('tagName')) {
                    case 'SPAN':
                        data[controlId] = $control.text();
                        break;
                    case 'SELECT':
                        if ($control.attr('multiple')){
                            $p.setMultiSelectData($control);
                        }else{
                            data[controlId] = $control.val();
                        }
                        break;
                    default:
                        data[controlId] = $control.val();
                        break;
                }
                break;
        }
    }
}

$p.setMustData = function ($form, action) {
    if (action.toLowerCase() === 'create') {
        $form.find('[class*="control-"]').each(function () {
            $p.setData($(this));
        });
    } else {
        $form.find('.must-transport').each(function () {
            $p.setData($(this));
        });
    }
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

$p.clearData = function (target, data, type) {
    if (data) {
        if (target === '') {
            for (controlId in data) {
                if (!$('#' + controlId).hasClass('control-selectable')) {
                    delete data[controlId];
                }
            }
        } else if (type === 'startsWith') {
            for (controlId in data) {
                if (controlId.indexOf(target) === 0) {
                    delete data[controlId];
                }
            }
        } else {
            if (target in data) {
                delete data[target];
            } else if ($(target).length !== 0) {
                delete data[$(target).attr('id')];
            }
        }
    }
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
}

$p.send = function ($eventSender, formId, async) {
    $form = formId !== undefined ? $('#' + formId) : $('.main-form');
    async = async !== undefined ? async : true;
    if ($eventSender.hasClass('validate')) {
        $form.validate();
        if (!$form.valid()) {
            $form.find(':input.error:first').focus();
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
    if (action !== undefined && method !== undefined) {
        var data = $p.getData($form.attr('id'));
        if (method !== 'get') {
            data.ControlId = $eventSender.attr('id');
            $p.setMustData($form, action);
        }
        return $p.ajax(
            $form.attr('action').replace('_action_', action.toLowerCase()),
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