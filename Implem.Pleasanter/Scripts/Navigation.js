﻿$p.currentIndex = function (array) {
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
        $p.transition($control.val());
    }
}

$p.transition = function(url) {
    try {
        location.href = url;
    } catch (e) {
        if (e.number !== -2147467259) {
            throw e;
        }
    }
};