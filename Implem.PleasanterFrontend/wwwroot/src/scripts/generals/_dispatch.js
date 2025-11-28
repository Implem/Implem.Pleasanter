$p.setByJson = function (url, methodType, data, $control, _async, json) {
    if (url) {
        $p.before_set($p.eventArgs(url, methodType, data, $control, _async, undefined, json));
    }
    if (json) {
        $.each(json, function () {
            $p.setByJsonElement(this, data, $control);
        });
    }
    if (
        json.filter(function (d) {
            return (
                d.Method === 'Html' ||
                d.Method === 'ReplaceAll' ||
                d.Method === 'Append' ||
                d.Method === 'Prepend' ||
                d.Method === 'After' ||
                d.Method === 'Before'
            );
        }).length > 0
    ) {
        $p.apply();
        $p.applyValidator();
    }
    if (url) {
        $p.after_set($p.eventArgs(url, methodType, data, $control, _async, undefined, json));
    }
};

$p.setByJsonElement = function (jsonElement, data, $control) {
    var method = jsonElement.Method;
    var target = jsonElement.Target;
    var value = jsonElement.Value;
    var options = jsonElement.Options !== undefined ? JSON.parse(jsonElement.Options) : {};
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
            $p.transition(value);
            break;
        case 'PushState':
            history.pushState(target, '', value);
            break;
        case 'Set':
            var $target = $p.getControl(target);
            if (!$target) {
                $target = $(target);
            }
            $p.set($target, value);
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
        case 'Attr': {
            let json = JSON.parse(value);
            $(target).attr(json.Name, json.Value);
            break;
        }
        case 'RemoveAttr':
            $(target).removeAttr(value);
            break;
        case 'Css': {
            let json = JSON.parse(value);
            $(target).css(json.Name, json.Value);
            break;
        }
        case 'Focus':
            if (target === '') {
                $('#' + data.ControlId).focus();
            } else {
                $(target).focus();
            }
            break;
        case 'SetValue':
            $p.setValue($(target), value);
            $p.hideField(target, options);
            break;
        case 'ClearFormData':
            $p.clearData(target, data, value);
            break;
        case 'CloseDialog':
            $p.clearMessage();
            if (target !== undefined) {
                if ($(target).hasClass('ui-dialog-content')) {
                    $(target).dialog('close');
                }
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
        case 'Click':
            $(target).click();
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
        case 'Log':
            if (value) {
                console.log(value);
            }
            break;
    }
};
