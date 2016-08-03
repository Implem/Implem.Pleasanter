$(document).on('click', '.burn-down-details-row', function () {
    if (!$(this).next().hasClass('burn-down-record-details')) {
        var data = $p.getData();
        data.BurnDownDate = $(this).attr('data-date');
        data.BurnDownColspan = $(this).find('td').length;
        $p.send($(this));
    } else {
        $(this).next().remove();
    }
});
$(document).on('click', '.burn-down-record-details', function () {
    $(this).remove();
});