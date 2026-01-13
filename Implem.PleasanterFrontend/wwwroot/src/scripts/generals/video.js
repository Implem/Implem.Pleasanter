$p.openVideo = function (controlId) {
    $p.video = document.getElementById('Video');
    $p.video.autoplay = true;
    $p.video.muted = true;
    $p.video.playsinline = true;
    $p.getVideoDeviceList()
        .then(function (videoDeviceList) {
            var maxDeviceIdIndexNo = videoDeviceList.length - 1;
            if (maxDeviceIdIndexNo === 0) {
                $('#ChangeCamera').hide();
            }
            $p.playVideo(controlId, videoDeviceList, maxDeviceIdIndexNo, 0);
        })
        .catch(function (error) {
            $p.setErrorMessage('CanNotConnectCamera');
            return;
        });
};

$p.playVideo = function (controlId, videoDeviceList, maxDeviceIdIndexNo, deviceIdIndexNo) {
    var deviceId = videoDeviceList[deviceIdIndexNo].deviceId;
    navigator.mediaDevices
        .getUserMedia({ video: { deviceId }, audio: false })
        .then(function (stream) {
            $('#VideoTarget').val(controlId);
            $('#VideoDialog').dialog({
                modal: true,
                width: '680px',
                resizable: false,
                close: function () {
                    $p.videoTracks.forEach(function (track) {
                        track.stop();
                    });
                }
            });
            $p.video.srcObject = stream;
            $p.video.onloadedmetadata = function (e) {
                $p.videoTracks = stream.getVideoTracks();
                $p.video.play();
                $('#ChangeCamera').on('click', function () {
                    $p.changeCamera(
                        controlId,
                        videoDeviceList,
                        $p.videoTracks,
                        maxDeviceIdIndexNo,
                        deviceIdIndexNo
                    );
                });
            };
        })
        .catch(function (error) {
            $p.setErrorMessage('CanNotGetMediaInformation');
            return;
        });
};

$p.toShoot = function ($control) {
    var canvas = document.getElementById('Canvas');
    var width = $p.video.offsetWidth;
    var height = $p.video.offsetHeight;
    canvas.setAttribute('width', width);
    canvas.setAttribute('height', height);
    canvas.getContext('2d').drawImage($p.video, 0, 0, width, height);
    canvas.toBlob(
        function (blob) {
            $p.uploadImage($('#VideoTarget').val(), blob);
        },
        'image/jpeg',
        0.95
    );
    $p.closeDialog($control);
};

$p.changeCamera = function (
    controlId,
    videoDeviceList,
    $videoTracks,
    maxDeviceIdIndexNo,
    nowDeviceIdIndexNo
) {
    var deviceIdIndexNo = nowDeviceIdIndexNo + 1;
    if (maxDeviceIdIndexNo - deviceIdIndexNo < 0) {
        deviceIdIndexNo = 0;
    }
    $('#ChangeCamera').off('click');
    $videoTracks.forEach(function (track) {
        track.stop();
    });
    $p.playVideo(controlId, videoDeviceList, maxDeviceIdIndexNo, deviceIdIndexNo);
};

$p.getVideoDeviceList = function () {
    if (!navigator.mediaDevices || typeof navigator.mediaDevices.enumerateDevices !== 'function') {
        $p.setErrorMessage('CanNotGetMediaInformation');
        return Promise.reject(new Error('enumerateDevices is not supported'));
    }
    if (
        /Safari/.test(navigator.userAgent) &&
        !/Chrome/.test(navigator.userAgent) &&
        typeof $p._safariEnumerateDevicesCalled === 'undefined'
    ) {
        $p._safariEnumerateDevicesCalled = true;
        return navigator.mediaDevices
            .getUserMedia({ video: true, audio: false })
            .then(function (stream) {
                stream.getTracks().forEach(function (track) {
                    track.stop();
                });
                return $p.getVideoDeviceList();
            })
            .catch(function (error) {
                $p.setErrorMessage('CanNotGetMediaInformation');
                throw error;
            });
    }
    return navigator.mediaDevices
        .enumerateDevices()
        .then(function (devices) {
            return devices.filter(device => device.kind === 'videoinput');
        })
        .catch(function (error) {
            $p.setErrorMessage('CanNotGetMediaInformation');
            throw error;
        });
};

$p.uploadImage = function (controlId, file) {
    const controller = $p.isForm() ? 'formbinaries' : 'binaries';
    var $tr = $('[id="' + controlId + '"]').closest('tr');
    var $editorInDialogRecordId = $('#EditorInDialogRecordId');
    var url;
    if ($tr.length) {
        url = $('#BaseUrl').val() + $tr.data('id') + '/' + controller + '/uploadimage';
    } else if ($('#EditorDialog').is(':visible') && $editorInDialogRecordId.length) {
        url =
            $('#BaseUrl').val() + $editorInDialogRecordId.val() + '/' + controller + '/uploadimage';
    } else {
        url = $('.main-form')
            .attr('action')
            .replace('_action_', controller + '/uploadimage');
    }
    var data = new FormData();
    data.append('ControlId', controlId);
    data.append('file', file);
    $p.multiUpload(url, data);
};

$p.insertText = function ($control, value) {
    var body = $control.get(0);
    body.focus();
    var start = body.value;
    var caret = body.selectionStart;
    var next = caret + value.length;
    body.value = start.slice(0, caret) + value + start.slice(caret);
    body.setSelectionRange(next, next);
    $p.setData($control);

    var markdownField = $control.get(0).closest('markdown-field');
    if (markdownField && !$control.is(':visible')) {
        markdownField.showViewer();
    }
};
