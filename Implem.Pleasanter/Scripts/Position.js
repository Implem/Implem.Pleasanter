var mouseX;
var mouseY;

$(window).mousemove(function (e) {
    mouseX = e.pageX;
    mouseY = e.pageY;
});

function getHoveredItem($elements) {
    var $element;
    $elements.each(function () {
        if (isHover($(this))) {
            $element = $(this);
            return false;
        }
    });
    return $element;
}

function isHover($element) {
    var left = $element.offset().left;
    var top = $element.offset().top;
    var right = left + $element.outerWidth();
    var bottom = top + $element.outerHeight();
    if (mouseX >= left &&
        mouseX <= right &&
        mouseY >= top &&
        mouseY <= bottom) {
        return true;
    } else {
        return false
    }
}