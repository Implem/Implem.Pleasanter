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
    $p.execEvents('ajax_before_send', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
    $.ajax({
        url: url,
        type: methodType,
        async: _async,
        cache: false,
        data: data,
        dataType: 'json'
    })
    .done(function (json, textStatus, jqXHR) {
        $p.execEvents('ajax_before_done', $p.eventArgs(url, methodType, data, $control, _async, ret, json));
        $p.setByJson(url, methodType, data, $control, _async, json);
        ret = json.filter(function (i) {
            return i.Method === 'Message' && JSON.parse(i.Value).Css === 'alert-error';
        }).length !== 0
            ? -1
            : 0;
        $p.execEvents('ajax_after_done', $p.eventArgs(url, methodType, data, $control, _async, ret, json));
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        $p.execEvents('ajax_before_fail', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
        alert(textStatus + '\n' +
            $(jqXHR.responseText).text().trim().replace('\n', ''));
        ret = -1;
        $p.execEvents('ajax_after_fail', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
    })
    .always(function (jqXHR, textStatus) {
        $p.execEvents('ajax_before_always', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
        $p.clearData('ControlId', data);
        $p.loaded();
        $p.execEvents('ajax_after_always', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
    });
    $p.execEvents('ajax_after_send', $p.eventArgs(url, methodType, data, $control, _async, ret, null));
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