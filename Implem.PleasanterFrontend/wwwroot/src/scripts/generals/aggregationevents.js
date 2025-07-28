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
            if ($(this).attr('data-selector').match('Check')) {
                $p.set($control, $(this).attr('data-value') === 'True');
            } else {
                $p.set(
                    $control,
                    $(this).hasClass('no-choice')
                        ? $control.val() !== $(this).attr('data-value')
                            ? $(this).attr('data-value')
                            : ''
                        : $control.find(':selected').length === 0
                          ? '["' + $(this).attr('data-value') + '"]'
                          : '[]'
                );
            }
            $p.send($control);
        }
    });
    $(document).on('click', '#Aggregations .overdue', function () {
        $('#ViewFilters_Overdue').click();
    });
});
