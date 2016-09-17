$p.currentIndex = function (array) {
    return array.indexOf($('#Id').val())
}

$p.switchTargets = function () {
    var $control = $('#SwitchTargets');
    return $control.length === 1
        ? $control.val().split(',')
        : [];
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