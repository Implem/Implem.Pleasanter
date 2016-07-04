func.setKamban = function () {
    $('.kamban-item').draggable({
        revert: 'invalid'
    });
    $('.kamban-container').droppable({
        drop: function (e, ui) {
            var formData = getFormData($(this));
            formData["KambanId"] = $(ui.draggable).attr('data-id');
            formData[$('#TableName').val() + '_' + $('#KambanGroupByColumn').val()] =
                $(this).attr('data-value');
            requestByForm(getForm($(this)), $('#KambanGrid'));
        }
    });
};