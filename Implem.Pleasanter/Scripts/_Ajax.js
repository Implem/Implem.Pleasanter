$p.ajax = function (requestUrl, methodType, data, $eventSender, async) {
    $p.loading($eventSender);
    var ret = 0;
    async = async !== undefined ? async : true;
    $p.clearMessage();
    $.ajax({
        url: requestUrl,
        type: methodType,
        async: async,
        cache: false,
        data: data,
        dataType: 'json'
    })
    .done(function (json, textStatus, jqXHR) {
        $p.setByJson(json, data, $eventSender);
        ret = json.filter(function (i) {
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
        $p.clearData('ControlId', data);
        $p.loaded();
    });
    return ret;
}

$p.upload = function (requestUrl, methodType, data, $eventSender) {
    $p.loading($eventSender);
    $p.clearMessage();
    return $.ajax({
        url: requestUrl,
        type: methodType,
        cache: false,
        contentType: false,
        processData: false,
        data: data
    })
    .done(function (response, textStatus, jqXHR) {
        $p.setByJson(JSON.parse(response), data, $eventSender);
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

$p.multiUpload = function (formData, statusBar, uploadUrl) {
    $p.clearMessage();
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
                    statusBar.setProgress(percent);
                }, false);
            }
            return uploadobj;
        },
        url: uploadUrl,
        type: 'POST',
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (data) {
            statusBar.setProgress(100);
        }
    })
        .done(function (response, textStatus, jqXHR) {
            $p.setByJson(JSON.parse(response), formData);
            return true;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + '\n' +
                $(jqXHR.responseText).text().trim().replace('\n', ''));
            return false;
        })
        .always(function (jqXHR, textStatus) {
            $p.clearData('ControlId', formData);
            $p.loaded();
            statusBar.status.hide();
        });
    statusBar.setAbort(uploader);
}