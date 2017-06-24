$(function () {
    $(document).on('dblclick', '.kamban-item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
    $(document).on('click', '.kamban-item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().attr('data-id');
    });
});