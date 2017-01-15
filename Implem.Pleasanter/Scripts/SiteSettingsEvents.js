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
        var $dataViewSorterOrderType = $('#ViewSorterOrderTypes option:selected');
        var orderType = $dataViewSorterOrderType.val();
        $p.addBasket(
            $('#ViewSorters'),
            $dataViewSorter.text() + '(' + $p.display('Order' + orderType) + ')',
            $dataViewSorter.val() + ',' + orderType);
    });
    $(document).on('click', '#CreateNotification,#UpdateNotification', function () {
        var $control = $(this);
        $p.getData($control).NotificationType = $('#NotificationType').val();
        $p.send($control);
    });
    $(document).on('click', '#NotificationSettings .grid-row', function () {
        var $control = $(this);
        $p.getData($control).NotificationId = $control.attr('data-id');
        $p.openNotificationDialog($('#EditNotification'), '#NotificationDialog');
    });
    $(document).on('click', '#NotificationSettings .delete', function (e) {
        var $control = $(this);
        e.stopPropagation();
        $p.getData($control).NotificationId = $control.attr('data-id');
        $p.send($('#DeleteNotification'));
    });
});