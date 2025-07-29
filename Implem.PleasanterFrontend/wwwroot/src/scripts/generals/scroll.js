$p.saveScroll = function () {
    $p.scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
    $p.scrollY = document.documentElement.scrollTop || document.body.scrollTop;
};

$p.loadScroll = function () {
    window.scroll($p.scrollX, $p.scrollY);
};

$p.clearScroll = function () {
    $p.scrollX = 0;
    $p.scrollY = 0;
};

$p.paging = function (selector) {
    if ($('.ui-dialog:visible').length > 0) {
        return;
    }
    var $control = $(selector);
    var $offset = $(selector + 'Offset');
    var $observer = $(selector + 'Observer');
    if ($control.length) {
        var $target = $observer.length ? $observer : $control;
        if ($(window).scrollTop() + window.innerHeight >= $target.offset().top + $target.height()) {
            if ($offset.val() !== '-1') {
                $p.setData($offset);
                $offset.val('-1');
                $p.send($control);
            }
        }
    }
};
$p.dashboardPaging = function (selector, target) {
    if ($('.ui-dialog:visible').length > 0) {
        return;
    }
    var $control = $(selector);
    var suffix = selector.substring(selector.indexOf('_'));
    var $offset = $('#Grid' + 'Offset' + suffix);
    if ($control.length) {
        if (
            $(target).scrollTop() + $(target).height() >=
            $control.offset().top + $control.height()
        ) {
            if ($offset.val() !== '-1') {
                var gridId = $control.attr('id');
                var tableId = $control.attr('id');
                if (tableId.match(/_(\d+)$/)) {
                    tableId = tableId.slice(0, tableId.lastIndexOf('_'));
                }
                var offsetId = $offset.attr('id');
                var offsetTableId = $offset.attr('id');
                if (offsetTableId.match(/_(\d+)$/)) {
                    offsetTableId = offsetTableId.slice(0, offsetTableId.lastIndexOf('_'));
                }
                $control.attr('id', tableId);
                $offset.attr('id', offsetTableId);
                $p.setData($offset);
                $p.getData($control).IndexSuffix = suffix;
                $offset.val('-1');
                $p.send($control);
                $control.attr('id', gridId);
                $offset.attr('id', offsetId);
            }
        }
    }
};

$p.setPaging = function (controlId, offsetId) {
    var wrapper = document.getElementById(controlId + 'Wrapper');
    if (wrapper) {
        var height = wrapper.offsetHeight;
        $(wrapper).scroll(function () {
            var scrollHeight = wrapper.scrollHeight;
            var scrollTop = wrapper.scrollTop;
            var scrollPosition = height + scrollTop;
            var $offset = $('#' + (offsetId === undefined ? controlId + 'Offset' : offsetId));
            if ((scrollHeight - scrollPosition) / scrollHeight <= 0 && $offset.val() !== '-1') {
                $p.send($('#' + controlId));
                $offset.val('-1');
            }
        });
    }
};

$p.clearScrollTop = function (controlId) {
    document.getElementById(controlId).scrollTop = 0;
};
