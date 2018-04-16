$p.getData = function ($control) {
    formId = $p.getFormId($control);
    if (!(formId in $p.data)) {
        $p.data[formId] = {};
    }
    return $p.data[formId];
}

$p.set = function ($control, val) {
    var controlId = $control.attr('id');
    if ($control.length === 1) {
        switch ($control.prop('type')) {
            case 'checkbox':
                $control.prop('checked', val);
                break;
            case 'textarea':
                $control.val(val);
                $p.showMarkDownViewer($control);
                break;
            default:
                switch ($control.prop('tagName')) {
                    case 'SELECT':
                        $control.val(val);
                        $control.change();
                        break;
                    default:
                        $control.val(val);
                        break;
                }
                break;
        }
        $p.setData($control);
    }
}

$p.setData = function ($control, data) {
    var controlId = $control.attr('id');
    if (!$control.hasClass('not-send')) {
        if (data === undefined) data = $p.getData($control);
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
                        data[controlId] = $control.attr('data-value') !== undefined
                            ? $control.attr('data-value')
                            : $control.text();
                        break;
                    case 'SELECT':
                        if ($control.attr('multiple')) {
                            $p.setMultiSelectData($control);
                        } else {
                            data[controlId] = $control.val();
                        }
                        break;
                    case 'OL':
                        if ($control.hasClass('control-selectable')) {
                            data[controlId] = $p.toJson($control.find('li.ui-selected'));
                            if ($control.hasClass('send-all')) {
                                data[controlId + 'All'] = $p.toJson($control.find('li'));
                            }
                        } else {
                            data[controlId] = $p.toJson($control.find('li'));
                        }
                        break;
                    case 'TABLE':
                        data[controlId] = JSON.stringify($control
                            .find('.select')
                            .filter(':checked')
                            .map(function () {
                                return $(this).closest('.grid-row').attr('data-id');
                            })
                            .toArray());
                        break;
                    case 'P':
                        if ($control.hasClass('control-slider')) {
                            data[controlId] = $control.attr('data-value');
                        } else {
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

$p.setAndSend = function (selector, $control) {
    $p.setData($(selector));
    $p.send($control);
}

$p.setMustData = function ($form, action) {
    if (action !== undefined && action.toLowerCase() === 'create') {
        $form.find('[class*="control-"]').each(function () {
            $p.setData($(this));
        });
    } else {
        $form.find('.always-send').each(function () {
            $p.setData($(this));
        });
    }
}

$p.clearData = function (target, data, type) {
    if (data === null) {
        data = $p.getData($('.main-form'))
    }
    if (target === undefined) {
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

$p.toJson = function ($control) {
    return JSON.stringify($control.map(function () {
        return $(this).attr('data-value') === undefined
            ? $(this).text()
            : $(this).attr('data-value')
    }).toArray());
}