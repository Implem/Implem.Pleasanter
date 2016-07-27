func.setKamban = function () {
    $('.kamban-item').draggable({
        revert: 'invalid',
        start: function () {
            $(this).parent().droppable({
                disabled: true
            });
        }
    });
    $('.kamban-container').droppable({
        hoverClass: 'hover',
        tolerance: 'pointer',
        drop: function (e, ui) {
            var formData = getFormData($(this));
            formData["KambanId"] = $(ui.draggable).attr('data-id');
            formData[$('#TableName').val() + '_' + $('#KambanGroupByColumn').val()] =
                $(this).attr('data-value') !== undefined
                    ? $(this).attr('data-value')
                    : '';
            requestByForm(getForm($(this)), $('#KambanGrid'));
        }
    });
};
$(function () {
    $(document).on('dblclick', '.kamban-item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
    $(document).on('click', '.kamban-item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().attr('data-id');
    });
});