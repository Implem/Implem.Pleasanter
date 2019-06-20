$p.openVideo = function (controlId) {
    navigator.mediaDevices.getUserMedia({ video: true, audio: false }).then(function (stream) {
        $('#VideoTarget').val(controlId);
        $('#VideoDialog').dialog({
            modal: true,
            width: '680px',
            appendTo: '#Application',
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