$(function () {
    $(document).on('dblclick', '.kamban-item', function () {
        $p.transition($('#BaseUrl').val() + $(this).attr('data-id'));
    });
    $(document).on('click', '.kamban-item .ui-icon-pencil', function () {
        $p.transition($('#BaseUrl').val() + $(this).parent().attr('data-id'));
    });
});
