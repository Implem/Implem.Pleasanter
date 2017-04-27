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