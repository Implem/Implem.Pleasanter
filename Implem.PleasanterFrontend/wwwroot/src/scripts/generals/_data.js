$p.getData = function ($control) {
    formId = $p.getFormId($control);
    if (!(formId in $p.data)) {
        $p.data[formId] = {};
    }
    return $p.data[formId];
}

$p.set = function ($control, val) {
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
                        if ($control.hasClass('search') && val) {
                            var $form = $('#MainForm');
                            var url = $form.attr('action').replace('_action_', 'SelectSearchDropDown');
                            var arr = $control.attr('multiple')
                                ? JSON.parse(val)
                                : new Array(val.toString());
                            if (arr.length > 0) {
                                var data = {};
                                data.DropDownSearchTarget = $control.attr('id');
                                if ($control.attr('multiple')) {
                                    data.DropDownSearchMultiple = true;
                                    data.DropDownSearchResultsAll = JSON.stringify(arr);
                                } else {
                                    data.DropDownSearchResults = JSON.stringify(arr);
                                }
                                $p.ajax(
                                    url,
                                    'post',
                                    data,
                                    $form,
                                    false);
                            }
                        }
                        if ($control.attr('multiple')) {
                            $p.selectMultiSelect($control, val);
                        } else {
                            $control.val(val);
                            $control.change();
                        }
                        break;
                    default:
                        if ($control.hasClass('radio-value')) {
                            // type="radio"のチェック変更
                            $('input[name="' + $control.attr('id') + '"]').val([val]);
                            // radio-valueへのデータ格納
                            $control.val(val);
                        } else {
                            $control.val(val);
                        }
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
        if (data === undefined) {
            data = $p.getData($control);
        }
        $p.setGridTimestamp($control, data);
        switch ($control.prop('type')) {
            case 'checkbox':
                data[controlId] = $control.prop('checked');
                break;
            case 'radio':
                if ($control.prop('checked')) {
                    // UIでラジオボタンを操作された場合には、こちらのルートを通るが
                    // $p.setで操作された場合には$controlはhiddenの.radio-valueとなるため
                    // このルートを通らずswitch ($control.prop('tagName'))のdefaultを通る
                    var name = $control.attr('name');
                    // $p.dataへのデータ格納
                    data[name] = $control.val();
                    // radio-valueへのデータ格納
                    $('[id="' + name + '"]').val($control.val());
                    // 入力必須エラーが表示されている場合には削除
                    $('[id="' + name + '-error"]').remove();
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

$p.setGridTimestamp = function ($control, data) {
    var timestamp = $control.closest('.grid-row').find('.timestamp');
    if (timestamp.length === 1) {
        data[timestamp.attr('id')] = timestamp.val();
    }
}

$p.setAndSend = function (selector, $control) {
    $p.setData($(selector));
    $p.send($control);
}

$p.setMustData = function ($form, action) {
    if (action !== undefined && action.toLowerCase() === 'create') {
        // 新規作成時はコントロールの値を全て送信する
        // 読み取り専用(SPAN)でCssClassにalways-sendの設定がないものを除く
        $form.find('[class*="control-"]:not(span:not(.always-send))').each(function () {
            $p.setData($(this));
        });
    } else if (action !== undefined && action.toLowerCase() === 'bulkupdate') {
        $form.find('[class*="control-"]').each(function () {
            $p.setData($(this));
        });
    } else {
        $form.find('.always-send,[data-always-send="1"]').each(function () {
            var $control = $(this);
            if (!($control.attr('data-readonly') === '1'
                && $control.attr('id').indexOf($p.tableName() + '_') === 0)) {
                $p.setData($(this));
            }
        });
    }
}

$p.clearData = function (target, data, type) {
    if (!data) {
        data = $p.getData($('.main-form'));
    }
    if (target === undefined) {
        for (controlId in data) {
            if (!$('#' + controlId).hasClass('control-selectable')) {
                Delete(controlId);
            }
        }
    } else if (type === 'startsWith') {
        for (controlId in data) {
            if (controlId.indexOf(target) === 0) {
                Delete(controlId);
            }
        }
    } else {
        if (target in data) {
            Delete(target);
        } else if ($(target).length !== 0) {
            Delete($(target).attr('id'));
        }
    }
    function Delete(key) {
        if (type === 'ignoreView') {
            if (key.indexOf('View') === 0) {
                return;
            }
        }
        delete data[key];
    }
};

$p.toJson = function ($control) {
    return JSON.stringify($control.map(function () {
        return $(this).attr('data-value') === undefined
            ? $(this).text()
            : $(this).attr('data-value')
    }).toArray());
}