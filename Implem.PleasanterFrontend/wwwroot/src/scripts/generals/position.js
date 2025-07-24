$p.hoverd = function ($elements) {
    var $element;
    $elements.each(function () {
        if (isHover($(this))) {
            $element = $(this);
            return false;
        }
    });
    return $element;

    function isHover($element) {
        var left = $element.offset().left;
        var top = $element.offset().top;
        var right = left + $element.outerWidth();
        var bottom = top + $element.outerHeight();
        if ($p.mouseX >= left &&
            $p.mouseX <= right &&
            $p.mouseY >= top &&
            $p.mouseY <= bottom) {
            return true;
        } else {
            return false
        }
    }
}