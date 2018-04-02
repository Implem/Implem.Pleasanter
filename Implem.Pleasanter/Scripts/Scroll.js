$p.saveScroll = function () {
    $p.scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
    $p.scrollY = document.documentElement.scrollTop || document.body.scrollTop;
}

$p.loadScroll = function () {
    window.scroll($p.scrollX, $p.scrollY);
}

$p.clearScroll = function () {
    $p.scrollX = 0;
    $p.scrollY = 0;
}

$p.paging = function (selector) {
    var $control = $(selector);
    var $offset = $(selector + 'Offset');
    if ($control.length) {
        if ($(window).scrollTop() + $(window).height() >= $control.offset().top + $control.height()) {
            if ($offset.val() !== '-1') {
                $p.setData($offset);
                $offset.val(-1);
                $p.send($control);
            }
        }
    }
}

$p.setPaging = function (controlId) {
    var wrapper = document.getElementById(controlId + 'Wrapper');
    var height = wrapper.offsetHeight;
    $(wrapper).scroll(function () {
        var scrollHeight = wrapper.scrollHeight;
        var scrollTop = wrapper.scrollTop;
        var scrollPosition = height + scrollTop;
        var $offset = $('#' + controlId + 'Offset');
        if ((scrollHeight - scrollPosition) / scrollHeight <= 0 && $offset.val() !== '-1') {
            $p.send($('#' + controlId));
            $offset.val('-1');
        }
    });
}