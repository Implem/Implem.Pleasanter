$(function () {
    $(document).on('dblclick', '#Calendar .item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id') + '?back=1';
    });
    $(document).on('click', '#Calendar .item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().attr('data-id') + '?back=1';
    });
});