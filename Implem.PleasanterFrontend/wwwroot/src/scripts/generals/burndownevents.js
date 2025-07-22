$(function () {
    $(document).on('click', '#BurnDownDetails > tbody > tr', function () {
        var $control = $(this);
        if (!$control.next().hasClass('items')) {
            var data = $p.getData($control);
            data.BurnDownDate = $control.attr('data-date');
            data.BurnDownColspan = $control.find('td').length;
            $p.send($control);
        } else {
            $control.next().remove();
        }
    });
    $(document).on('click', '#BurnDownDetails .items', function () {
        $(this).remove();
    });
});