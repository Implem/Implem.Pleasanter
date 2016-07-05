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
        tolerance: 'pointer',
        drop: function (e, ui) {
            var formData = getFormData($(this));
            formData["KambanId"] = $(ui.draggable).attr('data-id');
            formData[$('#TableName').val() + '_' + $('#KambanGroupByColumn').val()] =
                $(this).attr('data-value');
            requestByForm(getForm($(this)), $('#KambanGrid'));
        }
    });
    $(document).on('dblclick', '.kamban-item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
};