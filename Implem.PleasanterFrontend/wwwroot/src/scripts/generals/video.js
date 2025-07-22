$p.openVideo = function (controlId, deviceIdIndexNo = 0) {
    $p.getVideoDeviceList().then(function (videoDeviceList) {
        const maxDeviceIdIndexNo = videoDeviceList.length - 1;
        if (maxDeviceIdIndexNo === 0) {
            // カメラが一つなら切替ボタンを表示しない
        }
        if (maxDeviceIdIndexNo - deviceIdIndexNo < 0) {
            deviceIdIndexNo = 0;
        }
        let deviceId = videoDeviceList[deviceIdIndexNo].deviceId;
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
            $p.video = document.getElementById('Video');
            $p.video.srcObject = stream;
            $p.video.onloadedmetadata = function (e) {
                $p.videoTracks = stream.getVideoTracks();
                $p.video.play();
                $('#ChangeCamera').on('click', function () {
                    $p.changeCamera($(this), controlId, deviceIdIndexNo);
                });
            };
        }).catch(function (error) {
            $p.setErrorMessage('CanNotConnectCamera');
            return;
        });
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

$p.changeCamera = function ($control, controlId, deviceIdIndexNo) {
// index番号はイベントを設定した時点のものが保持されているハズ(Mediaが動いたままなので)
    const nowDeviceIdIndexNo = deviceIdIndexNo + 1;
    $('#ChangeCamera').off('click');
    $p.closeDialog($control);
    $p.openVideo(controlId, nowDeviceIdIndexNo);
}

$p.getVideoDeviceList = function () {
    return new Promise((resolve, reject) => navigator.mediaDevices.enumerateDevices().then((devices) => {
        resolve(devices.filter((device) => device.kind === "videoinput"));
    }).catch(function (error) {
        $p.setErrorMessage('CanNotGetMediaInformation');
        reject(error);
    }));
}