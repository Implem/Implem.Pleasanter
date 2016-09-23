$(document).on('click', '#BurnDownDetails > tbody > tr', function () {
    if (!$(this).next().hasClass('items')) {
        var data = $p.getData();
        data.BurnDownDate = $(this).attr('data-date');
        data.BurnDownColspan = $(this).find('td').length;
        $p.send($(this));
    } else {
        $(this).next().remove();
    }
});
$(document).on('click', '#BurnDownDetails .items', function () {
    $(this).remove();
});