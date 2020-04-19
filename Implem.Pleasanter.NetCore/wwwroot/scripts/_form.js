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
    var dialogs = $('.ui-dialog:visible').map(function (i, e) {
        return $('#' + e.getAttribute('aria-describedby'));
    });
    return dialogs.length !== 0 &&
        dialogs.filter(function (i, e) {
            return $control.closest(e).length === 1
        }).length === 0;
}

$p.syncSend = function ($control, formId) {
    return $p.send($control, formId, false);
}

$p.send = function ($control, formId, _async, clearMessage) {
    if ($p.outsideDialog($control)) return false;
    if ($control.hasClass('no-send')) return false;
    $form = formId !== undefined
        ? $('#' + formId)
        : $control.closest('form');
    var action = $control.attr('data-action');
    var methodType = $control.attr('data-method');
    var data = $p.getData($form);
    var url = action !== undefined
        ? $form.attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    _async = _async !== undefined ? _async : true;
    if (methodType !== 'get') {
        data.ControlId = $control.attr('id');
        $p.setMustData($form, action);
    }
    if ($control.hasClass('validate')) {
        if ($p.before_validate($p.eventArgs(url, methodType, data, $control, _async)) === false) {
            return false;
        }
        $form.validate();
        if (!$form.valid()) {
            $p.setValidationError($form);
            $p.setErrorMessage('ValidationError');
            if (!$control.closest('.ui-dialog')) {
                $("html,body").animate({
                    scrollTop: $('.error:first').offset().top
                });
            }
            return false;
        }
        if ($p.after_validate($p.eventArgs(url, methodType, data, $control, _async)) === false) {
            return false;
        }
    }
    if (methodType !== undefined) {
        return $p.ajax(
            url,
            methodType,
            methodType !== 'get' ? data : null,
            $control,
            _async,
            clearMessage);
    }
}

$p.setFormChanged = function ($control) {
    if (!$control.hasClass('not-set-form-changed')) {
        $p.formChanged = true;
    }
}

$p.throttle = (function () {
    var lastTime = 0;
    return function (action, interval) {
        if (lastTime + interval <= new Date().getTime()) {
            lastTime = new Date().getTime();
            action();
        }
    };
})();

$p.debounce = (function () {
    let timer;
    return function (action, interval) {
        clearTimeout(timer);
        timer = setTimeout(function () {
            action();
        }, interval);
    };
})();