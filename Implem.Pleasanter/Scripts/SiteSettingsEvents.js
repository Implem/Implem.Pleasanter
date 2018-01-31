$(function () {
    $(document).on('change', '#UseGridDesign', function () {
        $('#GridDesignField').toggle($(this).prop('checked'));
    });
    $(document).on('change', '#ControlType', function () {
        var visibility = $(this).val() === 'Spinner';
        $('#MinField').toggle(visibility);
        $('#MaxField').toggle(visibility);
        $('#StepField').toggle(visibility);
    });
    $(document).on('change', '#FormatSelector', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('#FormatField').toggle(true);
                $('#Format').val('');
                break;
            default:
                $('#FormatField').toggle(false);
                $('#Format').val($control.val());
                break;
        }
        $p.setData($('#Format'));
    });
    $(document).on('change', '#NotificationType', function () {

        $('#NotificationTokenField').toggle(
            $('#NotificationTokenEnableList').val()
                .split(',')
                .indexOf($('#NotificationType').val()) !== -1);

    });
    $(document).on('click', '#SummarySettings .grid-row button', function () {
        var $control = $($(this).attr('data-selector'))
        $p.getData($control)[$control.attr('id') + "Id"] = $(this).attr('data-id');
        $p.send($control);
    });
    $(document).on('change', '#SummaryDestinationCondition', function () {
        $('#SummarySetZeroWhenOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('change', '#FormulaCondition', function () {
        $('#FormulaOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('click', '#AddViewSorter', function () {
        var $dataViewSorter = $('#ViewSorterSelector option:selected');
        var orderType = $('#ViewSorterOrderTypes option:selected').val();
        var orderText = '';
        switch (orderType) {
            case 'asc':
                orderText = '(' + $p.display('OrderAsc') + ')';
                break;
            case 'desc':
                orderText = '(' + $p.display('OrderDesc') + ')';
                break;
        }
        $p.addBasket(
            $('#ViewSorters'),
            $dataViewSorter.text() + orderText,
            $dataViewSorter.val() + '&' + orderType);
    });
    $(document).on('change', '#StyleAll', function () {
        if ($('#StyleAll').prop('checked')) {
            $('.output-destination-style')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-style')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#ScriptAll', function () {
        if ($('#ScriptAll').prop('checked')) {
            $('.output-destination-script')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-script')
                .removeClass('hidden')
                .find('input');
        }
    });
});