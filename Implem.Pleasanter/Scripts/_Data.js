$p.getData = function ($control) {
    formId = $p.getFormId($control);
    if (!(formId in $p.data)) {
        $p.data[formId] = {};
    }
    return $p.data[formId];
}

$p.setData = function ($control) {
    var controlId = $control.attr('id');
    if (!$control.hasClass('not-transport')) {
        var data = $p.getData($control);
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
                        if ($control.attr('multiple')) {
                            $p.setMultiSelectData($control);
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

$p.setMustData = function ($form, action) {
    if (action !== undefined && action.toLowerCase() === 'create') {
        $form.find('[class*="control-"]').each(function () {
            $p.setData($(this));
        });
    } else {
        $form.find('.must-transport').each(function () {
            $p.setData($(this));
        });
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