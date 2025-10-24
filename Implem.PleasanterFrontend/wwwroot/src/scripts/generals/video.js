﻿$p.openVideo = function (controlId) {
    $p.video = document.getElementById('Video');
    $p.video.autoplay = true;
    $p.video.muted = true;
    $p.video.playsinline = true;
    $p.getVideoDeviceList().then(function (videoDeviceList) {
        var maxDeviceIdIndexNo = videoDeviceList.length - 1;
        if (maxDeviceIdIndexNo === 0) {
            $('#ChangeCamera').hide()
        }
        $p.playVideo(controlId, videoDeviceList, maxDeviceIdIndexNo, 0);
    }).catch(function (error) {
        $p.setErrorMessage('CanNotConnectCamera');
        return;
    });
}

$p.playVideo = function (controlId, videoDeviceList, maxDeviceIdIndexNo, deviceIdIndexNo) {
    var deviceId = videoDeviceList[deviceIdIndexNo].deviceId;
    navigator.mediaDevices.getUserMedia({ video: { deviceId }, audio: false }).then(function (stream) {
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
        $p.video.srcObject = stream;
        $p.video.onloadedmetadata = function (e) {
            $p.videoTracks = stream.getVideoTracks();
            $p.video.play();
            $('#ChangeCamera').on('click', function () {
                $p.changeCamera(controlId, videoDeviceList, $p.videoTracks, maxDeviceIdIndexNo, deviceIdIndexNo);
            });
        };
    }).catch(function (error) {
        $p.setErrorMessage('CanNotGetMediaInformation');
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

$p.changeCamera = function (controlId, videoDeviceList, $videoTracks, maxDeviceIdIndexNo, nowDeviceIdIndexNo) {
    var deviceIdIndexNo = nowDeviceIdIndexNo + 1;
    if (maxDeviceIdIndexNo - deviceIdIndexNo < 0) {
        deviceIdIndexNo = 0;
    }
    $('#ChangeCamera').off('click');
    $videoTracks.forEach(function (track) { track.stop() });
    $p.playVideo(controlId, videoDeviceList, maxDeviceIdIndexNo, deviceIdIndexNo);
}

$p.getVideoDeviceList = function () {
    if (
        !navigator.mediaDevices ||
        typeof navigator.mediaDevices.enumerateDevices !== "function"
    ) {
        $p.setErrorMessage('CanNotGetMediaInformation');
        return Promise.reject(new Error('enumerateDevices is not supported'));
    }
    if (
        /Safari/.test(navigator.userAgent) &&
        !/Chrome/.test(navigator.userAgent) &&
        typeof $p._safariEnumerateDevicesCalled === "undefined"
    ) {
        $p._safariEnumerateDevicesCalled = true;
        return navigator.mediaDevices.getUserMedia({ video: true, audio: false })
            .then(function (stream) {
                stream.getTracks().forEach(function (track) { track.stop(); });
                return $p.getVideoDeviceList();
            })
            .catch(function (error) {
                $p.setErrorMessage('CanNotGetMediaInformation');
                throw error;
            });
    }
    return navigator.mediaDevices.enumerateDevices().then(function (devices) {
        return devices.filter((device) => device.kind === "videoinput");
    }).catch(function (error) {
        $p.setErrorMessage('CanNotGetMediaInformation');
        throw error;
    });
}