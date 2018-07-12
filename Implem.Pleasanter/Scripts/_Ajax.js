$p.ajax = function (url, methodType, data, $control, async) {
    $p.before_send($p.eventArgs(url, methodType, data, $control, async));
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
    async = async !== undefined ? async : true;
    $p.clearMessage();
    $.ajax({
        url: url,
        type: methodType,
        async: async,
        cache: false,
        data: data,
        dataType: 'json'
    })
    .done(function (json, textStatus, jqXHR) {
        $p.setByJson(url, methodType, data, $control, async, json);
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
    $p.after_send($p.eventArgs(url, methodType, data, $control, async, ret));
    return ret;
}

$p.upload = function (url, methodType, data, $control) {
    $p.loading($control);
    $p.clearMessage();
    return $.ajax({
        url: url,
        type: methodType,
        cache: false,
        contentType: false,
        processData: false,
        data: data
    })
    .done(function (json, textStatus, jqXHR) {
        $p.setByJson(url, methodType, data, $control, true, json);
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
    });
}

$p.multiUpload = function (url, data, $control, statusBar) {
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