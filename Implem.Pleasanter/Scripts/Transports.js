var formData = {};
var transportQueue = {};

function getForm($control) {
    if ($control.prop('tagName') === 'FORM') {
        return $control;
    }
    else {
        return $control.closest('form');
    }
}

function getFormId($control) {
    return getForm($control).attr('id');
}

function getFormData($control) {
    var formId = getFormId($control);
    if (!(formId in formData)) {
        formData[formId] = {};
    }
    return formData[formId];
}

function setFormData($control) {
    if (!$control.hasClass('not-transport')) {
        var data = getFormData($control);
        switch ($control.prop('type')) {
            case 'checkbox':
                data[$control.attr('id')] = $control.prop('checked');
                break;
            case 'radio':
                if ($control.prop('checked')) {
                    data[$control.attr('name')] = $control.val();
                }
                break;
            default:
                switch ($control.prop('tagName')) {
                    case 'SPAN':
                        data[$control.attr('id')] = $control.text();
                        break;
                    default:
                        data[$control.attr('id')] = $control.val();
                        break;
                }
                break;
        }
    }
}

function setValueAndFormData($control, value, data) {
    setValue($control, value);
    if (data === undefined) {
        setFormData($control);
    } else {
        data[$control.attr('id')] = value;
    }
}

function setValue($control, value) {
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
                    $control.val('' + value + '');
                    break;
            }
    }
}

function clearFormData(target, data, type) {
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

function requestByForm($form, $eventSender, async) {
    async = (async === null) ? true : async;
    $('[class*="message"]').html('');
    if ($eventSender.hasClass('validate')) {
        $form.validate();
        if (!$form.valid()) {
            $form.find(':input.error:first').focus();
            return false;
        }
    }
    if ($eventSender.attr('data-confirm') !== undefined) {
        if (!confirm(getDisplay($eventSender.attr('data-confirm')))) {
            return false;
        }
    }
    var action = $eventSender.attr('data-action');
    var method = $eventSender.attr('data-method');
    if (action !== undefined && method !== undefined) {
        var data;
        if (method !== 'get') {
            if (!($form.attr('id') in formData)) {
                formData[$form.attr('id')] = {};
            }
            if ($eventSender.attr('id')) {
                formData[$form.attr('id')].ControlId = $eventSender.attr('id');
            }
            setMustData($form, action);
            data = formData[$form.attr('id')];
        }
        return request(
            $form.attr('action').replace('_action_', action.toLowerCase()),
            method,
            data,
            $eventSender,
            async);

        function setMustData($form, action) {
            if (action.toLowerCase() === 'create') {
                $form.find('[class*="control-"]').each(function () {
                    setFormData($(this));
                });
            } else {
                $form.find('.must-transport').each(function () {
                    setFormData($(this));
                });
            }
        }
    }
}

function requestFile(requestUrl, methodType, data, $eventSender) {
    $('[class*="message"]').html('');
    return $.ajax({
        url: requestUrl,
        type: methodType,
        cache: false,
        contentType: false,
        processData: false,
        data: data
    })
    .done(function (response, textStatus, jqXHR) {
        processResponse(JSON.parse(response), data, $eventSender);
        return true;
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        alert(textStatus + '\n' +
            $(jqXHR.responseText).text().trim().replace('\n', ''));
        return false;
    })
    .always(function (jqXHR, textStatus) {
        clearFormData('ControlId', data);
        delete transportQueue[requestUrl];
    });
}

function request(requestUrl, methodType, data, $eventSender, async) {
    var ret = 0;
    async = (async === null) ? true : async;
    $('#Message, .message-dialog').html('');
    var queueNumber = requestUrl in transportQueue
        ? transportQueue[requestUrl] + 1
        : 1;
    while (requestUrl in transportQueue) {
        if (transportQueue[requestUrl] !== queueNumber) {
            return false;
        }
    }
    transportQueue[requestUrl] = queueNumber;
    $.ajax({
        url: requestUrl,
        type: methodType,
        async: async,
        cache: false,
        data: data,
        dataType: 'json'
    })
    .done(function (response, textStatus, jqXHR) {
        processResponse(response, data, $eventSender);
        ret = response.filter(function (i) {
            return i.Method === 'Status' && i.Value === 'alert-error';
        }).length !== 0
            ? -1
            : 0;
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        alert(textStatus + '\n' +
            $(jqXHR.responseText).text().trim().replace('\n', ''));
        ret = -1;
    })
    .always(function (jqXHR, textStatus) {
        clearFormData('ControlId', data);
        delete transportQueue[requestUrl];
    });
    return ret;
}

function processResponse(response, data, $eventSender) {
    if (response) {
        $.each(response, function () {
            apply(this, data, $eventSender);
        });
    }
    if (response.filter(function (d) {
        return d.Method === 'Html' || d.Method === 'ReplaceAll'
    }).length > 0) {
        func.setUi();
    }

    function apply(response, data, $eventSender) {
        var method = response['Method'];
        var target = response['Target'];
        var value = response['Value'];
        switch (method) {
            case 'Html':
                $(target).html(value);
                break;
            case 'ReplaceAll':
                $(value).replaceAll(target);
                break;
            case 'Message':
                setMessage(value);
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
                    $('#' + data['ControlId']).focus();
                } else {
                    $(target).focus();
                }
                break;
            case 'SetValue':
                setValue($(target), value);
                break;
            case 'SetValueAndFormData':
                setValueAndFormData($(target), value, data);
                break;
            case 'Markup':
                $('.markup').each(function () {
                    $(this).html('' + getMarkedUp($(this).html(), true) + '');
                    $(this).removeClass('markup');
                });
                break;
            case 'ClearFormData':
                clearFormData(target, data, value);
                break;
            case 'CloseDialog':
                $('.ui-dialog-content').dialog('close');
                break;
            case 'Trigger':
                $(target).trigger(value);
                break;
            case 'Func':
                func[target]();
                break;
            case 'Validation':
                $p.form.validators[target]();
            case 'WindowScrollTop':
                $(window).scrollTop(value);
                break;
            case 'FocusMainForm':
                focusMainForm();
                break;
            case 'Empty':
                $(target).empty();
                break;
            case 'Disabled':
                $(target).prop('disabled', value);
                break;
        }

        function setMessage(value) {
            var $control = $('.message-dialog:visible');
            if ($control.length === 0) {
                $('#Message').html(value);
            } else {
                $control.html(value);
            }
        }
    }
}