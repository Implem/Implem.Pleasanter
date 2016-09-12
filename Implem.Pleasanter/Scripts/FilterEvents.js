$(function () {
    $(document).on('click', '#DataViewFilters_Reset', function () {
        $('[id^="DataViewFilters_"]').each(function () {
            var $control = $(this);
            switch ($control.prop('tagName')) {
                case 'INPUT':
                    switch ($control.prop('type')) {
                        case 'checkbox': $control.prop('checked', false); break;
                        case 'text': $control.val(''); break;
                    }
                    break;
                case 'SELECT':
                    if ($control.attr('multiple')) {
                        $control
                            .addClass('no-postback')
                            .multiselect('uncheckAll');
                    } else {
                        $control.val('');
                    }
                    break;
            }
            $p.setData($control);
        });
        $p.send($(this));
    });
});