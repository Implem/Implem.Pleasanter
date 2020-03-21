$(function () {
    $(document).on('change', '#AggregationType', function () {
        if ($(this).val() === 'Count') {
            $('#AggregationTarget').closest('.togglable').hide();
        } else {
            $('#AggregationTarget').closest('.togglable').show();
        }
    });
    $(document).on('click', '#Aggregations .data.link', function () {
        var $control = $($(this).attr('data-selector'));
        if ($control.length === 1) {
            var value = $(this).attr('data-value');
            $control.multiselect('widget').find(':checkbox').each(function () {
                if ($(this).val() === value) {
                    $(this).click();
                    return;
                }
            });
        }
    });
    $(document).on('click', '#Aggregations .overdue', function () {
        $('#ViewFilters_Overdue').click();
    });
});