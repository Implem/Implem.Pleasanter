var $p = {
    data: {},
    events: {},
    ex: {}
};
$p.ajax = function (url, methodType, data, $control, _async, clearMessage) {
    if ($p.before_send($p.eventArgs(url, methodType, data, $control, _async)) === false) {
        return false;
    }
    if ($control) {
        var _confirm = $control.attr('data-confirm');
        if (_confirm !== undefined) {
            if (!confirm($p.display(_confirm))) {
                return false;
            }
        }
    }
    $p.loading($control);
    var ret = 0;
    _async = _async !== undefined ? _async : true;
    if (clearMessage !== false) {
        $p.clearMessage();
    }
    $.ajax({
        url: url,
        type: methodType,
        async: _async,
        cache: false,
        data: data,
        dataType: 'json'
    })
        .done(function (json, textStatus, jqXHR) {
            $p.setByJson(url, methodType, data, $control, _async, json);
            ret = json.filter(function (i) {
                return i.Method === 'Message' && JSON.parse(i.Value).Css === 'alert-error';
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
            $p.clearData('ControlId', data);
            $p.loaded();
        });
    $p.after_send($p.eventArgs(url, methodType, data, $control, _async, ret));
    return ret;
}

$p.multiUpload = function (url, data, $control, statusBar) {
    $p.loading($control);
    $p.clearMessage();
    var methodType = 'POST';
    var uploader = $.ajax({
        xhr: function () {
            var uploadobj = $.ajaxSettings.xhr();
            if (uploadobj.upload) {
                uploadobj.upload.addEventListener('progress', function (event) {
                    var percent = 0;
                    var position = event.loaded || event.position;
                    var total = event.total;
                    if (event.lengthComputable) {
                        percent = Math.ceil(position / total * 100);
                    }
                    if (statusBar !== undefined) {
                        statusBar.setProgress(percent);
                    }
                }, false);
            }
            return uploadobj;
        },
        url: url,
        type: methodType,
        contentType: false,
        processData: false,
        cache: false,
        data: data,
        success: function (data) {
            if (statusBar !== undefined) {
                statusBar.setProgress(100);
            }
        }
    })
        .done(function (json, textStatus, jqXHR) {
            $p.setByJson(url, methodType, data, $control, true, JSON.parse(json));
            return true;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + '\n' +
                $(jqXHR.responseText).text().trim().replace('\n', ''));
            return false;
        })
        .always(function (jqXHR, textStatus) {
            $p.clearData('ControlId', data);
            $p.loaded();
            if (statusBar !== undefined) {
                statusBar.status.hide();
            }
        });
    if (statusBar !== undefined) {
        statusBar.setAbort(uploader);
    }
}
$p.apiUrl = function (id, action) {
    return $('#ApplicationPath').val() + 'api/items/' + id + '/' + action;
}

$p.apiGet = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'get'), args);
}

$p.apiCreate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'create'), args);
}

$p.apiUpdate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'update'), args);
}

$p.apiDelete = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'delete'), args);
}

$p.apiExec = function (url, args) {
    return $.ajax({
        type: 'Post',
        url: url,
        data: args.data !== undefined
            ? JSON.stringify(args.data)
            : "",
        dataType: 'json',
        async: args.async !== undefined
            ? args.async
            : true
    })
        .done(args.done)
        .fail(args.fail)
        .always(args.always);
}

$p.apiUsersUrl = function (action, id) {
    switch (action) {
        case 'get':
        case 'create':
            return $('#ApplicationPath').val() + 'api/users/' + action;
            break;
        case 'update':
        case 'delete':
            return $('#ApplicationPath').val() + 'api/users/' + id + '/' + action;
            break;
    }
}

$p.apiUsersGet = function (args) {
    return $p.apiExec($p.apiUsersUrl('get'), args);
}

$p.apiUsersCreate = function (args) {
    return $p.apiExec($p.apiUsersUrl('create'), args);
}

$p.apiUsersUpdate = function (args) {
    return $p.apiExec($p.apiUsersUrl('update', args.id), args);
}

$p.apiUsersDelete = function (args) {
    return $p.apiExec($p.apiUsersUrl('delete', args.id), args);
}

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
                            if (arr.length === 1) {
                                var data = {};
                                data.DropDownSearchTarget = $control.attr('id');
                                data.DropDownSearchResults = JSON.stringify(arr);
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
        $form.find('[class*="control-"]').each(function () {
            $p.setData($(this));
        });
    } else {
        $form.find('.always-send,[data-always-send="1"]').each(function () {
            $p.setData($(this));
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
$p.setByJson = function (url, methodType, data, $control, _async, json) {
    $p.before_set($p.eventArgs(url, methodType, data, $control, _async, undefined, json));
    if (json) {
        $.each(json, function () {
            $p.setByJsonElement(this, data, $control);
        });
    }
    if (json.filter(function (d) {
        return d.Method === 'Html' ||
            d.Method === 'ReplaceAll' ||
            d.Method === 'Append' ||
            d.Method === 'Prepend' ||
            d.Method === 'After' ||
            d.Method === 'Before';
    }).length > 0) {
        $p.apply();
        $p.applyValidator();
    }
    $p.after_set($p.eventArgs(url, methodType, data, $control, _async, undefined, json));
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
            $p.setMessage(target, value);
            break;
        case 'Href':
            $control.addClass('no-send');
            location.href = value;
            break;
        case 'PushState':
            history.pushState(target, '', value);
            break;
        case 'SetData':
            $p.setData($(target));
            break;
        case 'SetFormData':
            data[target] = value;
            break;
        case 'SetMemory':
            $p[target] = value;
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
        case 'InsertText':
            $p.insertText($(target), value);
            break;
        case 'Remove':
            $(target).remove();
            break;
        case 'Attr':
            var json = JSON.parse(value);
            $(target).attr(json.Name, json.Value);
            break;
        case 'RemoveAttr':
            $(target).removeAttr(value);
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
            $p.clearMessage();
            if (target !== undefined
                && $(target).hasClass('ui-dialog-content')) {
                $(target).dialog('close');
            } else {
                $('.ui-dialog-content').dialog('close');
            }
            break;
        case 'Paging':
            $p.paging(target);
            break;
        case 'Toggle':
            $(target).toggle(value === '1');
            break;
        case 'Trigger':
            $(target).trigger(value);
            break;
        case 'Invoke':
            $p[target](value);
            break;
        case 'Events':
            $p.execEvents(target, '');
            break;
        case 'WindowScrollTop':
            $(window).scrollTop(value);
            break;
        case 'ScrollTop':
            $(target).scrollTop(value);
            break;
        case 'LoadScroll':
            $p.loadScroll();
            break;
        case 'FocusMainForm':
            $p.focusMainForm();
            break;
        case 'Disabled':
            $(target).prop('disabled', value);
            break;
    }
}
$p.id = function () {
    return parseInt($('#Id').val());
}

$p.siteId = function (title) {
    if (title === undefined) {
        return parseInt($('#SiteId').val());
    } else {
        var sites = JSON.parse($('#JoinedSites').val()).filter(function (data) {
            return data.Title === title;
        });
        return sites.length > 0
            ? sites[0].SiteId
            : undefined
    }
}

$p.loginId = function () {
    return $('#LoginId').val();
}

$p.userId = function () {
    return parseInt($('#UserId').val());
}

$p.userName = function () {
    return $('#AccountUserName').text();
}

$p.referenceType = function () {
    return $('#ReferenceType').val();
}

$p.getColumnName = function (name) {
    var data = JSON.parse($('#Columns').val()).filter(function (column) {
        return column.LabelText === name || column.ColumnName === name
    });
    return data.length > 0
        ? data[0].ColumnName
        : undefined;
}

$p.getControl = function (name) {
    var columnName = $p.getColumnName(name);
    return columnName !== undefined
        ? $('#' + $('#ReferenceType').val() + '_' + columnName)
        : undefined;
}

$p.getField = function (name) {
    var columnName = $p.getColumnName(name);
    return columnName !== undefined
        ? $('#' + $('#ReferenceType').val() + '_' + columnName + 'Field')
        : undefined;
}

$p.getGridRow = function (id) {
    return $('#Grid > tbody > tr[data-id="' + id + '"]');
}

$p.getGridCell = function (id, name, excludeHistory) {
    return $('#Grid > tbody > tr[data-id="' + id + '"]' + (excludeHistory? ':not([data-history])' : '') + ' td:nth-child(' + ($p.getGridColumnIndex(name) + 1) + ')');
}

$p.getGridColumnIndex = function (name) {
    return $('#Grid > thead > tr > th').index($('#Grid > thead > tr > th[data-name="' + $p.getColumnName(name) + '"]'));
}

$p.on = function (events, name, func) {
    $(document).on(events, '#' + $p.getControl(name).attr('id'), func);
}
$p.eventArgs = function (url, methodType, data, $control, _async, ret, json) {
    var args = {};
    args.url = url;
    args.methodType = methodType;
    args.data = data;
    args.$control = $control;
    args.async = _async;
    args.ret = ret;
    args.json = json;
    return args;
}

$p.execEvents = function (event, args) {
    var result = exec(event);
    if (args.$control) {
        result = exec(event + '_' + args.$control.attr('id')) && result;
        result = exec(event + '_' + args.$control.attr('data-action')) && result;
    }
    return result;
    function exec(name) {
        if ($p.events[name] !== undefined) {
            return ($p.events[name](args) === false) ? false : true;
        }
    }
}

$p.before_validate = function (args) {
    return $p.execEvents('before_validate', args);
}

$p.after_validate = function (args) {
    return $p.execEvents('after_validate', args);
}

$p.before_send = function (args) {
    return $p.execEvents('before_send', args);
}

$p.after_send = function (args) {
    $p.execEvents('after_send', args);
}

$p.before_set = function (args) {
    $p.execEvents('before_set', args);
}

$p.after_set = function (args) {
    $p.execEvents('after_set', args);
}
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
$p.dateAdd = function (type, num, date) {
    switch (type) {
        case 'd':
            return new Date(new Date(date).setDate(date.getDate() + num));
    }
}

$p.dateDiff = function (type, date1, date2) {
    switch (type) {
        case 'd':
            return parseInt((date1 - date2) / 86400000);
    }
}

$p.shortDate = function (date) {
    if (date === undefined) date = new Date();
    return new Date($p.shortDateString(date));
}

$p.beginningMonth = function (date) {
    if (date === undefined) date = new Date();
    return new Date(date.getFullYear() + '/' + (date.getMonth() + 1) + '/1');
}

$p.shortDateString = function (date) {
    if (date === undefined) date = new Date();
    return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
}

$p.dateTimeString = function (date) {
    if (date === undefined) date = new Date();
    return $p.shortDateString(date) +
        (date.getHours() + date.getMinutes() !== 0
            ? ' ' + date.getHours() + ':' + date.getMinutes()
            : '');
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
$(function () {
    $(document).on('change', '[class^="control-"]:not(select[multiple])', function (e) {
        var $control = $(this);
        if ($control.hasClass('control-spinner')) {
            if ($control.val() === '' && $control.hasClass('allow-blank')) {
                $control.val('');
            } else if ($control.val() === '' ||
                $control.val().match(/[^0-9\.]/g) ||
                parseInt($control.val()) < parseInt($control.attr('data-min'))) {
                $control.val($control.attr('data-min'));
            } else if (parseInt($control.val()) > parseInt($control.attr('data-max'))) {
                $control.val($control.attr('data-max'));
            }
        }
        $p.setData($control);
        e.preventDefault();
    });
    $(document).on('spin', '.control-spinner', function (event, ui) {
        var $control = $(this);
        var data = $p.getData($control);
        data[this.id] = ui.value;
        $p.setGridTimestamp($control, data);
    });
    $(document).on('change', '.control-checkbox.visible', function () {
        show(this.id.substring(7, this.id.length), $(this).prop('checked'));

        function show(selector, value) {
            if (value) {
                $(selector).show();
            }
            else {
                $(selector).hide();
            }
        }
    });
    $(document).on('change', '.auto-postback:not([type="text"],select[multiple])', function () {
        $p.send($(this));
    });
    $(document).on('change', '.datepicker.auto-postback', function () {
        $p.send($(this));
    });
    $(document).on('keyup', '.auto-postback[type="text"]', function (e) {
        var $control = $(this);
        $p.setData($control);
        if (e.keyCode === 13) {
            $p.debounce(function () {
                $p.send($control);
                delete $p.getData($control)[$control.attr('id')];
            }, 500);
        }
    });
});
$(function () {
    $(document).on('change', '#AggregationType', function () {
        if ($(this).val() === 'Count') {
            $('#AggregationTarget').closest('.togglable').hide();
        } else {
            $('#AggregationTarget').closest('.togglable').show();
        }
    });
    $(document).on('click', '#Aggregations .data.link', function () {
        var $control = $($(this).attr('data-selector'));
        if ($control.length === 1) {
            var value = $(this).attr('data-value');
            $control.multiselect('widget').find(':checkbox').each(function () {
                if ($(this).val() === value) {
                    $(this).click();
                    return;
                }
            });
        }
    });
    $(document).on('click', '#Aggregations .overdue', function () {
        $('#ViewFilters_Overdue').click();
    });
});
$(function () {
    $(document).on('click', 'a', function (e) {
        e.stopPropagation();
    });
});
$p.uploadAttachments = function ($control, files) {
    var columnName = $control.attr('data-name');
    var controlId = $control.parent().find('.control-attachments').attr('id');
    var url = $('.main-form').attr('action').replace('_action_', $control.attr('data-action'));
    var data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append('file', files[i]);
    }
    data.append('ControlId', controlId);
    data.append('ColumnName', columnName);
    data.append('AttachmentsData', $('#' + controlId).val());
    var $status = $('[id="' + columnName + '.status"]');
    var statusBar = new createStatusbar(
        $status,
        $('[id="' + columnName + '.progress"]'),
        $('[id="' + columnName + '.abort"]'));
    $status.show();
    $p.multiUpload(url, data, $control, statusBar);

    function createStatusbar(status, progressBar, abort) {
        this.progressBar = progressBar;
        this.abort = abort;
        this.status = status;
        this.setProgress = function (progress) {
            var progressBarWidth = progress * this.progressBar.width() / 100;
            this.progressBar.find('div').animate({ width: progressBarWidth }, 10);
        }
        this.setAbort = function (uploader) {
            var sb = this.status;
            this.abort.click(function () {
                uploader.abort();
                sb.hide();
            });
        }
    }
}

$p.deleteAttachment = function ($control, $data) {
    var json = JSON.parse($control.val());
    json.filter(function (item, index, array) {
        if (item.Added === true) {
            if (item.Guid.toString() === $data.attr('data-id')) {
                var data = {};
                data.Guid = item.Guid;
                url = $('.main-form')
                    .attr('action')
                    .replace('_action_', $data.attr('data-action'));
                $p.ajax(url, 'post', data);
                $('#' + item.Guid).remove();
                array.splice(index, 1);
            }
        } else {
            if (item.Guid === $data.attr('data-id')) {
                if (!item.Deleted) {
                    item.Deleted = true;
                    $data.parent().addClass('preparation-delete');
                    $data.removeClass('ui-icon-circle-close');
                    $data.addClass('ui-icon-trash');

                } else {
                    item.Deleted = false;
                    $data.parent().removeClass('preparation-delete');
                    $data.removeClass('ui-icon-trash');
                    $data.addClass('ui-icon-circle-close');
                }
            }
        }
    });
    json = JSON.stringify(json);
    $control.val(json);
    $p.setData($control);
}
$(document).on('dragenter', '.control-attachments-upload', function (e) {
    e.stopPropagation();
    e.preventDefault();
});

$(document).on('dragover', '.control-attachments-upload', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).css('border', '2px solid #d19405');
});

$(document).on('drop', '.control-attachments-upload', function (e) {
    var $control = $(this);
    $control.css('border', '2px dotted #d19405');
    e.preventDefault();
    var files = e.originalEvent.dataTransfer.files;
    $p.uploadAttachments($control, files);
});

$(document).on('click', '.control-attachments-upload', function (e) {
    var $control = $(this);
    var input = document.getElementById($control.attr('data-name') + '.input');
    input.click();
});

$(document).on('change', '.control-attachments-upload', function (e) {
    var $control = $(this);
    var input = document.getElementById($control.attr('data-name') + '.input');
    if (input.files.length == 0) return;
    $p.uploadAttachments($control, input.files);
});

$(document).on('dragenter', function (e) {
    e.stopPropagation();
    e.preventDefault();
});

$(document).on('dragover', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).css('border', '2px solid #d19405');
});

$(document).on('drop', function (e) {
    e.stopPropagation();
    e.preventDefault();
});
$p.addBasket = function ($control, text, value) {
    var $li = $('<li/>');
    if (value !== undefined){
        $li.attr('data-value', value);
    }
    $li
        .addClass('ui-widget-content ui-selectee')
        .append($('<span/>').text(text))
        .append($('<span/>')
            .addClass('ui-icon ui-icon-close delete'));
    $control.append($li);
    $p.setData($control);
}
$(function () {
    $(document).on('click', '.control-basket > li > .delete', function () {
        var $control = $(this).closest('ol');
        $(this).parent().remove();
        $p.setData($control);
    });
});
$p.openBulkUpdateSelectorDialog = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#BulkUpdateSelectorDialog').dialog({
            modal: true,
            width: '520px',
            resizable: false
        });
    }
}

$p.bulkUpdate = function () {
    var maindata = $p.getData($('.main-form'));
    var data = $p.getData($('#BulkUpdateSelectorForm'));
    var key = $('#ReferenceType').val() + '_' + $('#BulkUpdateColumnName').val();
    data.GridCheckAll = maindata.GridCheckAll;
    data.GridUnCheckedItems = maindata.GridUnCheckedItems;
    data.GridCheckedItems = maindata.GridCheckedItems;
    $p.setData($('#' + key));
    $p.send($('#BulkUpdate'));
}
$p.drawBurnDown = function () {
    var $svg = $('#BurnDown');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#BurnDownJson').val());
    if (json.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#BurnDown');
    var padding = 40;
    var axisPadding = 70;
    var width = parseInt(svg.style('width'));
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPadding - (padding);
    var bodyHeight = height - axisPadding - (padding);
    var minDate = new Date(d3.min(json, function (d) { return d.Day; }));
    var maxDate = new Date(d3.max(json, function (d) { return d.Day; }));
    var dayWidth = (bodyWidth - padding) / $p.dateDiff('d', maxDate, minDate);
    var xScale = d3.scaleTime()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scaleLinear()
        .domain([d3.max(json, function (d) {
            return d.Total !== undefined || d.Earned !== undefined
                ? Math.max.apply(null, [d.Total, d.Planned, d.Earned])
                : d.Planned;
        }), 0])
        .range([padding, bodyHeight])
        .nice();
    var xAxis = d3.axisBottom(xScale)
        .tickFormat(d3.timeFormat('%m/%d'))
        .tickSizeInner(10);
    var yAxis = d3.axisLeft(yScale)
        .tickSizeInner(0)
        .tickFormat("");
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPadding + ', ' + (height - axisPadding) + ')')
        .call(xAxis)
        .selectAll('text')
        .attr('x', -20)
        .attr('y', 20)
        .style('text-anchor', 'start');
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPadding + ', 0)')
        .call(yAxis)
        .selectAll('text')
        .attr('x', -20);
    var now = axisPadding + xScale(new $p.shortDate());
    var nowLineData = [
        [now, axisPadding - 40],
        [now, yScale(0) + 20]];
    var nowLine = d3.line()
        .x(function (d) { return d[0]; })
        .y(function (d) { return d[1]; });
    svg.append('g').attr('class', 'now').append('path').attr('d', nowLine(nowLineData));
    draw('total', 0, json.filter(function (d) { return d.Total !== undefined; }));
    draw('planned', 1, json.filter(function (d) { return d.Planned !== undefined; }));
    draw('earned', 2, json.filter(function (d) { return d.Earned !== undefined; }));

    function draw(css, n, ds) {
        var line = d3.line()
            .x(function (d) {
                return ($p.dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPadding + padding;
            })
            .y(function (d) {
                return yScale(prop(d)) - 100;
            });
        var g = svg.append('g').attr('class', css);
        g.append('path').attr('d', line(ds));
        g.selectAll('circle')
            .data(ds)
            .enter()
            .append('circle')
            .attr('cx', function (d, i) { return i * dayWidth + axisPadding + padding })
            .attr('cy', function (d) { return yScale(prop(d)) - 100; })
            .attr('r', 4)
            .append('title')
            .text(function (d) { return prop(d); });

        function prop(d) {
            switch (n) {
                case 0: return d.Total;
                case 1: return d.Planned;
                case 2: return d.Earned;
            }
        }
    }
}
$(function () {
    $(document).on('click', '#BurnDownDetails > tbody > tr', function () {
        var $control = $(this);
        if (!$control.next().hasClass('items')) {
            var data = $p.getData($control);
            data.BurnDownDate = $control.attr('data-date');
            data.BurnDownColspan = $control.find('td').length;
            $p.send($control);
        } else {
            $control.next().remove();
        }
    });
    $(document).on('click', '#BurnDownDetails .items', function () {
        $(this).remove();
    });
});
$p.moveCalendar = function (type) {
    var $control = $('#CalendarMonth');
    $control.val($('#Calendar' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCalendar = function () {
    $('#Calendar .container > div > div:not(.day)').remove();
    var data = JSON.parse($('#CalendarJson').val());
    var hash = {};
    var begin = new Date($('#Calendar .container:first').attr('data-id'));
    var end = new Date($('#Calendar .container:last').attr('data-id'));
    switch ($('#CalendarTimePeriod').val()) {
        case 'Yearly':
            setYearly(data, hash, begin, end);
            break;
        case 'Monthly':
            setMonthly(data, hash, begin, end);
            break;
    }

    function setYearly(data, hash, begin, end) {
        data.forEach(function (element) {
            var current = $p.beginningMonth(new Date(element.From))
            if (current < begin) {
                current = new Date(begin);
            }
            rank = Rank(hash, $p.shortDateString(current));
            addItem(
                hash,
                element,
                current,
                undefined,
                undefined,
                1);
            if (element.To !== undefined) {
                current.setMonth(current.getMonth() + 1);
                var to = new Date(element.To);
                if (to > end) {
                    to = end;
                }
                while ($p.shortDate(to) >= $p.shortDate(current)) {
                    addItem(
                        hash,
                        element,
                        current,
                        1,
                        rank);
                    current.setMonth(current.getMonth() + 1);
                }
            }
        });
    }

    function setMonthly(data, hash, begin, end) {
        data.forEach(function (element) {
            var current = new Date(element.From);
            if (current < begin) {
                current = new Date(begin);
            }
            rank = Rank(hash, $p.shortDateString(current));
            addItem(
                hash,
                element,
                current);
            if (element.To !== undefined) {
                current.setDate(current.getDate() + 1);
                var to = new Date(element.To);
                if (to > end) {
                    to = end;
                }
                while ($p.shortDate(to) >= $p.shortDate(current)) {
                    if (current.getDay() === 1) {
                        rank = Rank(hash, $p.shortDateString(current));
                    }
                    addItem(
                        hash,
                        element,
                        current,
                        current.getDay() !== 1,
                        rank);
                    current.setDate(current.getDate() + 1);
                }
            }
        });
        if ($('#CalendarCanUpdate').val() === '1') {
            $('#Calendar .item').draggable({
                revert: 'invalid',
                start: function () {
                    $(this).parent().droppable({
                        disabled: true
                    });
                },
                helper: function () {
                    return $('<div />')
                        .addClass('dragging')
                        .append($('<div />')
                            .append($(this).text()));
                }
            });
            $('#Calendar .container').droppable({
                hoverClass: 'hover',
                tolerance: 'intersect',
                drop: function (e, ui) {
                    var $control = $(ui.draggable);
                    var from = new Date($control.attr('data-from'));
                    var target = new Date($(this).attr('data-id'));
                    var data = $p.getData($('.main-form'));
                    var fromTo = $('#CalendarFromTo').val().split('-');
                    var prefix = $('#TableName').val() + '_';
                    data.Id = $control.attr('data-id');
                    data[prefix + fromTo[0]] = margeTime(target, from);
                    if ($control.attr('data-to') !== undefined) {
                        var diff = $p.dateDiff('d', target, $p.shortDate(from));
                        var to = $p.dateAdd('d', diff, new Date($control.attr('data-to')));
                        data[prefix + fromTo[1]] = margeTime(to);
                    }
                    $p.saveScroll();
                    $p.send($('#CalendarBody'));
                }
            });
        }
    }

    function Rank(hash, id) {
        if (hash[id] === undefined) {
            hash[id] = 0;
        }
        return hash[id];
    }

    function addItem(hash, element, current, sub, rank, yearly) {
        var id = $p.shortDateString(current);
        var $cell = $('[data-id="' + id + '"] > div');
        while (Rank(hash, id) < rank) {
            $cell.append($('<div />').addClass('dummy'));
            hash[id]++;
        }
        var item = $('<div />')
            .addClass('item')
            .addClass(element.Changed === true ? 'changed' : '')
            .attr('data-id', element.Id)
            .attr('data-from', element.From)
            .attr('data-to', element.To);
        if (sub) {
            item.append($('<div />')
                .attr('data-id', element.Id)
                .addClass('connection')
                .addClass(element.Changed === true
                    ? 'changed'
                    : ''));
        }
        item.append($('<div />')
            .addClass('title')
            .css('width', function () {
                var width = $cell.parent().width();
                var margin = 16;
                if (sub) {
                    return '';
                }
                else if (element.To === undefined) {
                    return width - margin;
                }
                else if (yearly) {
                    var diff = 0;
                    var month = new Date(current);
                    month.setMonth(month.getMonth() + 1);
                    while (month <= new Date(element.To)) {
                        diff++;
                        month.setMonth(month.getMonth() + 1);
                    }
                    return (width * (diff + 1)) - margin;
                } else {
                    var diff = $p.dateDiff(
                        'd',
                        $p.shortDate(new Date(element.To)),
                        $p.shortDate(current));
                    var col = current.getDay() !== 0
                        ? current.getDay()
                        : 7;
                    if (col + diff > 6) {
                        diff = (7 - col);
                    } else if (diff < 0) {
                        diff = 0;
                    }
                    return (width * (diff + 1)) - margin;
                }
            })
            .addClass(sub ? 'sub' : '')
            .attr('title', element.Title + ' -- ' +
                $p.dateTimeString(new Date(element.From)) +
                    (element.To !== undefined && element.To !== element.From
                        ? ' - ' + $p.dateTimeString(new Date(element.To))
                        : ''))
            .append($('<span />').addClass('ui-icon ui-icon-pencil'))
            .append((element.Time !== undefined
                ? element.Time + ' '
                : '') +
                    element.Title));
        $cell.append(item);
        hash[id]++;
    }

    function margeTime(date, dateTime) {
        if (dateTime === undefined) dateTime = date;
        return date.getFullYear() + '/' +
            (date.getMonth() + 1) + '/' +
            date.getDate() + ' ' +
            dateTime.getHours() + ':' +
            dateTime.getMinutes() + ':' +
            dateTime.getSeconds();
    }
}
$(function () {
    $(document).on('dblclick', '#Calendar .item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
    $(document).on('click', '#Calendar .item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().parent().attr('data-id');
    });
    $(document).on('mouseenter', '#Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').addClass('hover');
    });
    $(document).on('mouseleave', '#Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').removeClass('hover');
    });
    $(window).on('resize', function () {
        if ($('#Calendar').length === 1) {
            setTimeout(function () {
                $p.saveScroll();
                $p.setCalendar('#Grid');
                $p.loadScroll();
            }, 10);
        }
    });
    $(document).on('dblclick', '#Calendar .ui-droppable', function (event) {
        var addDate = function (baseDate, add) {
            if (add === '') return '';
            var date = new Date(baseDate.getTime());
            date.setDate(date.getDate() + parseInt(add, 10));
            return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
        }
        var addInput = function (form, name, value) {
            if (!name) return;
            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', $('#ReferenceType').val() + '_' + name);
            input.setAttribute('value', value);
            form.appendChild(input);
        }
        if ($(event.target).is('.title')) return;
        var baseDate = new Date($(this).attr('data-id'));
        var names = $('#CalendarFromTo').val().split('-');
        var form = document.createElement("form");
        form.setAttribute("action", $('.ui-icon-plus').parent().attr('href'));
        form.setAttribute("method", "post");
        form.style.display = "none";
        document.body.appendChild(form);
        addInput(form, names[0], addDate(baseDate, $('#CalendarFromDefaultInput').val()));
        addInput(form, names[1], addDate(baseDate, $('#CalendarToDefaultInput').val()));
        form.submit();
    });
    $(document).on('click', '.calendar-to-monthly', function () {
        var data = {
            'CalendarTimePeriod': 'Monthly',
            'CalendarMonth': $(this).attr('data-id')
        };
        $p.ajax(location.href, 'post', data);
    });
});
$p.copyDirectUrlToClipboard = function (url) {
    var div = $('<div>');
    var pre = $('<pre>');
    div.append(pre);
    pre.text(url);
    div.css({ 'position': 'fixed', 'left': '-100%', 'top': '-100%' });
    $(document.body).append(div);
    document.getSelection().selectAllChildren(div.get(0));
    document.execCommand('copy');
    div.remove();
    alert($p.display('DirectUrlCopied'));
}
$p.openColumnAccessControlDialog = function ($control) {
    $p.data.ColumnAccessControlForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#ColumnAccessControlDialog').dialog({
            modal: true,
            width: '900px',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.addColumnAccessControl = function () {
    $('#SourceColumnAccessControl li.ui-selected').appendTo('#CurrentColumnAccessControl');
    $p.setData($('#CurrentColumnAccessControl'));
}

$p.deleteColumnAccessControl = function () {
    $('#CurrentColumnAccessControl li.ui-selected').appendTo('#SourceColumnAccessControl');
}

$p.changeColumnAccessControl = function ($control, type) {
    $p.setData($('#' + type + 'ColumnAccessControl'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.ColumnAccessControl = mainFormData[type + 'ColumnAccessControl'];
    data.ColumnAccessControlAll = mainFormData[type + 'ColumnAccessControlAll'];
    data.ColumnAccessControlType = type;
    $p.send($control);
}
$p.confirmReload = function confirmReload() {
    if ($p.formChanged) {
        return confirm($p.display('ConfirmUnload'));
    } else {
        return true;
    }
}
$(function () {
    $(document).on(
        'change',
        '.confirm-unload input, .confirm-unload select, .confirm-unload textarea,  .confirm-unload .control-spinner',
        function () {
            $p.setFormChanged($(this));
        });
    $(document).on(
        'spin',
        '.confirm-unload .control-spinner',
        function () {
            $p.setFormChanged($(this));
    });
    $(window).bind("beforeunload", function () {
        if ($p.formChanged) {
            return $p.display('ConfirmUnload');
        }
    });
});
$p.moveCrosstab = function (type) {
    var $control = $('#CrosstabMonth');
    $control.val($('#Crosstab' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCrosstab = function () {
    $('#CrosstabColumnsField').toggle($('#CrosstabGroupByY').val() === 'Columns');
    $('#CrosstabValueField').toggle(
        $('#CrosstabGroupByY').val() !== 'Columns' &&
        $('#CrosstabAggregateType').val() !== 'Count');
    var date = $('#CrosstabXType').val() === 'datetime';
    $('#CrosstabTimePeriodField').toggle(date);
    $('#CrosstabMonth').toggle(date);
    $('#CrosstabPreviousButton').toggle(date);
    $('#CrosstabNextButton').toggle(date);
    $('#CrosstabThisMonthButton').toggle(date);
}
$p.openDialog = function ($control, appendTo) {
    $($control.attr('data-selector')).dialog({
        modal: true,
        width: '420px',
        appendTo: appendTo,
        resizable: false
    });
}

$p.closeDialog = function ($control) {
    $p.clearMessage();
    $control.closest('.ui-dialog-content').dialog('close');
}

$p.clearDialogs = function () {
    $('body > .ui-dialog').remove();
}
$(function () {
    $(document).on('click', '.ui-widget-overlay', function () {
        $p.clearMessage();
        $('.ui-dialog-content:visible').dialog('close');
    });
});
$p.display = function (defaultId) {
    var displays = {
        CanNotConnectCamera: 'Can not connect to the camera.',
        CanNotConnectCamera_ja: 'カメラに接続できません。',
        CheckAll: 'Check all',
        CheckAll_ja: '全て選択',
        ConfirmCreateLink: 'Would you like to create a link ?',
        ConfirmCreateLink_ja: 'リンクを作成しますか？',
        ConfirmDelete: 'Are you sure you want to delete ?',
        ConfirmDelete_ja: '本当に削除してもよろしいですか？',
        ConfirmPhysicalDelete: 'Are you sure you want to delete ?',
        ConfirmPhysicalDelete_ja: 'この操作を行うと元に戻すことはできません。本当に削除してもよろしいですか？',
        ConfirmRebuildSearchIndex: 'Would you like to rebuild the search index ?',
        ConfirmRebuildSearchIndex_ja: '検索インデックスを再構築しますか？',
        ConfirmReset: 'Are you sure you want to reset ?',
        ConfirmReset_ja: '本当にリセットしてもよろしいですか？',
        ConfirmRestore: 'Are you sure you want to restore ?',
        ConfirmRestore_ja: '本当に復元してもよろしいですか？',
        ConfirmSendMail: 'Are you sure you want to send an email ?',
        ConfirmSendMail_ja: 'メールを送信してもよろしいですか？',
        ConfirmSeparate: 'Are you sure you want to separate ?',
        ConfirmSeparate_ja: '分割してもよろしいですか？',
        ConfirmSwitchUser: 'Are you sure you want to switch users?',
        ConfirmSwitchUser_ja: '本当にユーザを切り替えますか？',
        ConfirmSynchronize: 'Are you sure you want to synchronize the data ?',
        ConfirmSynchronize_ja: 'データを同期してもよろしいですか？',
        ConfirmUnload: 'Are you sure you want to unload this page?',
        ConfirmUnload_ja: 'このページを離れますか? 行った変更が保存されない可能性があります。',
        DirectUrlCopied: 'Copied to Clipboard',
        DirectUrlCopied_ja: 'クリップボードにコピーしました',
        IncludeData: 'Include data',
        IncludeData_ja: 'データを含める',
        Manager: 'Manager',
        Manager_ja: '管理者',
        OrderAsc: 'Asc',
        OrderAsc_ja: '昇順',
        OrderDesc: 'Desc',
        OrderDesc_ja: '降順',
        OrderRelease: 'Release',
        OrderRelease_ja: '解除',
        ResetOrder: 'Reset',
        ResetOrder_ja: 'リセット',
        UncheckAll: 'Uncheck all',
        UncheckAll_ja: '全て解除',
        ValidateDate: 'This is an invalid date.',
        ValidateDate_ja: '日付または時刻が不正です。',
        ValidateEmail: 'Doesn’t look like a valid email.',
        ValidateEmail_ja: 'メールアドレスの形式が不正です。',
        ValidateEqualTo: 'Please enter the same value again.',
        ValidateEqualTo_ja: '入力した文字列が一致しません。',
        ValidateMaxLength: 'Entered is too long.',
        ValidateMaxLength_ja: '入力した文字が長すぎます。',
        ValidateMaxNumber: 'The number entered is too large.',
        ValidateMaxNumber_ja: '入力された数字が大きすぎます。',
        ValidateMinNumber: 'The number entered is too small.',
        ValidateMinNumber_ja: '入力された数字が小さすぎます。',
        ValidateNumber: 'You can not enter a non-numeric.',
        ValidateNumber_ja: '数値以外入力不可です。',
        ValidateRequired: 'This information is required.',
        ValidateRequired_ja: '入力必須項目です。',
        ValidationError: 'There is an error in the entered content.',
        ValidationError_ja: '入力された内容に誤りがあります。'
    };
    var localId = defaultId + '_' + $('#Language').val();
    if (displays[localId]) {
        return displays[localId];
    } else if (displays[defaultId]) {
        return displays[defaultId];
    } else {
        return defaultId;
    }
}

$p.setDropDownSearch = function () {
    var $control = $('[id="' + $('#DropDownSearchTarget').val() + '"]');
    if ($control.attr('multiple') === 'multiple') {
        $control.multiselect('refresh');
    }
    $p.setData($control);
    if ($control.hasClass('auto-postback')) {
        $p.send($control);
    }
    if ($control.val() !== '' && $control.hasClass('error')) {
        $control.removeClass('error');
        $('[id="' + $control.attr('id') + '-error"]').remove();
    }
}

$p.openDropDownSearchDialog = function ($control) {
    var id = $control.attr('id');
    var $text = $('#DropDownSearchText');
    var $target = $('#DropDownSearchTarget');
    $('#DropDownSearchParentClass').val($("#" + id).attr('parent-data-class'));
    $('#DropDownSearchParentDataId').val($("#" + id).attr('parent-data-id'));
    $('#DropDownSearchResults').empty();
    $target.val(id);
    $text.val('');
    $('#DropDownSearchMultiple').val($control.attr('multiple') === 'multiple');
    $($('#DropDownSearchDialog')).dialog({
        title: $('label[for="' + id + '"]').text(),
        modal: true,
        width: '630px',
        resizable: false,
        close: function () {
            $('#' + $target.val()).prop("disabled", false);
        }
    });
    $p.send($text);
    $p.setPaging('DropDownSearchResults');
    $control.prop("disabled", true);
    $text.focus();
}
$(function () {
    $(document).on('focusin', '.control-dropdown.search', function () {
        if ($('#EditorLoading').val() === '1') {
            $(this).blur();
            $('#EditorLoading').val(0);
        } else {
            $p.openDropDownSearchDialog($(this));
        }
    });
});
$p.openExportSelectorDialog = function ($control) {
    error = $p.send($control);
    if (error === 0) {
        $('#ExportSelectorDialog').dialog({
            modal: true,
            width: '420px',
            resizable: false
        });
    }
}

$p.export = function () {
    var data = $p.getData($('.main-form'));
    var exp = JSON.parse($('#ExportId').val());
    if (exp.mailNotify === true) {
        data["ExportId"] = exp.id;
        $p.send($("#DoExport"),"MainForm");
    } else {
        location.href = $('.main-form').attr('action').replace('_action_', 'export')
            + '?id=' + exp.id
            + '&GridCheckAll=' + data.GridCheckAll
            + '&GridUnCheckedItems=' + data.GridUnCheckedItems
            + '&GridCheckedItems=' + data.GridCheckedItems;
    }
    $p.closeDialog($('#ExportSelectorDialog'));
}

$p.exportCrosstab = function () {
    location.href = $('.main-form').attr('action').replace('_action_', 'exportcrosstab');
}
$(function () {
    $(document).on('click', '#ViewFilters_Reset', function () {
        $('[id^="ViewFilters_"]').each(function () {
            $p.clear($(this));
        });
        $p.send($(this));
    });
});
$p.focusMainForm = function () {
    $('#FieldSetGeneral').find('[class^="control-"]').each(function () {
        if (!$(this).is(':hidden') &&
            !$(this).hasClass('control-text') &&
            !$(this).hasClass('control-markup')) {
            $(this).focus();
            return false;
        }
    });
}
$(function () {
    var $control = $('.control-textbox.focus');
    if ($control.length !== 0) {
        setTimeout(function () {
            $control.focus();
        }, 0);
    }
});
$p.moveGantt = function (type) {
    var $control = $('#GanttStartDate');
    var value = $('#Gantt' + type).val();
    $control.val(value);
    $control.attr('data-previous', value);
    $p.getData($control).GanttStartDate = value;
    $p.send($control);
}

$p.drawGantt = function () {
    var $gantt = $('#Gantt');
    var $axis = $('#GanttAxis');
    if ($gantt.length !== 1) {
        return;
    }
    $gantt.empty();
    $axis.empty();
    var json = JSON.parse($('#GanttJson').val());
    if (json.length === 0) {
        $gantt.hide();
        return;
    }
    $gantt.show();
    var justTime = new Date();
    var axis = d3.select('#GanttAxis');
    var svg = d3.select('#Gantt');
    var padding = 20;
    var width = parseInt(svg.style('width'));
    var minDate = new Date($('#GanttMinDate').val());
    var maxDate = new Date($('#GanttMaxDate').val());
    var xScale = d3.scaleTime()
        .domain([minDate, maxDate])
        .range([0, width - 60]);
    var xHarf = xScale(maxDate) / 2;
    var months = [];
    var currentMonth;
    var days = [];
    for (var s = 0; s < $p.dateDiff('d', maxDate, minDate); s++) {
        var d = $p.dateAdd('d', s, minDate);
        days.push(d);
        if (currentMonth !== d.getMonth()) {
            currentMonth = d.getMonth();
            months.push(d);
        }
    }
    axis.append('g')
        .selectAll('rect')
        .data(days)
        .enter()
        .append('rect')
        .attr('x', function (d) { return 30 + xScale(d) })
        .attr('y', 25)
        .attr('width', xScale(days[1]))
        .attr('height', 20)
        .attr('class', function (d) {
            switch (d.getDay()) {
                case 0: return 'sunday';
                case 6: return 'saturday';
                default: return 'weekday';
            }
        });
    var currentDate = minDate;
    while (currentDate <= maxDate) {
        var axisLine = [[30 + xScale(currentDate), 25], [30 + xScale(currentDate), 45]];
        var line = d3.line()
            .x(function (d) { return d[0]; })
            .y(function (d) { return d[1]; });
        axis.append('g').attr('class', 'date').append('path').attr('d', line(axisLine));
        currentDate = $p.dateAdd('d', 1, currentDate);
    }
    axis.append('g')
        .attr('class', 'title')
        .selectAll('text')
        .data(months)
        .enter()
        .append('text')
        .attr('text-anchor', 'middle')
        .attr('x', function (d) {
            return 30 + xScale(d) + (xScale($p.dateAdd('d', 1, d)) - xScale(d)) / 2;
        })
        .attr('y', 20)
        .text(function (d) {
            return d.getMonth() + 1;
        });
    axis.append('g')
        .attr('class', 'title')
        .selectAll('text')
        .data(days.filter(function (d) {
            return days.length <= 90 || [5, 10, 15, 20, 25, 30].indexOf(d.getDate()) > -1;
        }))
        .enter()
        .append('text')
        .attr('text-anchor', 'middle')
        .attr('x', function (d) {
            return 30 + xScale(d) + (xScale($p.dateAdd('d', 1, d)) - xScale(d)) / 2;
        })
        .attr('y', 40)
        .text(function (d) {
            return d.getDate();
        });
    var now = padding + xScale(justTime);
    var groupCount = json.filter(function (d) { return d.GroupSummary }).length === 0
        ? 0
        : -1;
    $.each(json, function (i, d) {
        if (d.GroupSummary) groupCount++;
        d.Y = padding + i * 25 + groupCount * 25;
    });
    $('#Gantt').css('height', d3.max(json, function (d) { return d.Y }) + 45);
    svg.append('g')
        .selectAll('rect')
        .data(days.filter(function (d) {
            switch (d.getDay()) {
                case 0: return true;
                case 6: return true;
                default: return false;
            }
        }))
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(d) })
        .attr('y', padding - 10)
        .attr('width', xScale(days[1]))
        .attr('height', (padding + d3.max(json, function (d) { return d.Y })))
        .attr('class', function (d) {
            switch (d.getDay()) {
                case 0: return 'sunday';
                case 6: return 'saturday';
                default: return null;
            }
        });
    currentDate = minDate;
    while (currentDate <= maxDate) {
        draw(padding + xScale(currentDate), 'date');
        currentDate = $p.dateAdd('d', 1, currentDate);
    }
    svg.append('g').attr('class', 'planned')
        .selectAll('rect')
        .data(json)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime))
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.Completed
                ? 'completed'
                : '';
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    svg.append('g').attr('class', 'earned')
        .selectAll('rect')
        .data(json)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                    ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                        * d.ProgressRate * 0.01)) < now
                ? 'delay'
                : d.ProgressRate === 100 && d.Completed
                    ? 'completed'
                    : ''
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    draw(now, 'now');
    svg.append('g').attr('class', 'title')
        .selectAll('text')
        .data(json)
        .enter()
        .append('text')
        .attr('x', function (d) {
            return xScale(new Date(d.StartTime)) < 0
                ? padding + 5
                : padding + xScale(new Date(d.StartTime)) + 5
        })
        .attr('y', function (d) {
            return d.Y + 16;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                    ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                        * d.ProgressRate * 0.01)) < now &&
                ($('#ShowGanttProgressRate').val() === '1' || !d.Completed)
                ? 'delay'
                : '';
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('text-anchor', function (d) {
            return 'start';
        })
        .attr('data-id', function (d) { return d.Id; })
        .text(function (d) {
            return d.Title;
        })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime + ' : ' + d.Title;
        });

    function draw(day, css) {
        var nowLineData = [
            [day, padding - 10],
            [day, (padding + d3.max(json, function (d) { return d.Y })) + 10]];
        var nowLine = d3.line()
            .x(function (d) { return d[0]; })
            .y(function (d) { return d[1]; });
        svg.append('g').attr('class', css).append('path').attr('d', nowLine(nowLineData));
    }
}
$(function () {
    $(document).on('click', '#Gantt .planned rect,#Gantt .earned rect,#Gantt .title text', function () {
        if ($(this).filter('.summary').length === 0) {
            location.href = $('#BaseUrl').val() + $(this).attr('data-id');
        }
    });
});
$p.setGrid = function () {
    $p.paging('#Grid');
}

$p.openEditorDialog = function (id) {
    $p.data.DialogEditorForm = {};
    $('#EditorDialog').dialog({
        modal: true,
        width: '90%',
        resizable: false,
        open: function () {
            $('#EditorLoading').val(0);
            $p.initRelatingColumn();
        }
    });
}

$p.editOnGrid = function ($control, val) {
    if (val === 0) {
        if (!$p.confirmReload()) return false;
    }
    $('#EditOnGrid').val(val);
    $p.send($control);
}

$p.newOnGrid = function ($control) {
    $p.send($control, 'MainForm');
    $(window).scrollTop(0);
}

$p.copyRow = function ($control) {
    $p.getData($control).OriginalId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
    $(window).scrollTop(0);
}

$p.cancelNewRow = function ($control) {
    $p.getData($control).CancelRowId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
}

$(function () {
    if ($("#Grid").length == 0) return;
    var TBLHD = TBLHD || {};
    TBLHD.$clone = null;
    TBLHD.view = 0;
    TBLHD.StyleCopy = function ($copyTo, $copyFrom) {
        $copyTo.css("width",
            $copyFrom.css("width"));
        $copyTo.css("height",
            $copyFrom.css("height"));
        $copyTo.css("padding-top",
            $copyFrom.css("padding-top"));
        $copyTo.css("padding-left",
            $copyFrom.css("padding-left"));
        $copyTo.css("padding-bottom",
            $copyFrom.css("padding-bottom"));
        $copyTo.css("padding-right",
            $copyFrom.css("padding-right"));
        $copyTo.css("background",
            $copyFrom.css("background"));
        $copyTo.css("background-color",
            $copyFrom.css("background-color"));
        $copyTo.css("vertical-align",
            $copyFrom.css("vertical-align"));
        $copyTo.css("border-top-width",
            $copyFrom.css("border-top-width"));
        $copyTo.css("border-top-color",
            $copyFrom.css("border-top-color"));
        $copyTo.css("border-top-style",
            $copyFrom.css("border-top-style"));
        $copyTo.css("border-left-width",
            $copyFrom.css("border-left-width"));
        $copyTo.css("border-left-color",
            $copyFrom.css("border-left-color"));
        $copyTo.css("border-left-style",
            $copyFrom.css("border-left-style"));
        $copyTo.css("border-right-width",
            $copyFrom.css("border-right-width"));
        $copyTo.css("border-right-color",
            $copyFrom.css("border-right-color"));
        $copyTo.css("border-right-style",
            $copyFrom.css("border-right-style"));
        $copyTo.css("border-bottom-width",
            $copyFrom.css("border-bottom-width"));
        $copyTo.css("border-bottom-color",
            $copyFrom.css("border-bottom-color"));
        $copyTo.css("border-bottom-style",
            $copyFrom.css("border-bottom-style"));
    }
    TBLHD.cssCopy = function ($clone, $thead) {
        TBLHD.StyleCopy($clone, $thead);
        for (var i = 0; i < $thead.children("tr").length; i++) {
            var $theadtr = $thead.children("tr").eq(i);
            var $clonetr = $clone.children("tr").eq(i);
            for (var j = 0; j < $theadtr.eq(i).children("th").length; j++) {
                var $theadth = $theadtr.eq(i).children("th").eq(j);
                var $cloneth = $clonetr.eq(i).children("th").eq(j);
                TBLHD.StyleCopy($cloneth, $theadth);
            }
        }
    }
    TBLHD.createClone = function () {
        if (TBLHD.$clone != null) {
            TBLHD.$clone.remove();
        }
        TBLHD.$table = $("#Grid");
        TBLHD.$thead = TBLHD.$table.children("thead");
        TBLHD.$table.children("thead:empty").remove();
        TBLHD.$table.children("tbody:empty").remove();
        TBLHD.$clone = TBLHD.$thead.clone(true);
        let events = $._data($(document).get(0), "events");
        let evs = {};
        for (let p in events) {
            let $es = $(events).prop(p);
            for (let ind in $es) {
                let $e = $es[ind];
                if (String($e.selector).indexOf('#') === 0) {
                    evs[String($e.selector).slice(1)] = String($e.type);
                }
            }
        }
        let $ids = TBLHD.$clone.find('[id]');
        for (var i = 0; i < $ids.length; i++) {
            let $i = $($ids[i]);
            let id = $i.attr('id');
            if (evs[id]) {
                $i.on(evs[id], function () {
                    $('#' + id).trigger(evs[id]);
                });
            }
        }
        TBLHD.$clone.appendTo("body");
        if (TBLHD.view == 0) {
            TBLHD.$clone.css("display", "none");
        } else {
            TBLHD.$clone.css("display", "table");
        }
        TBLHD.$clone.css("position", "fixed");
        TBLHD.$clone.css("border-collapse", "collapse");
        TBLHD.$clone.css("top", "0px");
        TBLHD.$clone.css("z-index", 99);
        TBLHD.$clone.css("left", TBLHD.$table.offset().left - $(window).scrollLeft());
        TBLHD.cssCopy(TBLHD.$clone, TBLHD.$thead);
    }
    TBLHD.observer = new MutationObserver(function (records) {
        if (records.some(function (value) {
            return value.target.className === 'menu-sort';
        })) { return;}
        TBLHD.createClone();
    });
    TBLHD.observer.observe(
        document.getElementById("ViewModeContainer"),
        {
            attributes: true,
            attributeOldValue: true,
            characterData: true,
            characterDataOldValue: true,
            childList: true,
            subtree: true
        }
    );
    TBLHD.createClone();
    $(window).resize(function () {
        if (TBLHD.$clone != null) {
            TBLHD.cssCopy(TBLHD.$clone, TBLHD.$thead);
        }
    });
    $(window).scroll(function () {
        let toffset = TBLHD.$table.offset();
        if (toffset.top + TBLHD.$table.height() < $(window).scrollTop()
            || toffset.top > $(window).scrollTop()) {
            if (TBLHD.view == 1) {
                TBLHD.view = 0;
                TBLHD.$clone.css("display", "none");
            }
        }
        else if (toffset.top < $(window).scrollTop()) {
            if (TBLHD.view == 0) {
                TBLHD.createClone();
                TBLHD.view = 1;
                TBLHD.$clone.css("display", "table");
            }
            TBLHD.$clone.css("left", toffset.left - $(window).scrollLeft());
        }
    });
});
$(function () {
    $(document).on('click', '#GridCheckAll', function () {
        $('.grid-check').prop('checked', $('#GridCheckAll').prop('checked'));
        var data = $p.getData($(this));
        data.GridUnCheckedItems = '';
        data.GridCheckedItems = '';
    });
    $(document).on('change', '.grid-check', function () {
        var $control = $(this);
        if ($('#GridCheckAll').prop('checked')) {
            $p.getData($control).GridUnCheckedItems =
                $('.grid-check').filter(':not(:checked)')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        } else {
            $p.getData($control).GridCheckedItems =
                $('.grid-check').filter(':checked')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        }
    });
    $(document).on('change', '.grid .select', function () {
        $p.setData($(this).closest('.grid'))
    });
    $(document).on('change', '.grid .select-all', function () {
        $control = $(this);
        $grid = $(this).closest('.grid');
        $grid.find('.select').prop('checked', $control.prop('checked'));
        $p.setData($grid);
    });
    $(document).on('click', '.grid-row td', function () {
        var $control = $(this).find('.grid-check,.select');
        if ($control.length === 0) {
            var $grid = $(this).closest('.grid');
            if (!$grid.hasClass('not-link')) {
                if ($grid.hasClass('history')) {
                    if (!$p.confirmReload()) return false;
                    var $control = $(this).closest('.grid-row');
                    var data = $p.getData($control);
                    data.Ver = $control.attr('data-ver');
                    data.Latest = $control.attr('data-latest');
                    data.SwitchTargets = $('#SwitchTargets').val();
                    $p.syncSend($control);
                    $p.setCurrentIndex();
                } else {
                    var func = $grid.attr('data-func');
                    var dataId = $(this).closest('.grid-row').attr('data-id');
                    var dataVer = $(this).closest('.grid-row').attr('data-ver');
                    var dataHistory = $(this).closest('.grid-row').attr('data-history');
                    if (func) {
                        $p.getData($grid)[$grid.attr('data-name')] = dataId;
                        $p[func]($grid);
                    }
                    else {
                        var paramVer = dataHistory ? '?ver=' + dataVer : '';
                        var paramBack = paramVer ? '&back=1' : '?back=1';
                        if ($('#EditorDialog').length === 1) {
                            var data = {};
                            data.EditInDialog = true;
                            url = $('#BaseUrl').val() + dataId
                                + paramVer;
                            $p.ajax(url, 'post', data);
                        } else {
                            location.href = $('#BaseUrl').val() + dataId
                                + paramVer
                                + ($grid.attr('data-value') === 'back'
                                    ? paramBack
                                    : '');
                        }
                    }
                }
            }
        } else if (!$p.hoverd($control)) {
            $control.trigger('click');
        }
    });
});

$(function () {
    var timer;
    $(document).on('mouseenter', 'table > thead > tr > th.sortable', function () {
        clearTimeout(timer);
        if ($(".menu-sort:visible").length) {
            $(".menu-sort:visible").hide();
        }
        if ($('.ui-multiselect-close:visible').length) {
            $('.ui-multiselect-close:visible').click();
        }
        timer = setTimeout(function ($control) {
            var dataName = $control.attr('data-name');
            $menuSort = $(".menu-sort[id='GridHeaderMenu__" + dataName + "']");
            $menuSort.css('width', '');
            $menuSort
                .css('position', 'absolute')
                .css('top', $control.position().top + $control.outerHeight())
                .css('left', $control.position().left)
                .outerWidth($control.outerWidth() > $menuSort.outerWidth()
                    ? $control.outerWidth()
                    : $menuSort.outerWidth())
                .show();
        }, 700, $(this));
    });
    $(document).on('mouseenter', 'body > thead > tr > th.sortable', function () {
        clearTimeout(timer);
        if ($(".menu-sort:visible").length) {
            $(".menu-sort:visible").hide();
        }
        if ($('.ui-multiselect-close:visible').length) {
            $('.ui-multiselect-close:visible').click();
        }
        timer = setTimeout(function ($control) {   
            var dataName = $control.attr('data-name');
            $menuSort = $(".menu-sort[id='GridHeaderMenu__" + dataName+ "']");
            $menuSort.css('width', '');
            $menuSort
                .css('position', 'fixed')
                .css('top', $control.position().top + $control.outerHeight())
                .css('left', $control.position().left + $control.offsetParent().offset().left - window.pageXOffset)
                .outerWidth($control.outerWidth() > $menuSort.outerWidth()
                    ? $control.outerWidth()
                    : $menuSort.outerWidth())
                .show();
        }, 700, $(this));
    });
    $(document).on('mouseleave', 'th.sortable', function () {
        clearTimeout(timer);
    });
    $(document).on('mouseleave', '.menu-sort', function () {
        if (!$('.ui-multiselect-menu:visible').length) {
            $('.menu-sort:visible').hide();
        }
    });
    $(document).on('click', '.menu-sort > li.sort', function (e) {
        sort($($(this).parent().attr('data-target')), $(this).attr('data-order-type'));
        e.stopPropagation();
    });
    $(document).on('click', '.menu-sort > li.reset', function (e) {
        var $control = $(this);
        var $grid = $control.closest('.grid');
        var data = $p.getData($control);
        data.Direction = $grid.attr('data-name');
        data.TableId = $grid.attr('id');
        data.TableSiteId = $grid.attr('data-id');
        $('[data-id^="ViewSorters_"]').each(function () {
            delete data[$(this).attr('data-id')];
        });
        $p.send($('#ViewSorters_Reset'));
        e.stopPropagation();
    });
    $(document).on('click', 'th.sortable', function (e) {
        var $control = $(this).find('div');
        sort($control, $control.attr('data-order-type'));
        e.stopPropagation();
    });

    function sort($control, type) {
        var $grid = $control.closest('.grid');
        var data = $p.getData($control);
        data[$control.attr('data-id')] = type;
        data.Direction = $grid.attr('data-name');
        data.TableId = $grid.attr('id');
        data.TableSiteId = $grid.attr('data-id');
        $p.send($grid);
        delete data[$control.attr('id')];
    }
});

$(function () {
    var timer;
    $(document).on('mouseenter', '.grid-row .grid-title-body, .grid-row .comment', function () {
        $(this).addClass('focus-inform');
        timer = setTimeout(function ($control) {
            $control.addClass('height-auto');
        }, 700, $(this));
    });
    $(document).on('mouseleave', '.grid-row .grid-title-body, .grid-row .comment', function () {
        clearTimeout(timer);
        $(this)
            .removeClass('height-auto')
            .removeClass('focus-inform');
    });
});

$p.setGroup = function ($control) {
    $('#CurrentMembers').find('.ui-selected').each(function () {
        var $this = $(this);
        var data = $this.attr('data-value').split(',');
        var type = $control.attr('id');
        $this.attr('data-value', data[0] + ',' + data[1] + ',' + (type === 'Manager'));
        $this.text($this.text().replace(/\(.*\)/, ''));
        $this.text($this.text() + (type === 'GeneralUser'
            ? ''
            : '(' + $p.display(type) + ')'));
    });
    $p.setData($('#CurrentMembers'));
}
$p.setImageLib = function () {
    $p.paging('#ImageLib');
}

$p.deleteImage = function ($control) {
    var data = {};
    data.Guid = $control.attr('data-id');
    $p.ajax(
        $control.attr('data-action'),
        $control.attr('data-method'),
        data,
        $control);
}
$(function () {
    $(window).on('scroll resize', function () {
        if ($('#ImageLib').length === 1) {
            $p.paging('#ImageLib');
        }
    });
});
$p.openImportSettingsDialog = function ($control) {
    $('#ImportSettingsDialog').dialog({
        modal: true,
        width: '520px',
        resizable: false
    });
}

$p.import = function ($control) {
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('Encoding', $('#Encoding').val());
    data.append('UpdatableImport', $('#UpdatableImport').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}
$p.get = function ($control, ajax) {
    if (!$p.confirmReload()) return false;
    if ($p.outsideDialog($control)) return false;
    switch ($control.attr('id')) {
        case 'Reload': move(0, ajax); break;
        case 'Previous': move(-1, ajax); break;
        case 'Next': move(1, ajax); break;
    }

    function move(additional, ajax) {
        var array = $p.switchTargets();
        var index = $p.currentIndex(array);
        var target = index + additional;
        if (index !== -1 && target >= 0 && target < array.length) {
            var url = $('#BaseUrl').val() + array[target];
            if (ajax) {
                $p.ajax(url, 'post', null, null, false);
                if (additional !== 0) history.pushState(null, null, url);
            } else {
                location.href = url;
            }
        }
    }
}

$p.new = function ($control) {
    location.href = $('#BaseUrl').val() +
        $control.attr('data-to-site-id') + '/new' +
        '?FromSiteId=' + $control.attr('data-from-site-id') +
        '&LinkId=' + $control.attr('data-id');
}

$p.copy = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        history.pushState(null, null, $('#BaseUrl').val() + $('#Id').val());
    }
}

$p.search = function (searchWord, redirect, offset) {
    offset = offset !== undefined ? offset : 0;
    if ($p.searchWord !== searchWord + offset) {
        var url = $('#ApplicationPath').val() +
            'items/search?text=' + escape(searchWord)
        if (offset > 0) url += '&offset=' + offset;
        if (redirect) {
            location.href = url;
        } else if (offset !== '-1') {
            $p.ajax(url, 'get', false);
            if (offset === 0) history.pushState(null, null, url);
        }
        $p.searchWord = searchWord + offset;
    }
}
$(function () {
    $p.apply = function () {
        $('.menu').menu();
        $('#EditorTabsContainer:not(.applied),#MailEditorTabsContainer:not(.applied),#ViewTabsContainer:not(.applied),#ColumnAccessControlTabsContainer:not(.applied)').tabs({
            beforeActivate: function (event, ui) {
                if (ui.newPanel.attr('data-action')) {
                    $p.send(ui.newPanel);
                }
            }
        }).addClass('applied');
        $('.button-icon:not(.applied)').each(function () {
            var $control = $(this);
            var icon = $control.attr('data-icon');
            $control.button({ icon: icon });
        }).addClass('applied');
        $('.button-icon.hidden').toggle(false);
        $('select[multiple]:not(.applied)').multiselect({
            selectedList: 100,
            checkAllText: $p.display('CheckAll'),
            uncheckAllText: $p.display('UncheckAll'),
            noneSelectedText: '',
            beforeopen: function (){
                if ($(this).hasClass('search')) {
                    $p.openDropDownSearchDialog($(this));
                    return false;
                }
            },
            click: function () {
                $p.changeMultiSelect($(this))
            },
            checkAll: function () {
                $p.changeMultiSelect($(this))
            },
            uncheckAll: function () {
                $p.changeMultiSelect($(this))
            }
        }).addClass('applied');
        $('.datepicker:not(.applied)').each(function () {
            var $control = $(this);
            $control.datetimepicker({
                format: $control.attr('data-format'),
                timepicker: $control.attr('data-timepicker') === '1',
                step: 10,
                dayOfWeekStart: 1,
                scrollInput: false
            }).addClass('applied');
        });
        switch ($('#Language').val()) {
            case 'ja':
                $.datetimepicker.setLocale('ja');
                break;
        }
        $('.radio:not(.applied)').buttonset().addClass('applied');
        $('.control-selectable:not(.applied)').selectable({
            selected: function (event, ui) {
                if ($(this).hasClass('single')){
                    $(ui.selected)
                        .addClass("ui-selected")
                        .siblings()
                        .removeClass("ui-selected")
                        .each(function (key, value) {
                            $(value).find('*').removeClass("ui-selected");
                        });
                }
            },
            stop: function () {
                $p.setData($(this));
            }
        }).addClass('applied');
        $('.control-slider-ui').each(function () {
            var $control = $('#' + $(this).attr('id').split(',')[0]);
            $(this).slider({
                min: parseFloat($(this).attr('data-min')),
                max: parseFloat($(this).attr('data-max')),
                step: parseFloat($(this).attr('data-step')),
                value: parseFloat($control.text()),
                slide: function (event, ui) {
                    $control.text(ui.value);
                    $control.attr('data-value', ui.value);
                    $p.setData($control);
                },
                stop: function (event, ui) {
                    if ($control.hasClass('auto-postback')) {
                        $p.send($control);
                    }
                }
            });
        });
        $('.control-spinner:not(.applied)').each(function () {
            var $control = $(this);
            $control.spinner({
                min: $control.attr('data-min'),
                max: $control.attr('data-max'),
                step: $control.attr('data-step')
            }).css('width', function () {
                return $control.attr('data-width');
            });
            $control.addClass('applied');
        });
        $('[class*="enclosed"] .legend:not(.applied)').each(function (e) {
            var $control = $(this);
            if ($control.find('[class^="ui-icon ui-icon-triangle-1-"]').length === 0) {
                $control.prepend($('<span/>').addClass('ui-icon ui-icon-triangle-1-s'));
            }
            $control.addClass('applied');
        });
        $('.control-dropdown:not(.applied)').each(function () {
            var $control = $(this);
            var selectedCss = $control.find('option:selected').attr('data-class');
            if (selectedCss !== undefined) {
                $control
                    .addClass(selectedCss)
                    .addClass('applied');
            }
        });
        $('.control-markdown:not(.applied)').each(function () {
            var $control = $(this);
            var $viewer = $('[id="' + this.id + '.viewer"]');
            $viewer.html($p.markup($control.val()));
            $control.addClass('applied');
        });
        $('.markup:not(.applied)').each(function () {
            var $control = $(this);
            $control.html($p.markup($control.html(), true));
            $control.addClass('applied');
        });
        if ($('#Publish').length === 1) {
            $('a').each(function () {
                var $control = $(this);
                if ($control.attr('href').indexOf('/binaries/') !== -1) {
                    $control.attr('href', $control.attr('href').replace('/binaries/', '/publishbinaries/'))
                }
            });
            $('img').each(function () {
                var $control = $(this);
                if ($control.attr('src').indexOf('/binaries/') !== -1) {
                    $control.attr('src', $control.attr('src').replace('/binaries/', '/publishbinaries/'))
                }
            });
        }
        replaceMenu();
    };
    function replaceMenu() {
        var $header;
        var $menu = $('[id^=GridHeaderMenu__]:visible');
        if (!$menu.length) {
            return;
        }
        var dataName = $menu.attr('id').replace('GridHeaderMenu__', '');
        $header = $("body > thead:visible > tr > th.sortable[data-name='" + dataName + "']");
        if ($header.length) {
            if ($(".menu-sort:visible").length) {
                $(".menu-sort:visible").hide();
            }
            if ($('.ui-multiselect-close:visible').length) {
                $('.ui-multiselect-close:visible').click();
            }
        } else {
            $header = $("table > thead > tr > th.sortable[data-name='" + dataName + "']");
            if (!$header.length) {
                return;
            }
            $menu.css('width', '');
            $menu.css('position', 'absolute')
                .css('top', $header.position().top + $header.outerHeight())
                .css('left', $header.position().left)
                .outerWidth($header.outerWidth() > $menuSort.outerWidth()
                    ? $header.outerWidth()
                    : $menuSort.outerWidth());
        }
        
        var $multiSelect = $('.ui-multiselect-menu:visible');
        var $control = $("[id='ViewFiltersOnGridHeader__" + dataName + "_ms']");
        if ($multiSelect.length && $control.length) {
            $multiSelect.css('top', $control.offset().top + $control.outerHeight())
                .css('left', $control.offset().left);
        }
    }
    $p.apply();
});
$p.setKamban = function () {
    $('#KambanValueField').toggle($('#KambanAggregateType').val() !== 'Count');
    $('#KambanBody .kamban-item').draggable({
        revert: 'invalid',
        start: function () {
            $(this).parent().droppable({
                disabled: true
            });
        }
    });
    $('#KambanBody .kamban-container').droppable({
        hoverClass: 'hover',
        tolerance: 'intersect',
        drop: function (e, ui) {
            var data = $p.getData($('.main-form'));
            var tableNamePrefix = $('#TableName').val() + '_';
            var dataX = $(this).attr('data-x');
            var dataY = $(this).attr('data-y');
            data["KambanId"] = $(ui.draggable).attr('data-id');
            if (dataX !== undefined){
                data[tableNamePrefix + $('#KambanGroupByX').val()] = dataX;
            }
            if (dataY !== undefined) {
                data[tableNamePrefix + $('#KambanGroupByY').val()] = dataY;
            }
            $p.send($('#KambanBody'));
        }
    });
}
$(function () {
    $(document).on('dblclick', '.kamban-item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
    $(document).on('click', '.kamban-item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().attr('data-id');
    });
});
$(function () {
    $(document).on('keypress', 'form[data-enter] input:not([type="button"])', function (e) {
        if (e.which === 13) {
            $($(this).closest('form').attr('data-enter')).click();
        }
    });
    $(document).on('keypress', 'input', function (e) {
        return e.which !== 13;
    });
    $(document).on('keydown', '[class^="control-"]', function (e) {
        if (e.keyCode === 9) {
            var controlId = $(this).attr('id');
            var find = false;
            var ret = true;
            var status = 0;
            var $controls = !event.shiftKey
                ? $('[class^="control-"]')
                : $($('[class^="control-"]').get().reverse());
            $controls.each(function () {
                if (find) {
                    status = setFocus($(this));
                    if (status === 1) {
                        return false;
                    }
                    if (status === 2) {
                        ret = false;
                        return false;
                    }
                } else if ($(this).attr('id') === controlId) {
                    find = true;
                }
            });
            return ret;
        }

        function setFocus($control) {
            if ($control.hasClass('control-markdown')) {
                $p.toggleEditor($control, true);
                $('#' + $control.attr('id')).focus();
                return 2;
            } else if ($control.is(':hidden') ||
                $control.hasClass('control-text') ||
                $control.hasClass('control-markup')) {
                return 0;
            }
            return 1;
        }
    });
});
$(function () {
    $(document).on('click', '[class*="enclosed"] .legend', function () {
        var icon = $(this).find('.ui-icon');
        if (icon.length === 1) {
            if ($(this).find('.ui-icon-triangle-1-s')[0]) {
                icon.removeClass('ui-icon-triangle-1-s');
                icon.addClass('ui-icon-triangle-1-e');
            }
            else {
                icon.removeClass('ui-icon-triangle-1-e');
                icon.addClass('ui-icon-triangle-1-s');
            }
            $(this).parent().children('div').toggle();
        }
    });
});
$p.loading = function ($control) {
    if ($control) {
        if ($control.prop('tagName') === 'BUTTON') {
            $control.prop('disabled', true).addClass('loading');
            var $icon = $control.find('.ui-icon');
            $icon
                .attr('data-css', $icon.prop('class'))
                .prop('class', 'ui-icon')
                .css('background-image', 'url(' + $('#Logo > a').attr('href') + 'images/loading.gif)');
        }
    }
}

$p.loaded = function () {
    $('button.loading').each(function () {
        var $control = $(this);
        $control
            .removeClass('loading')
            .prop('disabled', false);
        var $icon = $control.find('.ui-icon');
        $icon
            .removeAttr('style')
            .prop('class', $icon.attr('data-css'));
    });
}
$p.generalId = function ($control) {
    var controlId = $control.attr('id');
    return controlId.indexOf('.') === -1
        ? controlId
        : controlId.substring(0, controlId.indexOf('.'));
}

$p.showMarkDownViewer = function ($control) {
    var $viewer = $('[id="' + $control.attr('id') + '.viewer"]');
    if ($viewer.length === 1) {
        $viewer.html($p.markup($control.val()));
        $p.resizeEditor($control, $viewer);
        $p.toggleEditor($viewer, false);
    }
}

$p.editMarkdown = function ($control) {
    $p.toggleEditor($control, true);
    $('[id=\'' + $p.generalId($control) + '\']').focus();
}

$p.toggleEditor = function ($control, edit) {
    var id = $p.generalId($control);
    if ($('[id="' + id + '.editor"]').length !== 0) {
        if (edit) {
            $p.resizeEditor($('[id="' + id + '"]'), $('[id="' + id + '.viewer"]'));
        }
        $('[id="' + id + '.viewer"]').toggle(!edit);
        $('[id="' + id + '.editor"]').toggle(!edit);
        $('[id="' + id + '"]').toggle(edit);
    }
}

$p.resizeEditor = function ($control, $viewer) {
    if ($viewer.height() <= 300) {
        $control.height($viewer.height());
    }
    else {
        $control.height(300);
    }
}

$p.markup = function (markdownValue, encoded) {
    var text = markdownValue;
    if (!encoded) text = getEncordedHtml(text);
    text = replaceUnc(text);
    return text.indexOf('[md]') === 0
        ? '<div class="md">' + marked(text.substring(4)) + '</div>'
        : replaceUrl(markedUp(text));

    function markedUp(text) {
        var $html = $('<pre/>')
        text.split(/\r\n|\r|\n/).forEach(function (line) {
            if (line !== '') {
                $html.append($('<span/>').append(line));
            }
            $html.append($('<br/>'));
        });
        return $html[0].outerHTML;
    }

    function replaceUrl(text) {
        var regex_i = /(!\[[^\]]+\]\(.+?\))/gi;
        var regex_t = /(\[[^\]]+\]\(.+?\))/gi;
        var regex = /(\b(https?|notes|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_i, function ($1) {
                return '<a href="' + address($1) + '" target="_blank">' +
                    '<img src="' + address($1) + '" alt="' + title($1) + '" /></a>';
            })
            .replace(regex_t, function ($1) {
                return '<a href="' + address($1) + '" target="_blank">' + title($1) + '</a>';
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? '<a href="' + $1 + '" target="_blank">' + $1 + '</a>'
                    : $1;
            });
    }

    function replaceUnc(text) {
        var regex_t = /(\[[^\]]+\]\(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\))/gi;
        var regex = /(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_t, function ($1) {
                return '<a href="file://' + address($1) + '">' + title($1) + '</a>';
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? '<a href="file://' + $1 + '">' + $1 + '</a>'
                    : $1;
            });
    }

    function getEncordedHtml(value) {
        return $('<div/>').text(value).html();
    }

    function address($1) {
        var m = $1.match(/\(.+?\)/gi)[0];
        return m.substring(1, m.length - 1);
    }

    function title($1) {
        var m = $1.match(/\[[^\]]+\]/i)[0];
        return m.substring(1, m.length - 1);
    }
}

$p.insertText = function ($control, value) {
    var body = $control.get(0);
    body.focus();
    var start = body.value;
    var caret = body.selectionStart;
    var next = caret + value.length;
    body.value = start.substr(0, caret) + value + start.substr(caret);
    body.setSelectionRange(next, next);
    $p.setData($control);
    if (!$control.is(':visible')) {
        $p.showMarkDownViewer($control);
    }
}

$p.selectImage = function (controlId) {
    $('[id="' + controlId + '.upload-image-file"]').click();
}

$p.uploadImage = function (controlId, file) {
    var url = $('.main-form')
        .attr('action')
        .replace('_action_', 'binaries/uploadimage');
    var data = new FormData();
    data.append('ControlId', controlId);
    data.append('file', file);
    $p.multiUpload(url, data);
}
$(function () {
    $(document).on('blur', '.control-markdown:not(.error)', function () {
        $p.showMarkDownViewer($(this));
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined &&
            e.originalEvent.clipboardData.types.indexOf('text/plain') === -1) {
            var items = e.originalEvent.clipboardData.items;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
                if (item.type.indexOf('image') !== -1) {
                    $p.uploadImage(this.id, item.getAsFile());
                }
            }
        }
    });
    $(document).on('change', '.upload-image-file', function () {
        if (this.files.length === 1) {
            $p.uploadImage($(this).attr('data-id'), this.files[0]);
            this.value = '';
        }
    });
});
$(function () {
    $(document).on('mouseenter', '#NavigationMenu > li', function () {
        var $container = $(this).find(':first-child');
        $container.addClass('hover');
        $('#' + $container.attr('data-id')).show();
    });
    $(document).on('mouseleave', '#NavigationMenu > li', function () {
        var $container = $(this).find(':first-child');
        $container.removeClass('hover');
        $('#' + $container.attr('data-id')).hide();
    });
});
$p.setMessage = function (target, value) {
    var message = JSON.parse(value);
    var $control = target !== undefined
        ? target.split('_')[0] === 'row'
            ? $('[data-id="' + target.split('_')[1] + '"]')
            : $(target)
        : $('.message-dialog:visible');
    if ($control.prop('tagName') === 'TR') {
        var $body = $($('<tr/>')
            .addClass("message-row")
            .append($('<td/>')
                .attr('colspan', $control.find('td').length)
                .append($('<span/>')
                    .addClass('body')
                    .addClass(message.Css)
                    .text(message.Text))));
        $control.after($body);
        $('html,body').animate({
            scrollTop: $control.offset().top - 50
        });
    }
    else {
        var $body = $($('<div/>')
            .append($('<span/>')
                .addClass('body')
                .addClass(message.Css)
                .text(message.Text)));
        if ($control.length === 0) {
            if ($('#Message').hasClass('message')) {
                $body.append($('<span/>')
                    .addClass('ui-icon ui-icon-close close'));
            }
            $('#Message').append($body);
        } else {
            $control.append($body);
        }
    }
}

$p.setErrorMessage = function (error, target) {
    var data = {};
    data.Css = 'alert-error';
    data.Text = $p.display(error);
    $p.clearMessage();
    $p.setMessage(target, JSON.stringify(data));
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
}
$(function () {
    $(document).on('click', '.message .close', function () {
        $(this).parent().remove();
    });

    var $data = $('#MessageData');
    if ($data.length === 1) {
        $p.setMessage('#Message', $data.val());
        $data.remove();
    }
});
$p.moveTargets = function ($control) {
    $p.send($control);
    $p.openDialog($control, '.main-form')
}

$p.move = function ($control) {
    var error = $p.syncSend($control);
}
$p.changeMultiSelect = function ($control) {
    $p.setData($control);
    if ($control.hasClass('auto-postback') && !$control.hasClass('no-postback')) {
        $p.send($control);
    }
    $control.removeClass('no-postback');
};

$p.setMultiSelectData = function ($control) {
    $p.getData($control)[$control.attr('id')] = JSON.stringify(
        $('[name="multiselect_' + $control.attr('id') + '"]')
            .filter(function () { return $(this).prop('checked'); })
            .map(function () { return $(this).val() })
            .toArray());
};

$p.selectMultiSelect = function ($control, json) {
    $control.find('option').each(function (index, element) {
        var $element = $(element);
        $element.prop('selected', false);
        if (JSON.parse(json).indexOf($element.val()) > -1) {
            $element.prop('selected', true);
        }
    });
    $control.multiselect('refresh');
};

$p.RefreshMultiSelectRelatingColum = function ($target) {
    if ($target.length === 0) {
        return;
    }
    if ($target.prop('multiple')) {
        $target.multiselect('refresh');
        var $currentOptions = $('#' + $target.attr('id') + ' option:selected');
        var curOptions = [];
        $currentOptions.each(function (index, element) {
            curOptions.push($(element).val());
        });
        var oldOptions = JSON.parse($target.attr('selected-options'));
        if (!Array.isArray(oldOptions)) {
            return false;
        }
        if (curOptions.length !== oldOptions.length) {
            $p.setData($target);
            $p.send($target);
            return;
        }
        var equals = true;
        curOptions.forEach(function (item) {
            if (!oldOptions.includes(item)) {
                equals = false;
            }
        });
        if (!equals) {
            $p.setData($target);
            $p.send($target);
        }
    }
};
$p.currentIndex = function (array) {
    return array.indexOf($('#Id').val())
}

$p.switchTargets = function () {
    var $control = $('#SwitchTargets');
    return $control.length === 1
        ? $control.val().split(',')
        : [];
}

$p.setSwitchTargets = function () {
    var $control = $('#SwitchTargets');
    if ($control.length === 1) {
        $control.appendTo('body');
        $p.setCurrentIndex();
    }
}

$p.setCurrentIndex = function () {
    var array = $p.switchTargets();
    if (array.length > 1) {
        var index = $p.currentIndex(array);
        $('#CurrentIndex').text(index + 1 + '/' + array.length);
    } else {
        $('#Previous').hide();
        $('#CurrentIndex').hide();
        $('#Next').hide();
    }
}

$p.back = function () {
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        location.href = $control.val();
    }
}
$(function () {
    $(window).on('popstate', function (e) {
        if (e.originalEvent.currentTarget.location.pathname !== $('#BaseUrl').val() + $('#Id').val()
            || e.originalEvent.state === "History" || urlParams()["ver"]) {
            $p.ajax(e.originalEvent.currentTarget.location, 'post');
        }
    });
    $p.setSwitchTargets();
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $control.appendTo('body');
    }

    function urlParams() {
        var urlParam = location.search.substring(1);
        var paramArray = [];
        if (urlParam) {
            var param = urlParam.split('&');
            for (i = 0; i < param.length; i++) {
                var paramItem = param[i].split('=');
                paramArray[paramItem[0]] = paramItem[1];
            }
        }
        return paramArray;
    }
});
$p.openOutgoingMailDialog = function ($control) {
    var error = 0;
    if ($('#OutgoingMails_Title').length === 0) {
        var data = $p.getData($('#OutgoingMailsForm'));
        data.Controller = $('#Controller').val();
        data.Id = $('#Id').val();
        error = $p.syncSend($control, 'OutgoingMailsForm');
    }
    if (error === 0) {
        $('#OutgoingMailDialog').dialog({
            modal: true,
            width: '90%',
            height: 'auto',
            dialogClass: 'outgoing-mail',
            resizable: false
        });
    }
}

$p.openOutgoingMailReplyDialog = function ($control) {
    $p.getData($('#OutgoingMailsForm')).OutgoingMails_OutgoingMailId = $control.attr('data-id');
    var error = $p.syncSend($control, 'OutgoingMailsForm');
    if (error === 0) {
        $('#OutgoingMailDialog').dialog({
            modal: true,
            width: '90%',
            height: 'auto',
            dialogClass: 'outgoing-mail',
            resizable: false
        });
    }
}

$p.sendMail = function ($control) {
    var data = $p.getData($('#OutgoingMailForm'));
    data.Ver = $('._Ver')[0].innerHTML;
    data.Controller = $('#Controller').val();
    data.Id = $('#Id').val();
    $p.send($control);
}

$p.initOutgoingMailDialog = function () {
    var body = $('#' + $p.tableName() + '_Body').val();
    body = body !== undefined ? body + '\n\n' : '';
    if ($('#OutgoingMails_Reply').val() !== '1') {
        $('#OutgoingMails_Title').val($('#HeaderTitle').text());
        $('#OutgoingMails_Body').val(body + $('#OutgoingMails_Location').val());
    }
    $p.addMailAddress($('#OutgoingMails_To'), $('#To').val());
    $p.addMailAddress($('#OutgoingMails_Cc'), $('#Cc').val());
    $p.addMailAddress($('#OutgoingMails_Bcc'), $('#Bcc').val());
}

$p.addMailAddress = function ($control, defaultMailAddresses) {
    var mailAddresses = defaultMailAddresses !== undefined
        ? defaultMailAddresses
        : $('#OutgoingMails_MailAddresses').find('.ui-selected').map(function () {
            return unescape($(this).text());
        }).get().join(';');
    if (mailAddresses) {
        mailAddresses.split(';').forEach(function (mailAddress) {
            if (mailAddress) {
                $p.addBasket($control, mailAddress);
            }
        });
    }
    $p.setData($control);
}
$(function () {
    $(document).on('click', '#OutgoingMails_AddTo', function () {
        $p.addMailAddress($('#OutgoingMails_To'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddCc', function () {
        $p.addMailAddress($('#OutgoingMails_Cc'));
        showMailEditor();
    });
    $(document).on('click', '#OutgoingMails_AddBcc', function () {
        $p.addMailAddress($('#OutgoingMails_Bcc'));
        showMailEditor();
    });

    function showMailEditor() {
        $('#MailEditorTabsContainer').tabs('option', 'active', 0);
    }
});
$p.setPermissionEvents = function () {
    $p.setPaging('SourcePermissions');
}

$p.setPermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    $p.setData($('#SourcePermissions'));
    $p.setData($('#SearchPermissionElements'));
    $p.send($control);
}

$p.openPermissionsDialog = function ($control) {
    $p.data.PermissionsForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionsDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.changePermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissions = mainFormData.CurrentPermissions;
    data.CurrentPermissionsAll = mainFormData.CurrentPermissionsAll;
    $p.send($control);
}

$p.setPermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    $p.setData($('#SourcePermissionForCreating'));
    $p.send($control);
}

$p.openPermissionForCreatingDialog = function ($control) {
    $p.data.PermissionForCreatingForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionForCreatingDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.changePermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissionForCreating = mainFormData.CurrentPermissionForCreating;
    data.CurrentPermissionForCreatingAll = mainFormData.CurrentPermissionForCreatingAll;
    $p.send($control);
}
$p.hoverd = function ($elements) {
    var $element;
    $elements.each(function () {
        if (isHover($(this))) {
            $element = $(this);
            return false;
        }
    });
    return $element;

    function isHover($element) {
        var left = $element.offset().left;
        var top = $element.offset().top;
        var right = left + $element.outerWidth();
        var bottom = top + $element.outerHeight();
        if ($p.mouseX >= left &&
            $p.mouseX <= right &&
            $p.mouseY >= top &&
            $p.mouseY <= bottom) {
            return true;
        } else {
            return false
        }
    }
}
$(window).mousemove(function (e) {
    $p.mouseX = e.pageX;
    $p.mouseY = e.pageY;
});
$p.saveScroll = function () {
    $p.scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
    $p.scrollY = document.documentElement.scrollTop || document.body.scrollTop;
}

$p.loadScroll = function () {
    window.scroll($p.scrollX, $p.scrollY);
}

$p.clearScroll = function () {
    $p.scrollX = 0;
    $p.scrollY = 0;
}

$p.paging = function (selector) {
    var $control = $(selector);
    var $offset = $(selector + 'Offset');
    if ($control.length) {
        if ($(window).scrollTop() + $(window).height() >= $control.offset().top + $control.height()) {
            if ($offset.val() !== '-1') {
                $p.setData($offset);
                $offset.val('-1');
                $p.send($control);
            }
        }
    }
}

$p.setPaging = function (controlId) {
    var wrapper = document.getElementById(controlId + 'Wrapper');
    var height = wrapper.offsetHeight;
    $(wrapper).scroll(function () {
        var scrollHeight = wrapper.scrollHeight;
        var scrollTop = wrapper.scrollTop;
        var scrollPosition = height + scrollTop;
        var $offset = $('#' + controlId + 'Offset');
        if ((scrollHeight - scrollPosition) / scrollHeight <= 0 && $offset.val() !== '-1') {
            $p.send($('#' + controlId));
            $offset.val('-1');
        }
    });
}
$(function () {
    $(window).on('scroll resize', function () {
        $p.paging('#Grid');
    });
});
$(function () {
    $(document).on('mouseenter', '#Search', function () {
        $(this).prop('disabled', false);
    });
    $(document).on('change', '#Search', function () {
        $p.search($(this).val(), $(this).hasClass('redirect'));
    });
    $(document).on('keydown', '#Search', function (e) {
        if (e.which === 13) $p.search($(this).val(), $(this).hasClass('redirect'));
    });
    if ($('#SearchResults').length === 1) {
        $(window).on('scroll resize', function () {
            var $control = $('#SearchResults')
            if ($control.length) {
                if ($(window).scrollTop() + $(window).height() >= $control.offset().top + $control.height()) {
                    if ($('#SearchOffset').val() !== '-1') {
                        $p.search($('#Search').val(), false, $('#SearchOffset').val());
                    }
                }
            }
        });
        $(document).on('click', '#SearchResults .result', function () {
            location.href = $(this).attr('data-href');
        });
    }
});
$p.openChangePasswordDialog = function () {
    var data = $p.getData($('#ChangePasswordForm'));
    data.Users_LoginId = $('#Users_LoginId').val();
    data.Users_OldPassword = $('#Users_Password').val();
    data.ReturnUrl = $('#ReturnUrl').val();
    $('#ChangePasswordDialog').dialog({
        modal: true,
        width: '420px',
        resizable: false
    });
}

$p.changePasswordAtLogin = function ($control) {
    var data = $p.getData($('#ChangePasswordForm'));
    data.Users_LoginId = $('#Users_LoginId').val();
    data.Users_Password = $('#Users_Password').val();
    $p.send($control);
}
$p.addSelected = function ($control, $target) {
    $control
        .closest('.container-selectable')
        .find('.ui-selected')
        .appendTo($target);
    $p.setData($target);
    $p.send($control);
}

$p.deleteSelected = function ($control) {
    var $targets = $control
        .closest('.container-selectable')
        .find('.control-selectable')
    $targets
        .find('.ui-selected')
        .remove();
    $p.setData($control);
    $p.send($control);
}
$p.openSeparateSettingsDialog = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#SeparateSettingsDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '.main-form',
            resizable: false
        });
    }
}

$p.separateSettings = function () {
    $(function () {
        $("#SeparateSettings").on('spin', '#SeparateNumber', function (event, ui) {
            setSeparateNumber($(this), ui.value);
        });
        $("#SeparateSettings").on('change', '#SeparateNumber', function () {
            setSeparateNumber($(this), $(this).val());
        });
        $("#SeparateSettings").on('spin', '[id*="SeparateWorkValue_"]', function (event, ui) {
            return setSource($(this), ui.value);
        });
        $("#SeparateSettings").on('change', '[id*="SeparateWorkValue_"]', function () {
            setSource($(this), $(this).val());
        });
    });

    function setSeparateNumber($sender, number) {
        $('#SeparateSettings .item').each(function (i) {
            if (number >= i + 1) {
                if ($(this).hasClass('hidden')) {
                    $(this).removeClass('hidden');
                }
            } else {
                if (!$(this).hasClass('hidden')) {
                    $(this).addClass('hidden');
                }
            }
        });
        setSource($sender, 0);
    }

    function setSource($sender, value) {
        var ret = true;
        var sum = getSum($sender, value);
        var $source = $('#SourceWorkValue');
        var source = parseFloat($source.attr('data-value'));
        if (source >= sum) {
            source = (Math.round((source - sum) * 100) / 100);
        } else {
            $sender.val(Math.round((source - getSum($sender, 0)) * 100) / 100);
            ret = false;
            source = 0;
        }
        $source.text(source + $('#WorkValueUnit').val());
        $p.getData($sender).SourceWorkValue = source;
        return ret;
    }

    function getSum($sender, value) {
        var controls = $('[id*="SeparateWorkValue_"]:visible');
        return $.map(controls, function (control) {
            return $sender.attr('id') !== control.id
                ? parseFloat($(control).val())
                : parseFloat(value);
        }).reduce(function (x, y) { return x + y; });
    }
}
$p.openSetDateRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    error = $p.send($control);
    if (error === 0) {
        $('#SetDateRangeDialog').dialog({
            autoOpen: false,
            modal: true,
            height: 'auto',
            width: '700px',
            resizable: false,
            position: { of: window }
        });
        $('#SetDateRangeDialog').dialog("open");
    }
}
$p.openSiteSetDateRangeDialog = function ($control, timepicker) {
    $control.blur();
    $p.openSiteSettingsDialog($control, '#SetDateRangeDialog', 'auto');
    $target = $('#' + $control.attr('id').replace("_DateRange", ""));
    var initValue = JSON.parse($target.val() || "null");
    var startValue = "";
    var endValue = "";
    if (Array.isArray(initValue) && initValue.length > 0) {
        var values = initValue[0].split(',');
        if (values.length > 0) {
            startValue = timepicker ? values[0] : values[0].split(' ')[0];
        }
        if (values.length > 1) {
            endValue = timepicker ? values[1] : values[1].split(' ')[0];
        }
    }
    $('#dateRangeStart').val(startValue);
    $('#dateRangeEnd').val(endValue);
}
$p.openSetDateRangeOK = function ($controlID, timepicker) {
    var sdval = $('#dateRangeStart').val();
    var edval = $('#dateRangeEnd').val();
    var setval = "";
    var dispval = "";
    if (sdval || edval) {
        dispval = sdval + "-" + edval;
        if (!timepicker && sdval) { sdval += " 00:00:00.000"; }
        if (!timepicker && edval) { edval += " 23:59:59.997"; }
        setval = '["' + sdval + ',' + edval + '"]';
    }
    $control = $('#' + $controlID);
    $target = $('#' + $controlID.replace("_DateRange", ""));
    $control.val(dispval);
    $p.set($target, setval);
    $('#SetDateRangeDialog').dialog("close");
    $p.send($target);
}
$p.openSetDateRangeClear = function () {
    $('#dateRangeStart').val("");
    $('#dateRangeEnd').val("");
}
$p.openSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.set($control, $control.val());
    error = $p.send($control);
    if (error === 0) {
        $('#SetNumericRangeDialog').dialog({
            modal: true,
            height: 'auto',
            width: '700px',
            resizable: false,
            position: { of: window }
        });
    }
}
$p.openSiteSetNumericRangeDialog = function ($control) {
    $control.blur();
    $p.openSiteSettingsDialog($control, '#SetNumericRangeDialog', 'auto');
    $target = $('#' + $control.attr('id').replace("_NumericRange", ""));
    var initValue = JSON.parse($target.val() || "null");
    var startValue = "";
    var endValue = "";
    if (Array.isArray(initValue) && initValue.length > 0) {
        var values = initValue[0].split(',');
        if (values.length > 0) {
            startValue = values[0];
        }
        if (values.length > 1) {
            endValue = values[1];
        }
    }
    $('#numericRangeStart').val(startValue);
    $('#numericRangeEnd').val(endValue);
}
$p.openSetNumericRangeOK = function ($controlID) {
    $start = $('#numericRangeStart');
    $end = $('#numericRangeEnd');
    $start.validate();
    $end.validate();
    if (!$start.valid() || !$end.valid()) {
        $p.setErrorMessage('ValidationError');
        return false;
    }
    $control = $('#' + $controlID);
    $target = $('#' + $controlID.replace("_NumericRange", ""));
    var sdval = $("#numericRangeStart").val();
    var edval = $("#numericRangeEnd").val();
    var setval = "";
    var dispval = "";
    if (sdval || edval) {
        dispval = sdval + " - " + edval;
        setval = '["' + sdval + ',' + edval + '"]';
    }
    $control.val(dispval);
    $p.set($target, setval);
    $('#SetNumericRangeDialog').dialog("close");
    $p.send($target);
}
$p.openSetNumericRangeClear = function ($control) {
    $("#numericRangeStart").val("");
    $("#numericRangeEnd").val("");
    $p.clearMessage();
}
$p.openDeleteSiteDialog = function () {
    $('#DeleteSiteDialog input').val('');
    $('#DeleteSiteDialog').dialog({
        modal: true,
        width: '420px',
        height: 'auto',
        appendTo: '.main-form',
        resizable: false
    });
}
$p.tableName = function () {
    return $('#TableName').val();
}

$p.methodType = function () {
    return $('#MethodType').val();
}
$p.setSiteMenu = function () {
    $('.nav-sites.sortable').sortable({
        delay: 150,
        stop: function (event, ui) {
            var siteId = ui.item.attr('data-value');
            var $element = $p.hoverd($('.nav-site:not([data-value="' + siteId + '"])'));
            if ($element) {
                ui.item.hide();
                var data = $p.getData($('.main-form'));
                data.SiteId = siteId;
                data.DestinationId = $element.attr('data-value');
                $p.send($('#MoveSiteMenu'));
            }
        },
        update: function () {
            $p.getData($(this)).Data = $p.toJson($('.nav-sites.sortable li'));
            $p.send($('#SortSiteMenu'));
        }
    });
}

$p.openLinkDialog = function () {
    $('#LinkDialog').dialog({
        modal: true,
        width: '420px',
        appendTo: '.main-form',
        resizable: false
    });
}
$p.openImportSitePackageDialog = function ($control) {
    error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ImportSitePackageDialog').dialog({
            modal: true,
            width: '520px'
        });
    }
}

$p.importSitePackage = function ($control) {
    $p.setData($('#IncludeData'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('IncludeData', $('#IncludeData').prop('checked'));
    data.append('IncludeSitePermission', $('#IncludeSitePermission').prop('checked'));
    data.append('IncludeRecordPermission', $('#IncludeRecordPermission').prop('checked'));
    data.append('IncludeColumnPermission', $('#IncludeColumnPermission').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}

$p.openExportSitePackageDialog = function ($control) {
    error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ExportSitePackageDialog').dialog({
            modal: true,
            width: '720px'
        });
    }
}

$p.exportSitePackage = function () {
    $p.setData($('#SitePackagesSelectable'));
    $p.setData($('#SitePackagesSelectableAll'));
    $p.setData($('#SitePackagesSource'));
    $p.setData($('#UseIndentOption'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    var data = $p.getData($('#SitePackageForm'));
    location.href = $('.main-form').attr('action').replace('_action_', 'ExportSitePackage')
        + '?SitePackagesSelectableAll=' + data.SitePackagesSelectableAll
        + '&UseIndentOption=' + data.UseIndentOption
        + '&IncludeSitePermission=' + data.IncludeSitePermission
        + '&IncludeRecordPermission=' + data.IncludeRecordPermission
        + '&IncludeColumnPermission=' + data.IncludeColumnPermission;
    $p.closeDialog($('#ExportSitePackageDialog'));
    $('#ExportSitePackageDialog').html('');
}

$p.siteSelected = function ($control, $target) {
    $control
        .closest('.container-selectable')
        .find('.ui-selected[data-value!=' + $('#Id').val() + ']')
        .appendTo($target);
    var container = document.getElementById("SitePackagesSelectable");
    var items = container.getElementsByTagName("li");
    var itemArray = Array.prototype.slice.call(items);
    function compareText(a, b) {
        var _a = parseInt(a.attributes.getNamedItem("data-order").value);
        var _b = parseInt(b.attributes.getNamedItem("data-order").value);
        if (_a > _b)
            return 1;
        else if (_a < _b)
            return -1;
        return 0;
    }
    itemArray.sort(compareText);
    for (var i = 0; i < itemArray.length; i++) {
        container.appendChild(container.removeChild(itemArray[i]))
    }
    $p.setData($target);
}

$p.setIncludeExportData = function ($control) {
    $('#SitePackagesSelectable').find('.ui-selected').each(function () {
        var $selected = $(this);
        if ($selected.attr('data-value') == undefined) {
            return true;
        }
        var data = $selected.attr('data-value').split('-');
        var type = $control.attr('id');
        $selected.attr('data-value', data[0] + '-' + (type === 'IncludeData'));
        $selected.find('span.include-data').remove();
        if (type === 'IncludeData') {
            $selected.append('<span class="include-data">(' + $p.display('IncludeData') + ')</span>');
        }
    });
    $p.setData($('#SitePackagesSelectable'));
}
$p.uploadSiteImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#SiteImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}

$p.openSiteSettingsDialog = function ($control, selector, width) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $(selector).dialog({
            modal: true,
            width: width !== undefined ? width : '90%',
            height: 'auto',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.openGridColumnDialog = function ($control) {
    $p.data.GridColumnForm = {};
    $p.openSiteSettingsDialog($control, '#GridColumnDialog');
}

$p.setGridColumn = function ($control) {
    $p.setData($('#UseGridDesign'));
    $p.setData($('#GridDesign'));
    $p.send($control);
}

$p.openFilterColumnDialog = function ($control) {
    $p.data.FilterColumnForm = {};
    $p.openSiteSettingsDialog($control, '#FilterColumnDialog');
}

$p.openAggregationDetailsDialog = function ($control) {
    $p.data.AggregationDetailsForm = {};
    $p.openSiteSettingsDialog($control, '#AggregationDetailsDialog', '420px');
}

$p.setAggregationDetails = function ($control) {
    $p.setData($('#AggregationType'));
    $p.setData($('#AggregationTarget'));
    $p.setData($('#SelectedAggregation'));
    $p.send($control);
}

$p.openEditorColumnDialog = function ($control) {
    $p.data.EditorColumnForm = {};
    $p.openSiteSettingsDialog($control, '#EditorColumnDialog');
}

$p.resetEditorColumn = function ($control) {
    $p.syncSend($control);
    var data = $p.getData($('#EditorColumnForm'));
    $('#EditorColumnForm [class^="control-"]').each(function (index, control) {
        $p.setData($(control), data);
    });
}

$p.openSummaryDialog = function ($control) {
    $p.data.SummaryForm = {};
    $p.openSiteSettingsDialog($control, '#SummaryDialog');
}

$p.setSummary = function ($control) {
    $p.setData($('#EditSummary'), $p.getData($control));
    $p.send($control);
}

$p.openFormulaDialog = function ($control) {
    $p.data.FormulaForm = {};
    $p.openSiteSettingsDialog($control, '#FormulaDialog');
}

$p.openViewDialog = function ($control) {
    $p.data.ViewForm = {};
    $p.openSiteSettingsDialog($control, '#ViewDialog');
}

$p.openNotificationDialog = function ($control) {
    $p.data.NotificationForm = {};
    $p.openSiteSettingsDialog($control, '#NotificationDialog');
}

$p.setNotification = function ($control) {
    $p.setData($('#EditNotification'), $p.getData($control));
    $p.send($control);
}

$p.openReminderDialog = function ($control) {
    $p.data.ReminderForm = {};
    $p.openSiteSettingsDialog($control, '#ReminderDialog');
}

$p.setReminder = function ($control) {
    $p.setData($('#EditReminder'), $p.getData($control));
    $p.send($control);
}

$p.openExportDialog = function ($control) {
    $p.data.ExportForm = {};
    $p.openSiteSettingsDialog($control, '#ExportDialog');
}

$p.setExport = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openExportColumnsDialog = function ($control) {
    $p.data.ExportColumnsForm = {};
    $p.openSiteSettingsDialog($control, '#ExportColumnsDialog', '420px');
}

$p.setExportColumn = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openScriptDialog = function ($control) {
    $p.data.ScriptForm = {};
    $p.openSiteSettingsDialog($control, '#ScriptDialog');
}

$p.setScript = function ($control) {
    $p.setData($('#EditScript'), $p.getData($control));
    $p.send($control);
}

$p.openStyleDialog = function ($control) {
    $p.data.StyleForm = {};
    $p.openSiteSettingsDialog($control, '#StyleDialog');
}

$p.setStyle = function ($control) {
    $p.setData($('#EditStyle'), $p.getData($control));
    $p.send($control);
}

$p.addSummary = function ($control) {
    $p.setData($('#SummarySiteId'));
    $p.setData($('#SummaryDestinationColumn'));
    $p.setData($('#SummaryLinkColumn'));
    $p.setData($('#SummaryType'));
    $p.setData($('#SummarySourceColumn'));
    $p.send($control);
}

$p.openRelatingColumnDialog = function ($control) {
    $p.data.RelatingColumnForm = {};
    $p.openSiteSettingsDialog($control, '#RelatingColumnDialog');
}

$p.setRelatingColumn = function ($control) {
    $p.setData($('#EditRelatingColumns'), $p.getData($control));
    $p.send($control);
}
$(function () {
    $(document).on('change', '#UseGridDesign', function () {
        $('#GridDesignField').toggle($(this).prop('checked'));
    });
    $(document).on('change', '#ControlType', function () {
        var visibility = $(this).val() === 'Spinner';
        $('#MinField').toggle(visibility);
        $('#MaxField').toggle(visibility);
        $('#StepField').toggle(visibility);
    });
    $(document).on('change', '#FormatSelector', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('#FormatField').toggle(true);
                $('#Format').val('');
                break;
            default:
                $('#FormatField').toggle(false);
                $('#Format').val($control.val());
                break;
        }
        $p.setData($('#Format'));
    });
    $(document).on('change', '#NotificationType', function () {
        $('#NotificationTokenField').toggle(
            $('#NotificationTokenEnableList').val()
                .split(',')
                .indexOf($('#NotificationType').val()) !== -1);
    });
    $(document).on('click', '#SummarySettings .grid-row button', function () {
        var $control = $($(this).attr('data-selector'))
        $p.getData($control)[$control.attr('id') + "Id"] = $(this).attr('data-id');
        $p.send($control);
    });
    $(document).on('change', '#SummaryDestinationCondition', function () {
        $('#SummarySetZeroWhenOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('change', '#FormulaCondition', function () {
        $('#FormulaOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('click', '#AddViewSorter', function () {
        var $dataViewSorter = $('#ViewSorterSelector option:selected');
        var orderType = $('#ViewSorterOrderTypes option:selected').val();
        var orderText = '';
        switch (orderType) {
            case 'asc':
                orderText = '(' + $p.display('OrderAsc') + ')';
                break;
            case 'desc':
                orderText = '(' + $p.display('OrderDesc') + ')';
                break;
        }
        $p.addBasket(
            $('#ViewSorters'),
            $dataViewSorter.text() + orderText,
            $dataViewSorter.val() + '&' + orderType);
    });
    $(document).on('change', '#StyleAll', function () {
        if ($('#StyleAll').prop('checked')) {
            $('.output-destination-style')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-style')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#ScriptAll', function () {
        if ($('#ScriptAll').prop('checked')) {
            $('.output-destination-script')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-script')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#DateFilterSetMode', function () {
        if ($('#DateFilterSetMode').val() === '1') {
            $('#FilterColumnSettingField').removeClass('hidden');
        } else {
            $('#FilterColumnSettingField').addClass('hidden');
        }
    });
    $(document).on('change', '#UseRelatingColumnsOnFilter', function () {
        if ($('#UseRelatingColumnsOnFilter').prop('checked')) {
            $p.set($('#UseGridHeaderFilters'), false);
            $('#UseGridHeaderFilters').prop('disabled', true);
        } else {
            $('#UseGridHeaderFilters').prop('disabled', false);
        }
    });
});
$p.setStartGuide = function (disable, redirect) {
    var data = {};
    data.DisableStartGuide = disable;
    $p.ajax(
        $('#ApplicationPath').val() + 'Users/SetStartGuide',
        'POST',
        data,
        undefined,
        true);
    if (redirect === 1) {
        location.href = $('#ApplicationPath').val();
    }
}
$(function () {
    $(document).on('change', '.control-dropdown', function () {
        var selectedCss = $(this).find('option:selected').attr('data-class');
        $(this).removeClass(function (index, css) {
            return (css.match(/\bstatus-\S+/g) || []).join(' ');
        });
        if (selectedCss !== undefined) {
            $(this).addClass(selectedCss);
        }
    });
});
$(function () {
    $(document).on('submit', function () {
        return false;
    });
})
$p.templates = function ($control) {
    $p.send($control, 'MainForm');
}

$p.setTemplate = function () {
    var selector = '.template-selectable .control-selectable';
    $('#TemplateTabsContainer:not(.applied)').tabs({
        beforeActivate: function (event, ui) {
            $p.setTemplateData(ui.newPanel.find(selector));
        }
    }).addClass('applied');
    $(selector).selectable({
        selected: function (event, ui) {
            $(ui.selected)
                .addClass("ui-selected")
                .siblings()
                .removeClass("ui-selected")
                .each(function (key, value) {
                    $(value).find('*').removeClass("ui-selected");
                });
        },
        stop: function () {
            $p.setTemplateData($(this));
        }
    });
}

$p.setTemplateData = function ($control) {
    var selected = $control.find('li.ui-selected');
    var show = selected.length === 1;
    $p.setData($control);
    $p.send($control);
    $('#OpenSiteTitleDialog')
        .removeClass('hidden')
        .toggle(show);
    $('#SiteTitle').val(show
        ? selected[0].innerText
        : '');
    $('#TemplateId').val(show
        ? selected[0].getAttribute('data-value')
        : '');
}

$p.setTemplateViewer = function () {
    $('.template-tab-container').tabs().addClass('applied');
}

$p.openSiteTitleDialog = function ($control) {
    $('#SiteTitleDialog').dialog({
        modal: true,
        width: '370px',
        appendTo: '#Application',
        resizable: false
    });
}
$p.drawTimeSeries = function () {
    $('#TimeSeriesValueField').toggle($('#TimeSeriesAggregateType').val() !== 'Count');
    var $svg = $('#TimeSeries');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#TimeSeriesJson').val());
    var indexes = json.Indexes;
    var elements = json.Elements;
    if (elements.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#TimeSeries');
    var padding = 40;
    var axisPaddingX = 130;
    var axisPaddingY = 50;
    var width = parseInt(svg.style('width'));
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPaddingX - (padding);
    var bodyHeight = height - axisPaddingY - (padding);
    var minDate = new Date(d3.min(elements, function (d) { return d.Day; }));
    var maxDate = new Date(d3.max(elements, function (d) { return d.Day; }));
    var dayWidth = (bodyWidth - padding) / $p.dateDiff('d', maxDate, minDate);
    var xScale = d3.scaleTime()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scaleLinear()
        .domain([d3.max(elements, function (d) {
            return d.Y;
        }), 0])
        .range([padding, bodyHeight])
        .nice();
    var xAxis = d3.axisBottom(xScale)
        .tickFormat(d3.timeFormat('%m/%d'))
        .ticks(10);
    var yAxis = d3.axisLeft(yScale);
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPaddingX + ', ' + (height - axisPaddingY) + ')')
        .call(xAxis)
        .selectAll('text')
        .attr('x', -20)
        .attr('y', 20)
        .style('text-anchor', 'start');
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPaddingX + ', 0)')
        .call(yAxis)
        .selectAll('text')
        .attr('x', -20);
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) { return d.Index === index.Id; });
        draw(ds);
    });
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) { return d.Index === index.Id; });
        if (ds.length !== 0) {
            var last = ds[ds.length - 1];
            var g = svg.append('g');
            g.append('text')
                .attr('class', 'index')
                .attr('x', ($p.dateDiff('d', new Date(last.Day), minDate) * dayWidth)
                    + axisPaddingX + padding - 10)
                .attr('y', yScale(last.Y - (last.Value / 2)))
                .attr('text-anchor', 'end')
                .attr('dominant-baseline', 'middle')
                .text(indexes.filter(function (d) { return d.Id === last.Index })[0].Text);
        }
    });

    function draw(ds) {
        var area = d3.area()
            .x(function (d) {
                return ($p.dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPaddingX + padding;
            })
            .y0(function (d) {
                return yScale(0);
            })
            .y1(function (d) {
                return yScale(d.Y);
            });
        var g = svg.append('g').attr('class', 'surface');
        g.append('path').attr('d', area(ds)).attr('fill', color());
    }

    function color() {
        var c = Math.floor(Math.random() * 50 + 180);
        return '#' + part(c) + part(c) + part(c);
    }

    function part(c) {
        return (c + Math.floor(Math.random() * 10 - 5)).toString(16);
    }
}
$p.returnOriginalUser = function (url) {
    $p.ajax(url, 'post', null, $('#SwitchUserInfo a'));
}
$(function () {
    $.validator.addMethod(
        'c_num',
        function (value, element) {
            return this.optional(element) || /^(-)?(¥|\\|\$)?[\d,.]+$/.test(value);
        }
    );
    $.validator.addMethod(
        'c_min_num',
        function (value, element, params) {
            return this.optional(element) ||
                parseFloat(value.replace(/[¥,]/g, '')) >= parseFloat(params);
        }
    );
    $.validator.addMethod(
        'c_max_num',
        function (value, element, params) {
            return this.optional(element) ||
                parseFloat(value.replace(/[¥,]/g, '')) <= parseFloat(params);
        }
    );

    $p.setValidationError = function ($form) {
        $form.find('.ui-tabs li').each(function () {
            $('.control-markdown.error').each(function () {
                $p.toggleEditor($(this), true);
            });
            var $control = $('#' + $(this)
                .attr('aria-controls'))
                .find('input.error:first:not(.search)');
            if ($control.length === 1) {
                $(this).closest('.ui-tabs').tabs('option', 'active', $(this).index());
                $control.focus();
            }
        });
    }

    $p.applyValidator = function () {
        $.extend($.validator.messages, {
            required: $p.display('ValidateRequired'),
            c_num: $p.display('ValidateNumber'),
            c_min_num: $p.display('ValidateMinNumber'),
            c_max_num: $p.display('ValidateMaxNumber'),
            date: $p.display('ValidateDate'),
            email: $p.display('ValidateEmail'),
            equalTo: $p.display('ValidateEqualTo'),
            maxlength: $p.display('ValidateMaxLength')
        });
        $('form').each(function () {
            $(this).validate({ ignore: '' });
        });
        $('[data-validate-required="1"]').each(function () {
            $(this).rules('add', { required: true });
        });
        $('[data-validate-number="1"]').each(function () {
            $(this).rules('add', { c_num: true });
        });
        $('[data-validate-min-number]').each(function () {
            $(this).rules('add', { c_min_num: $(this).attr('data-validate-min-number') });
        });
        $('[data-validate-max-number]').each(function () {
            $(this).rules('add', { c_max_num: $(this).attr('data-validate-max-number') });
        });
        $('[data-validate-date="1"]').each(function () {
            $(this).rules('add', { date: true });
        });
        $('[data-validate-email="1"]').each(function () {
            $(this).rules('add', { email: true });
        });
        $('[data-validate-equal-to]').each(function () {
            $(this).rules('add', { equalTo: $(this).attr('data-validate-equal-to') });
        });
        $('[data-validate-maxlength]').each(function () {
            $(this).rules('add', { maxlength: $(this).attr('data-validate-maxlength') });
        });
    }
    $p.applyValidator();
});
$p.openVideo = function (controlId) {
    navigator.mediaDevices.getUserMedia({ video: true, audio: false }).then(function (stream) {
        $('#VideoTarget').val(controlId);
        $('#VideoDialog').dialog({
            modal: true,
            width: '680px',
            appendTo: '#Application',
            resizable: false,
            close: function () {
                $p.videoTracks.forEach(function (track) { track.stop() });
            }
        });
        $p.video = document.getElementById('Video');
        $p.video.src = window.URL.createObjectURL(stream);
        $p.videoTracks = stream.getVideoTracks();
        $p.video.play();
    }).catch(function (error) {
        $p.setErrorMessage('CanNotConnectCamera');
        return;
    });
}

$p.toShoot = function ($control) {
    var canvas = document.getElementById('Canvas');
    var width = $p.video.offsetWidth;
    var height = $p.video.offsetHeight;
    canvas.setAttribute('width', width);
    canvas.setAttribute('height', height);
    canvas.getContext('2d').drawImage($p.video, 0, 0, width, height);
    canvas.toBlob(function (blob) {
        $p.uploadImage($('#VideoTarget').val(), blob);
    }, 'image/jpeg', 0.95);
    $p.closeDialog($control);
}
$p.viewMode = function ($control) {
    var url = $('.main-form').attr('action')
        .replace('_action_', $control.attr('data-action').toLowerCase());
    $p.ajax(url, 'post', $p.getData($control), $control);
    history.pushState(null, null, url);
}
$(function () {
    $('body').css({ visibility: 'visible' });
});
$(document).ready(function () {
    var methodType = $('#MethodType').val();
    if (methodType === 'edit' || methodType === 'new') {
        initRelatingColumn(
            $('#TriggerRelatingColumns_Editor'),
            $('#TableName').val());
    } else {
        initRelatingColumn(
            $('#TriggerRelatingColumns_Filter'),
            'ViewFilters_');
    }
    $p.initRelatingColumn = function () {
        initRelatingColumn(
            $('#TriggerRelatingColumns_Dialog'),
            $('#TableName').val());
    };

    function initRelatingColumn($trigger, tablename) {
        var param = $trigger.val();
        if (param === undefined) return;
        if (tablename === undefined) return;
        var rcols = JSON.parse(param);
        for (var k in rcols) {
            var prekey = '';
            for (var k2 in rcols[k].Columns) {
                if (prekey !== '' && rcols[k].Columns[k2] !== null && rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]] !== null) {
                    applyRelatingColumn(
                        prekey,
                        rcols[k].Columns[k2],
                        rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]],
                        tablename,
                        $trigger);
                }
                prekey = rcols[k].Columns[k2];
            }
        }
    }

    function applyRelatingColumn(prnt, chld, linkedClass, tablename, $trigger) {
        $(document).ready(function () {
            var debounce = function (fn, interval) {
                let timer;
                return function () {
                    clearTimeout(timer);
                    timer = setTimeout(function () {
                        fn();
                    }, interval);
                };
            };
            c_change(tablename);
            $(document).on(
                'change',
                '#' + tablename + '_' + prnt,
                debounce(function () {
                    c_change(tablename);
                }, 500));
        });
        var c_change = function (tablename) {
            var parentIds = [];
            var $parent = $('#' + tablename + '_' + prnt + ' option:selected');
            $parent.each(function (index, element) {
                var value = $(element).val();
                if (value === '\t') {
                    value = '-1';
                }
                parentIds.push(value);
            });
            var childIds = [];
            var $child = $('#' + tablename + '_' + chld + ' option:selected');
            $child.each(function (index, element) {
                childIds.push($(element).val());
            });
            $('#' + tablename + '_' + chld).attr('parent-data-class', linkedClass);
            $('#' + tablename + '_' + chld).attr('parent-data-id', JSON.stringify(parentIds));
            $('#' + tablename + '_' + chld).attr('selected-options', JSON.stringify(childIds));
            var formData = $p.getData($trigger.closest('form'));
            formData["RelatingDropDownControlId"] = tablename + '_' + chld;
            formData["RelatingDropDownSelected"] = JSON.stringify(childIds);
            formData["RelatingDropDownParentClass"] = linkedClass;
            formData["RelatingDropDownParentDataId"] = JSON.stringify(parentIds);
            $trigger.attr('parent-data-class', linkedClass);
            $trigger.attr('parent-data-id', JSON.stringify(parentIds));
            $trigger.attr('data-action', 'RelatingDropDown');
            $trigger.attr('data-method', 'post');
            const formId = undefined;
            const _async = true;
            const clearMessage = false;
            $p.send($trigger, formId, _async, clearMessage);
        };
    }
    $p.callbackRelatingColumn = function (targetId) {
        var $target = $(targetId);
        if ($target.length === 0) {
            return;
        }
        $p.RefreshMultiSelectRelatingColum($target);
        $target.addClass('not-set-form-changed');
        $target.trigger('change');
        $target.removeClass('not-set-form-changed');
    };
});
$p.moveColumns = function ($control, columnHeader, isKeepSource, isJoin) {
    if (formId === undefined) return false;
    return $p.moveColumnsById($control,
        columnHeader + 'Columns',
        columnHeader + 'SourceColumns',
        isKeepSource,
        isJoin !== undefined && isJoin === true ? columnHeader + 'Join' : undefined);
};
$p.moveColumnsById = function ($control, columnsId, srcColumnsId, isKeepSource, joinId) {
    if ($p.outsideDialog($control)) {
        alert("outsideDialog");
        return false;
    }
    if (formId === undefined) return false;
    if ($control.attr('id') === undefined || $control.attr('id') === null) return false;
    $form = $('#' + formId);
    var controlId = $control.attr('id');
    var mode = 0;
    var keepSource = (isKeepSource !== undefined && isKeepSource === true);
    if (controlId.indexOf('MoveUp') === 0) mode = 1;
    if (controlId.indexOf('MoveDown') === 0) mode = 2;
    if (controlId.indexOf('ToDisable') === 0) mode = 3;
    if (controlId.indexOf('ToEnable') === 0) mode = 4;
    if (mode === 0) return false;
    var data = $p.getData($form);
    var liListPool = [];
    var beforeColumns = [];
    var afterColumns = [];
    var beforeSourceColumns = [];
    var afterSourceColumns = [];
    var i = 0; j = 0;
    var o = null;
    var selected = $('#' + columnsId + ' li').map(function (i, elm) {
        if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
    });
    $('#' + columnsId + ' li').each(function (index, element) {
        beforeColumns.push($(element).attr("data-value"));
    });
    if (srcColumnsId !== '') {
        var srcSelected = $('#' + srcColumnsId + ' li').map(function (i, elm) {
            if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
        });
        $('#' + srcColumnsId + ' li').each(function (index, element) {
            beforeSourceColumns.push($(element).attr("data-value"));
        });
    }
    afterSourceColumns = [].concat(beforeSourceColumns);
    if (mode === 1 || mode === 2) {
        if (mode === 1) {
            beforeColumns = beforeColumns.reverse();
        }
        for (i = 0; i < beforeColumns.length; i++) {
            if (selected.get().indexOf(beforeColumns[i]) >= 0) {
                liListPool.push(beforeColumns[i]);
            }
            else {
                afterColumns.push(beforeColumns[i]);
                if (liListPool.length > 0) {
                    afterColumns = afterColumns.concat(liListPool);
                    liListPool = [];
                }
            }
        }
        if (liListPool.length > 0) {
            afterColumns = afterColumns.concat(liListPool);
            liListPool = [];
        }
        if (mode === 1) {
            beforeColumns = beforeColumns.reverse();
            afterColumns = afterColumns.reverse();
        }
    }
    else if (mode === 3) {
        $('#' + srcColumnsId + ' li').each(function (i, elm) {
            if ($(this).hasClass('ui-selected')) $(this).removeClass('ui-selected');
        });
        if ($('#' + columnsId + 'NessesaryColumns')) {
            var param = $('#' + columnsId + 'NessesaryColumns').val();
            if (param !== undefined) {
                var nessesaryColumns = JSON.parse(param);
                for (i = 0; i < selected.length; i++) {
                    if (nessesaryColumns.indexOf(selected[i]) >= 0) {
                        alert($('#' + columnsId + 'NessesaryMessage').val()
                            .replace("COLUMNNAME", $('#' + columnsId + ' li[data-value=\'' + selected[i] + '\'').html()));
                        return false;
                    }
                }
            }
        }
        afterColumns = [].concat(beforeColumns);
        var pos = afterSourceColumns.length;
        for (i = 0; i < selected.length; i++) {
            pos = afterSourceColumns.length;
            afterColumns.splice(afterColumns.indexOf(selected[i]), 1);
            if ((joinId === undefined
                || ($('#' + joinId + ' option:selected').val() !== '' && selected[i].indexOf($('#' + joinId + ' option:selected').val() + ',') >= 0)
                || ($('#' + joinId + ' option:selected').val() === '' && selected[i].indexOf(',') < 0))
                && !keepSource) {
                if ($('#' + columnsId + ' li[data-value=\'' + selected[i] + '\']').attr('data-order') !== undefined) {
                    for (j = 0; j < afterSourceColumns.length; j++) {
                        o = $('#' + columnsId + ' li[data-value=\'' + afterSourceColumns[j] + '\']');
                        if (!$(o).get(0)) o = $('#' + srcColumnsId + ' li[data-value=\'' + afterSourceColumns[j] + '\']');
                        if ($(o).attr('data-order') === undefined) break;
                        if (parseInt($('#' + columnsId + ' li[data-value=\'' + selected[i] + '\']').attr('data-order'), 10)
                            < parseInt($(o).attr('data-order'), 10)) {
                            pos = j;
                            break;
                        }
                    }
                }
                afterSourceColumns.splice(pos, 0, selected[i]);
            }
        }
    }
    else if (mode === 4) {
        $('#' + columnsId + ' li').each(function (i, elm) {
            if ($(this).hasClass('ui-selected')) $(this).removeClass('ui-selected');
        });
        afterColumns = [].concat(beforeColumns);
        for (i = 0; i < srcSelected.length; i++) {
            afterColumns.push(srcSelected[i]);
            if (!keepSource) afterSourceColumns.splice(afterSourceColumns.indexOf(srcSelected[i]), 1);
        }
    }
    var html = '';
    var srchtml = '';
    o = null;
    for (i = 0; i < afterColumns.length; i++) {
        o = $('#' + columnsId + ' li[data-value=\'' + afterColumns[i] + '\']');
        if (!$(o).get(0)) {
            o = $('#' + srcColumnsId + ' li[data-value=\'' + afterColumns[i] + '\']');
        }
        if ($(o).get(0)) {
            html += $(o).get(0).outerHTML;
        }
    }
    for (i = 0; i < afterSourceColumns.length; i++) {
        o = $('#' + columnsId + ' li[data-value=\'' + afterSourceColumns[i] + '\']');
        if (!$(o).get(0)) {
            o = $('#' + srcColumnsId + ' li[data-value=\'' + afterSourceColumns[i] + '\']');
        }
        if ($(o).get(0)) {
            srchtml += $(o).get(0).outerHTML;
        }
    }
    $('#' + columnsId).html(html);
    if (srcColumnsId !== '') {
        $('#' + srcColumnsId).html(srchtml);
        if (keepSource) {
            $('#' + srcColumnsId + ' li').removeClass('ui-selected');
        }
    }
};
$p.uploadTenantImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#TenantImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}
