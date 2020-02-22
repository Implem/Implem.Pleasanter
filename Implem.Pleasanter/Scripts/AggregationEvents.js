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
            $p.set($control, $control.find(':selected').length === 0
                ? '["' + $(this).attr('data-value') + '"]'
                : '[]');
            $p.send($control);
        }
    });
    $(document).on('click', '#Aggregations .overdue', function () {
        $('#ViewFilters_Overdue').click();
    });
});